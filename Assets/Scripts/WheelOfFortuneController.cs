using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WheelOfFortuneController : MonoBehaviour
{
    public RectTransform wheelTransform;   // Reference to the wheel's RectTransform
    private float[] probabilities;
    private float[] cumulativeProbabilities;
    public float spinDuration = 4f;
    private int targetSlot;
    private Zone currentZone;
    [SerializeField] private List<WheelSlot> wheelSlots = new List<WheelSlot>();
    
    [Header("WheelImage")]
    [SerializeField] private Image wheelImage;

    [SerializeField] private Sprite defaultWheel;
    [SerializeField] private Sprite silverWheel;
    [SerializeField] private Sprite goldenWheel;
    
    public Button spinButton;

    void Awake()
    {
        spinButton.onClick.AddListener(SpinWheel);
    }

    void Start()
    {
        InitWheel();
    }
    public void InitWheel()
    {
        currentZone = ZoneManager.Instance.GetCurrentZone();
        probabilities = new float[8];
        probabilities = currentZone.Slots.Select(x => x.Possibility).ToArray();
        cumulativeProbabilities = new float[probabilities.Length];
        cumulativeProbabilities[0] = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
        }

        for (int i = 0; i < wheelSlots.Count; i++)
        {
            wheelSlots[i].Image.sprite = currentZone.Slots[i].Icon;
            if(!currentZone.Slots[i].IsBomb)
                wheelSlots[i].Text.text = "X" + currentZone.Slots[i].Multiplier.ToString();
            else
            {
                wheelSlots[i].Text.text = "";
            }
        }

        if (ZoneManager.Instance.ZoneCount % 30 == 0)
            wheelImage.sprite = goldenWheel;
        else if(ZoneManager.Instance.ZoneCount % 5 == 0 || ZoneManager.Instance.ZoneCount == 1)
            wheelImage.sprite = silverWheel;
        else
            wheelImage.sprite = defaultWheel;
    }
    
    public void SpinWheel()
    {
        targetSlot = GetRandomSlotIndex();
        
        float targetAngle = 360f / probabilities.Length * targetSlot;
        float finalAngle = (360) + targetAngle;

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
        if(currentZone.Slots[targetSlot].IsBomb)
            Debug.Log("Bomb");
        else
            ZoneManager.Instance.NextZone();
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
