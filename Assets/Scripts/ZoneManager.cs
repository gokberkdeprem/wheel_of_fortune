using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;
    public int ZoneCount = 1;
    public List<Zone> Zones = new List<Zone>();
    [SerializeField] ZoneAnimationController ZoneAnimationController;
    [SerializeField] WheelOfFortuneController WheelOfFortuneController;

    private void Awake()
    {
        Instance = this;
    }

    public void NextZone()
    {
        ZoneCount++;
        ZoneAnimationController.Play();
        WheelOfFortuneController.InitWheel();
    }

    public Zone GetCurrentZone()
    {
        return Zones[ZoneCount - 1];
    }
}
