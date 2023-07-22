using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Mobile
{
    public class Bottle : MonoBehaviour
    {
        static readonly int Wave = Animator.StringToHash("Wave");
        
        [SerializeField] Animator surface;
        [SerializeField] ParticleSystem splash;

        Tween shakeTween;
        Vector3 initialPosition;
        Quaternion initialRotation;

        void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        public void StartWave()
        {
            surface.SetTrigger(Wave);
        }

        public void Shake()
        {
            // リセット
            shakeTween?.Kill();
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            
            var seq = DOTween.Sequence();
            seq.Append(transform.DOShakePosition(0.5f, new Vector3(1, 1, 0)));
            seq.Join(transform.DOShakeRotation(0.5f, new Vector3(0, 0, 20)));
            shakeTween = seq;
            
            splash.Play();
        }
    }
}
