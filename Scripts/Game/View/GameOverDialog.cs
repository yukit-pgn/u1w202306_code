using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game
{
    public class GameOverDialog : Dialog
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] Button restartButton;
        [SerializeField] Button tweetButton;
        
        /// <summary>
        /// リスタートボタン押下時
        /// </summary>
        public IObservable<Unit> OnRestart => restartButton.OnClickAsObservable();
        
        /// <summary>
        /// ツイートボタン押下時
        /// </summary>
        public IObservable<Unit> OnTweet => tweetButton.OnClickAsObservable();

        protected override void Awake()
        {
            base.Awake();
            
            // リスタートボタンを押したらゲームオーバーダイアログを閉じる
            OnRestart.Subscribe(_ => Hide().Forget()).AddTo(this);
        }

        public async UniTask ShowScore(float score)
        {
            scoreText.text = $"記録：{score:F3}m";
            await UniTask.Delay(1000);
            await Show();
        }
    }
}
