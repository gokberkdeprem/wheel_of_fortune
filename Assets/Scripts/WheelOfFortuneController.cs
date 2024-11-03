using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class WheelOfFortuneController : MonoBehaviour
{
    public RectTransform wheelTransform;
    private float[] probabilities;
    private float[] cumulativeProbabilities;
    public float spinDuration = 4f;
    private int targetSlot;
    private Zone currentZone;
    public List<Slot> Prizes = new List<Slot>();
    [SerializeField] private List<WheelSlot> wheelSlots = new List<WheelSlot>();
    [SerializeField] private WheelSlot prizePrefab;
    [SerializeField] private Transform prizeColumn;
    [SerializeField] private Dictionary<int, WheelSlot> _prizesDictionary = new Dictionary<int, WheelSlot>();

    [Header("WheelImage")]
    [SerializeField] private Image wheelImage;
    [SerializeField] private Sprite defaultWheel;
    [SerializeField] private Sprite silverWheel;
    [SerializeField] private Sprite goldenWheel;
    [SerializeField] private float bombProbability = 0.125f;
    public float bombProbabilityIncrement = 0.01f;

    public Button spinButton;

    private void OnValidate()
    {
        spinButton = GameObject.Find("spin_button").GetComponent<Button>();
        spinButton.onClick.AddListener(SpinWheel);
    }

    void Start()
    {
        InitWheel();
    }

    public void InitWheel()
    {
        currentZone = ZoneManager.Instance.CurrentZone;
        bombProbability += bombProbabilityIncrement;

        probabilities = currentZone.Slots.Select(x => x.Possibility).ToArray();
        for (int i = 0; i < currentZone.Slots.Count; i++)
        {
            if (currentZone.Slots[i].IsBomb)
                probabilities[i] += bombProbability;
        }

        float total = probabilities.Sum();
        for (int i = 0; i < probabilities.Length; i++)
            probabilities[i] /= total;

        cumulativeProbabilities = new float[probabilities.Length];
        cumulativeProbabilities[0] = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];

        for (int i = 0; i < wheelSlots.Count; i++)
        {
            wheelSlots[i].Image.sprite = currentZone.Slots[i].Icon;
            wheelSlots[i].Text.text = currentZone.Slots[i].IsBomb ? "" : "X" + currentZone.Slots[i].Multiplier.ToString();
        }

        if (ZoneManager.Instance.ZoneCount % 30 == 0)
            wheelImage.sprite = goldenWheel;
        else if (ZoneManager.Instance.ZoneCount % 5 == 0 || ZoneManager.Instance.ZoneCount == 1)
            wheelImage.sprite = silverWheel;
        else
            wheelImage.sprite = defaultWheel;
    }
    
    public void SpinWheel()
    {
        ZoneManager.Instance.exitButton.enabled = false;
        spinButton.enabled = false;
        targetSlot = GetRandomSlotIndex();
        
        float targetAngle = 360f / probabilities.Length * targetSlot;
        float finalAngle = 360 + targetAngle;

        wheelTransform.DORotate(new Vector3(0, 0, 360 * 2), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                wheelTransform.DORotate(new Vector3(0, 0, finalAngle), spinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutCubic).OnComplete(GiveResult);
            });
    }

    private void GiveResult()
    {
        if (currentZone.Slots[targetSlot].IsBomb)
        {
            var currentPrize = currentZone.Slots[targetSlot];
            var prize = Instantiate(prizePrefab, gameObject.transform);
            prize.transform.DOScale(new Vector3(10, 10, 10), 0.5f).OnComplete(() => ZoneManager.Instance.ShowRetryButton());
            prize.Image.sprite = currentPrize.Icon;
            prize.Text.text = "";

            foreach (Transform child in prizeColumn.transform)
            {
                _prizesDictionary.Clear();
                Destroy(child.gameObject);
            }
        }
        else
        {
            PlayPrizeAnimation();
        }
    }

    private void PlayPrizeAnimation()
    {
        var currentPrize = currentZone.Slots[targetSlot];
       
        if (_prizesDictionary.ContainsKey(currentPrize.SlotId))
        {
            var prize = Instantiate(prizePrefab, gameObject.transform);
            prize.Image.sprite = currentPrize.Icon;
            prize.Text.text = currentPrize.Multiplier.ToString();

            prize.transform.DOMove(_prizesDictionary[currentPrize.SlotId].transform.position, 0.5f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    int.TryParse(_prizesDictionary[currentPrize.SlotId].Text.text, out var value);
                    value += currentPrize.Multiplier;
                    _prizesDictionary[currentPrize.SlotId].Text.text = value.ToString();
                    Destroy(prize.gameObject);
                    ZoneManager.Instance.exitButton.enabled = true;
                    spinButton.enabled = true;
                    ZoneManager.Instance.NextZone();
                });
        }
        else
        {
            WheelSlot prizeUnderPrizeColumn = Instantiate(prizePrefab, prizeColumn);
            prizeUnderPrizeColumn.Image.sprite = currentPrize.Icon;
            prizeUnderPrizeColumn.Text.text = currentPrize.Multiplier.ToString();
            prizeUnderPrizeColumn.gameObject.SetActive(false);

            WheelSlot prizeToMove = Instantiate(prizePrefab, gameObject.transform);
            prizeToMove.Image.sprite = currentPrize.Icon;
            prizeToMove.Text.text = currentPrize.Multiplier.ToString();

            prizeToMove.transform.DOMove(prizeColumn.position, 0.5f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Destroy(prizeToMove.gameObject);
                    prizeUnderPrizeColumn.gameObject.SetActive(true);
                    ZoneManager.Instance.NextZone();
                    ZoneManager.Instance.exitButton.enabled = true;
                    spinButton.enabled = true;
                });
            _prizesDictionary.Add(currentPrize.SlotId, prizeUnderPrizeColumn);
        }
    }
    
    private int GetRandomSlotIndex()
    {
        float randomValue = Random.Range(0f, 1f);
        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue <= cumulativeProbabilities[i])
                return i;
        }
        return cumulativeProbabilities.Length - 1;
    }
}
