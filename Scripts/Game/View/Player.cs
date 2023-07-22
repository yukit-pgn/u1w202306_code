using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Main.Game
{
    public class Player : MonoBehaviour
    {
        static readonly int Fly = Animator.StringToHash("Fly");
        
        [SerializeField] Rigidbody2D rigidbody2D;
        [SerializeField] Animator animator;
        [SerializeField] ParticleSystem splash;
        [SerializeField] Transform explosion;

        Vector3 hitPosition;

        Subject<Unit> onHitBarrier = new Subject<Unit>();
        /// <summary>
        /// 障害物にぶつかった時
        /// </summary>
        public IObservable<Unit> OnHitBarrier => onHitBarrier;

        void Reset()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Barrier"))
            {
                hitPosition = other.contacts[0].point;
                onHitBarrier.OnNext(Unit.Default);
            }
        }

        public void SetVelocity(Vector2 velocity)
        {
            rigidbody2D.velocity = velocity;
        }

        public void FlyAnimation()
        {
            animator.SetTrigger(Fly);
            splash.Play();
        }

        /// <summary>
        /// 障害物に当たってゲームオーバーになった時
        /// </summary>
        public void DropOut()
        {
            splash.Stop();
            
            rigidbody2D.simulated = false;
            transform.DOMoveY(-10f, 2f).SetRelative().SetEase(Ease.Linear);
            transform.DORotate(Vector3.forward * 720, 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            
            // 爆発エフェクト
            explosion.position = hitPosition;
            explosion.gameObject.SetActive(true);
        }
    }
}
