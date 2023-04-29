using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PulseTween : MonoBehaviour
{
    [SerializeField] private float scale = 2;    
    [SerializeField] private float duration = 1;

    
    void Start()
    {
        transform.DOScale(scale, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
