using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Dialog : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] RectTransform body;
        [SerializeField] bool isInitialShow = false;

        void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            body = transform.Find("Body") as RectTransform;
        }

        protected virtual void Awake()
        {
            if (!isInitialShow)
            {
                HideImmediately();
            }
        }

        public async UniTask Show(float duration = 0.2f)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, duration);
            body.DOScale(1f, duration);
            
            await UniTask.Delay((int)(duration * 1000));
        }
        
        public async UniTask Hide(float duration = 0.2f)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, duration);
            body.DOScale(0f, duration);
            
            await UniTask.Delay((int)(duration * 1000));
        }
        
        public void HideImmediately()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            body.localScale = Vector3.zero;
        }
    }
}
