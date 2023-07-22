using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer.Unity;

namespace Main.Game
{
    public class GamePresenter : IStartable, ITickable, IDisposable
    {
        GameModel gameModel;
        Player player;
        FieldManager fieldManager;
        GameOverDialog gameOverDialog;
        GameUIView uiView;
        
        GamePhotonView gamePhotonView;
        List<IDisposable> disposables = new List<IDisposable>();

        public GamePresenter(GameModel gameModel, 
            Player player, 
            FieldManager fieldManager,
            GameOverDialog gameOverDialog,
            GameUIView uiView)
        {
            this.gameModel = gameModel;
            this.player = player;
            this.fieldManager = fieldManager;
            this.gameOverDialog = gameOverDialog;
            this.uiView = uiView;
        }

        public void Start()
        {
            // Awakeより後で実行したいのでStartで
            StartAsync().Forget();
        }
        
        async UniTask StartAsync()
        {
            Bind();
            SetEvents();
            
            // スマホの接続を確認
            if (gameModel.CheckFailedConnection()) return;
            
            await FindGamePhotonViewAsync();
            
            SetupPhotonView();
            
            gameModel.SetReady();
        }
        
        async UniTask FindGamePhotonViewAsync()
        {
            // モバイル側にGamePhotonViewを生成してもらうのを待つ
            while (true)
            {
                if (GamePhotonView.Instance != null)
                {
                    gamePhotonView = GamePhotonView.Instance;
                    break;
                }

                await UniTask.Delay(100);
            }
        }

        void Bind()
        {
            // モデルのステートによる処理
            disposables.Add(gameModel.CurrentState.Subscribe(state =>
            {
                uiView.ChangeDialog(state).Forget();
                switch (state)
                {
                    case GameModel.State.GameOver:
                        player.DropOut();
                        gameOverDialog.ShowScore(gameModel.FlightDistance.Value).Forget();
                        break;
                    case GameModel.State.Disconnected:
                        break;
                }
            }));
            // プレイヤーの移動
            disposables.Add(gameModel.PlayerVelocity.Subscribe(player.SetVelocity));
            // 飛ぶアニメーション
            disposables.Add(gameModel.OnFly.Subscribe(_ => player.FlyAnimation()));
            // ステージスクロール
            disposables.Add(gameModel.FlightDistance.Pairwise()
                .Subscribe(pair => fieldManager.Slide(pair.Current - pair.Previous)));
            // スコア表示
            disposables.Add(gameModel.FlightDistance.Subscribe(uiView.UpdateScore));
            // リスタートボタン
            disposables.Add(gameOverDialog.OnRestart.Subscribe(_ => gameModel.Restart()));
            // ツイートボタン
            disposables.Add(gameOverDialog.OnTweet.Subscribe(_ => gameModel.Tweet()));
            // タイトルに戻るボタン
            disposables.Add(uiView.OnBack.Subscribe(_ => gameModel.BackToTitle()));
        }

        void SetEvents()
        {
            disposables.Add(player.OnHitBarrier.Subscribe(_ => gameModel.HitBarrier()));
        }

        void SetupPhotonView()
        {
            disposables.Add(gamePhotonView.OnShake.Subscribe(_ => gameModel.Shake()));
            disposables.Add(gamePhotonView.OnDestroyAsObservable().Subscribe(_ => gameModel.OnDestroyedPhotonView()));
        }

        public void Tick()
        {
            gameModel.Tick();
        }

        public void Dispose()
        {
            disposables.ForEach(d => d.Dispose());
        }
    }
}
