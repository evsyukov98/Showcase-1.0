using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSqueezeTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Transform image;
        [SerializeField, Range(0f, 1f)] private float squeezedScale = 0.9f;
        [SerializeField, Range(0f, 0.4f)] private float squeezeAnimationDuration = 0.15f;
        [SerializeField, Range(0f, 0.4f)] private float unSqueezeAnimationDuration = 0.15f;

        public float SqueezeAnimationDuration => squeezeAnimationDuration;
        public float UnSqueezeAnimationDuration => unSqueezeAnimationDuration;

        private TweenerCore<float, float, FloatOptions> _squeezeTween;

        private void Awake()
        {
            Init();
            if (image == null)
                image = transform;
        }

        private void OnDestroy()
        {
            _squeezeTween?.Kill();
            _squeezeTween = null;
        }

        public void ResetState()
        {
            _squeezeTween?.Kill();
            
            _squeezeTween = DOTween.To(
                () => image.localScale.x,
                scale => image.localScale = Vector3.one * scale,
                1f,
                unSqueezeAnimationDuration).SetUpdate(true);
            _squeezeTween.Play();
        }

        protected virtual void Init() { }

        protected virtual bool CanSqueeze() => true;

        private void Squeeze()
        {
            if (!CanSqueeze())
            {
                return;
            }

            _squeezeTween?.Kill(true);
            _squeezeTween = null;

            _squeezeTween = DOTween.To(
                () => image.localScale.x,
                scale => image.localScale = Vector3.one * scale,
                squeezedScale,
                squeezeAnimationDuration).SetUpdate(true);
            _squeezeTween.Play();
        }

        private void UnSqueeze()
        {
            if (!CanSqueeze())
            {
                return;
            }

            _squeezeTween?.Kill(true);
            _squeezeTween = null;

            _squeezeTween = DOTween.To(
                () => image.localScale.x,
                scale => image.localScale = Vector3.one * scale,
                1f,
                unSqueezeAnimationDuration).SetUpdate(true);
            _squeezeTween.Play();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => Squeeze();
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => UnSqueeze();
    }
