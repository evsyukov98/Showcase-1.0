using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowController : MonoBehaviour
    {
        [BoxGroup("Fade"),SerializeField] private bool isAlphaFade;

        [ShowIf(nameof(isAlphaFade))] 
        [BoxGroup("Fade"),SerializeField] private float showFadeDuration = 0.5f;
        [ShowIf(nameof(isAlphaFade))] 
        [BoxGroup("Fade"),SerializeField] private float hideFadeDuration = 0f;
        
        public event Action WindowShowEvent; 
        public event Action WindowHideEvent;
        
        private bool _isActiveWindow;
        private CanvasGroup _canvasGroup;

        public bool IsActiveWindow => _isActiveWindow;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void ShowWindow()
        {
            _isActiveWindow = true;
            gameObject.SetActive(true);
            WindowShowEvent?.Invoke();

            if (isAlphaFade)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.DOFade(1, showFadeDuration);
            }
            else
            {
                _canvasGroup.alpha = 1;
            }
        }

        public virtual void HideWindow()
        {
            _isActiveWindow = false;
            WindowHideEvent?.Invoke();
            
            if (isAlphaFade)
            {
                _canvasGroup.
                    DOFade(0, hideFadeDuration).
                    OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void ForceHideWindow()
        {
            _isActiveWindow = false;
            WindowHideEvent?.Invoke();

            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        public void ForceShowWindow()
        {
            _isActiveWindow = true;
            gameObject.SetActive(true);
            WindowShowEvent?.Invoke();
            _canvasGroup.alpha = 1;
        }
    }
}
