using DG.Tweening;
using UnityEngine;

public class MovingLeftRigtTween : MonoBehaviour
{
    [SerializeField] private Vector3 endValue;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease = Ease.Flash;
    

    private void Start()
    {
        transform.DOLocalMove(endValue, duration, false).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }
}
