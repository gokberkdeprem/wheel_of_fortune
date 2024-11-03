using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;
    public int ZoneCount = 1;
    public List<Zone> Zones = new List<Zone>();
    [SerializeField] ZoneAnimationController ZoneAnimationController;
    [SerializeField] WheelOfFortuneController WheelOfFortuneController;
    public Button retryButton;
    public Button exitButton;

    private void OnValidate()
    {
        exitButton = GameObject.Find("exit_button").GetComponent<Button>();
        retryButton = GameObject.Find("retry_button").GetComponent<Button>();
        retryButton.onClick.AddListener(Retry);
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    private void Awake()
    {
        retryButton.gameObject.SetActive(false);
        Instance = this;
    }

    public void NextZone()
    {
        ZoneCount++;
        ZoneAnimationController.Play();
        WheelOfFortuneController.InitWheel();
    }

    public Zone CurrentZone => Zones[ZoneCount - 1];

    public void ShowRetryButton()
    {
        retryButton.gameObject.SetActive(true);
    }
}
