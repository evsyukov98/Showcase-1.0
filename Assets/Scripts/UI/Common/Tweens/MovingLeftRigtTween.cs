using DG.Tweening;
using UnityEngine;

public class MovingLeftRigtTween : MonoBehaviour
{
    [SerializeField] private Vector3 endValue;
    [SerializeField] private float duration;

    private void Start()
    {
        transform.DOLocalMove(endValue, duration, true).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }
}
