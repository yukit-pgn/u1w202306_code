using System;
using Cysharp.Threading.Tasks;
using Main.Data;
using Main.Mobile;
using Main.Service;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using unityroom.Api;
using SoundService = Main.Service.SoundService;

namespace Main.Game
{
    public class GameModel
    {
        const float gravity = 9.8f;
        const float flightSpeed = 3f;

        ReactiveProperty<State> currentState = new ReactiveProperty<State>(State.Setup);
        public IReadOnlyReactiveProperty<State> CurrentState => currentState;

        Vector2ReactiveProperty playerVelocity = new Vector2ReactiveProperty();
        public IReadOnlyReactiveProperty<Vector2> PlayerVelocity => playerVelocity;
        
        FloatReactiveProperty flightDistance = new FloatReactiveProperty();
        public IReadOnlyReactiveProperty<float> FlightDistance => flightDistance;
        
        Subject<Unit> onFly = new Subject<Unit>();
        public IObservable<Unit> OnFly => onFly;

        public GameModel()
        {
            SoundService.Instance.SetBGM(BGMPath.InGameIntro, BGMPath.InGameLoop).Forget();
        }

        public void SetReady()
        {
            currentState.Value = State.Ready;
        }

        public bool CheckFailedConnection()
        {
            // スマホの接続を確認
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                currentState.Value = State.Disconnected;
                return true;
            }

            return false;
        }

        public void OnDestroyedPhotonView()
        {
            if (currentState.Value == State.GameOver) return;
            
            currentState.Value = State.Disconnected;
        }

        public void Shake()
        {
            if (currentState.Value == State.Ready)
            {
                // ゲーム開始
                currentState.Value = State.Playing;
                
                // BGM
                SoundService.Instance.PlayBGM();
            }
            
            if (currentState.Value != State.Playing) return;
            
            // 浮上する
            playerVelocity.Value = Vector2.up * 4f;
            onFly.OnNext(Unit.Default);

            SoundService.Instance.PlaySE(SEPath.Jump).Forget();
        }

        public void Tick()
        {
            switch (currentState.Value)
            {
                case State.Setup:
                    break;
                case State.Ready:
                    break;
                case State.Playing:
                    Fly();
                    break;
                case State.GameOver:
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        Restart();
                    }
                    break;
            }
            
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Shake();
            }
#endif
        }

        void Fly()
        {
            playerVelocity.Value += Vector2.down * gravity * Time.deltaTime;
            
            flightDistance.Value += flightSpeed * Time.deltaTime;
        }

        public void HitBarrier()
        {
            if (currentState.Value != State.Playing)
            {
                return;
            }
            
            currentState.Value = State.GameOver;
            SendScore();
            
            SoundService.Instance.StopBGM();
            SoundService.Instance.PlaySE(SEPath.Explosion).Forget();
        }
        
        /// <summary>
        /// ランキング送信
        /// </summary>
        void SendScore()
        {
            UnityroomApiClient.Instance.SendScore(1, flightDistance.Value, ScoreboardWriteMode.HighScoreDesc);
        }

        /// <summary>
        /// ツイート
        /// </summary>
        public void Tweet()
        {
            naichilab.UnityRoomTweet.Tweet("furuppy_penguin", $"記録：{flightDistance.Value:F3}m 飛んだ！！", "unityroom", "unity1week", "フルッピー・ペンギン");
        }

        public void Restart()
        {
            // シーン再読み込み
            SceneService.ChangeScene(SceneName.GameScene, 0.5f);
        }

        /// <summary>
        /// サーバーから切断してタイトルシーンに戻る
        /// </summary>
        public void BackToTitle()
        {
            PhotonNetwork.Disconnect();
            SceneService.ChangeScene(SceneName.TitleScene, 0.5f);
        }
        
        public enum State
        {
            Setup,
            Ready,
            Playing,
            GameOver,
            Disconnected,
        }
    }
}
