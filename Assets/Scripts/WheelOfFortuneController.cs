using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class WheelOfFortuneController : MonoBehaviour
{
    public RectTransform wheelTransform;   // Reference to the wheel's RectTransform
    private float[] probabilities = new float[] { 0.1f, 0.15f, 0.2f, 0.1f, 0.15f, 0.1f, 0.1f, 0.1f }; // Hard-coded probabilities
    private float[] cumulativeProbabilities; // For calculating random slice based on probabilities
    public float spinDuration = 4f;        // Duration for the initial fast spin
    private int targetSlice;               // Selected slice to land on
    public Button spinButton;

    void Awake()
    {
        spinButton.onClick.AddListener(SpinWheel);
    }
    void Start()
    {
        // Calculate cumulative probabilities for slice selection
        cumulativeProbabilities = new float[probabilities.Length];
        cumulativeProbabilities[0] = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
        }
    }

    public void SpinWheel()
    {
        // Choose the slice based on probabilities
        targetSlice = GetRandomSliceIndex();
        
        // Set the final angle based on the chosen slice
        float targetAngle = 360f / probabilities.Length * targetSlice;
        float finalAngle = (360 * 3) + targetAngle; // Three full rotations plus target angle

        // Start the initial fast spin for `spinDuration` seconds
        wheelTransform.DORotate(new Vector3(0, 0, -(360 * 4)), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // After the initial spin, smoothly rotate to the final target slice
                wheelTransform.DORotate(new Vector3(0, 0, -finalAngle), 4f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutCubic);
            });
    }

    private int GetRandomSliceIndex()
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
