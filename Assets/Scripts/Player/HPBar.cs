using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] RectTransform _barRect;
    [SerializeField] RectMask2D _mask;
    [SerializeField] TMP_Text _hpText;

    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] bool isEnemy;

    private float _maxRightMask;
    private float _initialRightMask;
    private float _maxLeftMask;
    private float _initialLeftMask;

    private void Start(){
        _maxRightMask = _barRect.rect.width - _mask.padding.x - _mask.padding.z;
        _maxLeftMask = _barRect.rect.width - _mask.padding.x - _mask.padding.z;
        _hpText.SetText($"{currentHealth}/{maxHealth}");
        _initialRightMask = _mask.padding.z;
        _initialLeftMask = _mask.padding.x;
    }

    public void SetHealth(int newVal){
        var padding = _mask.padding;
        if(isEnemy){ //changes left side
            var targetWidth = _maxLeftMask * newVal / maxHealth;
            var newLeftMask = _maxLeftMask + _initialLeftMask - targetWidth;
            padding.x = newLeftMask;
        }
        else{ // changes right side
             var targetWidth = _maxRightMask * newVal / maxHealth;
             var newRightMask = _maxRightMask + _initialRightMask - targetWidth;
             padding.z = newRightMask;
        }
            
        _mask.padding = padding;
        _hpText.SetText($"{newVal}/{maxHealth}");

    }
    public void SetMaxHealth(int newVal){
        maxHealth = newVal;
        if(isEnemy){
            currentHealth = newVal;
        }
        _hpText.SetText($"{currentHealth}/{newVal}");
    }
}
