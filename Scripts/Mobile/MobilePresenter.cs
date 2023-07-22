using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Main.Mobile
{
    public class MobilePresenter : IInitializable, IDisposable
    {
        MobileModel mobileModel;
        MatchingView matchingView;
        MobileUIView uiView;
        InputObserver inputObserver;
        Bottle bottle;
        
        GamePhotonView gamePhotonView;
        List<IDisposable> disposables = new List<IDisposable>();

        public MobilePresenter(MobileModel mobileModel, 
            MatchingView matchingView, 
            MobileUIView uiView,
            InputObserver inputObserver,
            Bottle bottle)
        {
            this.mobileModel = mobileModel;
            this.matchingView = matchingView;
            this.uiView = uiView;
            this.inputObserver = inputObserver;
            this.bottle = bottle;
        }

        public void Initialize()
        {
            inputObserver.Setup();
            
            Bind();
            SetEvents();

            if (!inputObserver.hasAccelerometer)
            {
                // ご使用の端末は加速度センサーに対応していません
                Debug.Log("ご使用の端末は加速度センサーに対応していません");
            }
        }

        void Bind()
        {
            // モデルのステートによる処理
            disposables.Add(mobileModel.CurrentState.Subscribe(state =>
            {
                // ダイアログ切り替え
                uiView.ChangeDialog(state).Forget();
                switch (state)
                {
                    case MobileModel.State.ConnectToMaster:
                        matchingView.Connect();
                        break;
                    case MobileModel.State.InGame:
                        bottle.StartWave();
                        break;
                }

                Debug.Log("current state: " + state);
            }));
            // 端末を振った時
            disposables.Add(mobileModel.OnShake.Subscribe(_ => bottle.Shake()));
        }

        void SetEvents()
        {
            // 初回ジャイロ取得確認
            disposables.Add(inputObserver.Acceleration.Where(v => v != Vector3.zero)
                .First()
                .Subscribe(_ => mobileModel.OnGetGyroInfo()));
            // ジャイロ入力
            disposables.Add(inputObserver.Acceleration.Subscribe(mobileModel.OnAccelerationChanged));
            // マスターサーバー接続時
            disposables.Add(
                matchingView.OnConnectedToMasterAsObservable.Subscribe(_ => mobileModel.OnConnectedToMaster()));
            // ルーム接続
            disposables.Add(uiView.OnTryConnectToRoom.Subscribe(v =>
            {
                mobileModel.OnTryConnectToRoom();
                matchingView.JoinRoom(v);
            }));
            // ルーム接続失敗時
            disposables.Add(matchingView.OnJoinRoomFailedAsObservable.Subscribe(_ =>
            {
                mobileModel.OnJoinRoomFailed();
                uiView.ShowJoinFailureMessage();
            }));
            // ルーム接続成功時
            disposables.Add(matchingView.OnJoinedRoomAsObservable.Subscribe(_ => SetupGame().Forget()));
            // サーバー切断時
            disposables.Add(matchingView.OnDisconnectedAsObservable.Subscribe(_ => mobileModel.OnDisconnected()));
            // サーバー再接続
            disposables.Add(uiView.OnBack.Subscribe(_ => mobileModel.ReconnectToServer()));
        }

        async UniTask SetupGame()
        {
            var go = PhotonNetwork.Instantiate(GamePhotonView.ObjectName, Vector3.zero, Quaternion.identity);
            gamePhotonView = go.GetComponent<GamePhotonView>();
            
            SetupPhotonView();
            
            // ゲーム開始
            mobileModel.StartGame();
        }

        void SetupPhotonView()
        {
            // 端末を振った時
            disposables.Add(mobileModel.OnShake.Subscribe(_ => gamePhotonView.Shake()));
        }

        public void Dispose()
        {
            disposables.ForEach(d => d.Dispose());
        }
    }
}
