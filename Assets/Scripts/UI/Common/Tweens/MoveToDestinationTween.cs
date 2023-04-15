using DG.Tweening;
using UnityEngine;

public class MoveToDestinationTween : MonoBehaviour
{
    [SerializeField] private Vector3 endValue;
    [SerializeField] private float duration;
    
    private void Start()
    {
        transform.DOMove(endValue, duration, true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endValue);
    }
}
