using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneAnimationController : MonoBehaviour
{
    [SerializeField] private TMP_Text _zoneTextPrefab;
    [SerializeField] private RectTransform _zoneTextRectTransform;
    
    [SerializeField] private Image _currentZoneImage;
    [SerializeField] private RectTransform _currentZoneImageRectTransform;
    
    [SerializeField] private Sprite _defaultZoneSprite;
    [SerializeField] private Sprite _silverZoneSprite;
    [SerializeField] private Sprite _goldZoneSprite;
    private float _currentZoneImageWidth;

    void Start()
    {
        for (int i = 1; i <= 60 ; i++)
        {
            _currentZoneImageWidth = _currentZoneImageRectTransform.sizeDelta.x;
            _currentZoneImage.sprite = _silverZoneSprite;
            var zoneText = Instantiate(_zoneTextPrefab, _zoneTextRectTransform);
            zoneText.text = i.ToString();
            if(i == 1) zoneText.color = Color.green;
            if(i % 5 == 0) zoneText.color = Color.green;
            if(i % 30 == 0) zoneText.color = Color.yellow;
        }
    }

    public void Play()
    {
        _currentZoneImageRectTransform.pivot = new Vector2(0, 0.5f);
        _currentZoneImageRectTransform.DOScaleX(0, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (ZoneManager.Instance.ZoneCount % 30 == 0)
            {
                _currentZoneImage.sprite = _goldZoneSprite;
            }
            else if (ZoneManager.Instance.ZoneCount % 5 == 0)
            {
                _currentZoneImage.sprite = _silverZoneSprite;
            }
            else
            {
                _currentZoneImage.sprite = _defaultZoneSprite;
            }
            
            _currentZoneImageRectTransform.pivot = new Vector2(1, 0.5f);
            _currentZoneImageRectTransform.anchoredPosition += new Vector2(_currentZoneImageWidth,0);
            _currentZoneImageRectTransform.DOScaleX(1, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _currentZoneImageRectTransform.pivot = new Vector2(0, 0.5f);
                _currentZoneImageRectTransform.anchoredPosition += new Vector2(-_currentZoneImageWidth,0);
            });
            
        });
        
        var nextPosition = _zoneTextRectTransform.anchoredPosition.x - 84.5f;
        _zoneTextRectTransform.DOAnchorPosX(nextPosition, 0.2f).SetEase(Ease.Linear);
    }
}
