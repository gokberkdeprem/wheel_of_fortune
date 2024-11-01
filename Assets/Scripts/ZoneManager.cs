using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _zoneTextPrefab;
    [SerializeField] private Transform _zoneTextContainer;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 60 ; i++)
        {
            var zoneText = Instantiate(_zoneTextPrefab, _zoneTextContainer);
            zoneText.text = i.ToString();
            if(i == 1) zoneText.color = Color.green;
            if(i % 5 == 0) zoneText.color = Color.green;
            if(i % 30 == 0) zoneText.color = Color.yellow;
        }
    }
}
