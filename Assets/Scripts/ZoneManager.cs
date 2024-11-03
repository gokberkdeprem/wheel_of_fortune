using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    [SerializeField] private Transform prizeTransform;
    [SerializeField] Transform resultContent;
    [SerializeField] GameObject resultGrid;
    [SerializeField] private Canvas _canvas;
    public Button retryButton;
    public Button exitButton;
    public Button collectButton;

    private void OnValidate()
    {
        exitButton = GameObject.Find("exit_button").GetComponent<Button>();
        exitButton.onClick.AddListener(ExitEvent);
        retryButton = GameObject.Find("retry_button").GetComponent<Button>();
        retryButton.onClick.AddListener(Retry);
        collectButton = GameObject.Find("collect_all").GetComponent<Button>();
        collectButton.onClick.AddListener(Retry);
        
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    private void Awake()
    {
        resultGrid.SetActive(false);
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

    public void ExitEvent()
    {
        if(prizeTransform.childCount == 0)
            Retry();
        else
        {
            foreach (Transform child in _canvas.transform)
            {
                if (child != gameObject.transform || child != resultGrid.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            
            
            resultGrid.SetActive(true);
            foreach (Transform child in prizeTransform)
            {
                Instantiate(child.gameObject, resultContent.transform);
            }
        }
    }
}
