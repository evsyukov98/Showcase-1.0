using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.Tweens
{
    public class FadePulseTween : MonoBehaviour
    {
        [SerializeField] private float fadeValue = 1;    
        [SerializeField] private float duration = 1;
        [SerializeField] private float startDelay = 0.5f;

        private Image _image;
    
        void Start()
        {
            _image = GetComponent<Image>();
            _image.DOFade(fadeValue, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetDelay(startDelay);
        }
    }
}
