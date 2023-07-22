using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Main.Title
{
    public class TitlePresenter : IStartable, IDisposable
    {
        TitleModel titleModel;
        MatchingView matchingView;
        TitleUIView uiView;

        List<IDisposable> disposables = new List<IDisposable>();

        public TitlePresenter(TitleModel titleModel, MatchingView matchingView, TitleUIView uiView)
        {
            this.titleModel = titleModel;
            this.matchingView = matchingView;
            this.uiView = uiView;
        }
        
        public void Start()
        {
            Bind();
            SetEvents();
            
            titleModel.WaitClick().Forget();
        }

        void Bind()
        {
            // モデルのステートによる処理
            disposables.Add(titleModel.CurrentState.Subscribe(state =>
            {
                Debug.Log($"current state: {state}");
                uiView.ChangeDialog(state).Forget();
                switch (state)
                {
                    case TitleModel.State.ConnectToMaster:
                        matchingView.Connect();
                        break;
                    case TitleModel.State.WaitMatching:
                        uiView.ShowRoomName(titleModel.RoomName);
                        matchingView.WaitMatching().Forget();
                        break;
                }
            }));
            // ルームを作成
            disposables.Add(titleModel.OnCreateRoomName.Subscribe(matchingView.CreateRoom));
        }

        void SetEvents()
        {
            // マスターサーバーに接続した時
            disposables.Add(matchingView.OnConnectedToMasterAsObservable.Subscribe(_ => titleModel.CreateRoomName()));
            // ルーム作成に失敗した時
            disposables.Add(matchingView.OnJoinRoomFailedAsObservable.Subscribe(_ => titleModel.CreateRoomName()));
            // ルーム作成に成功した時
            disposables.Add(matchingView.OnJoinedRoomAsObservable.Subscribe(_ =>
            {
                titleModel.OnCreateRoomSuccess();
            }));
            // マッチング成功時
            disposables.Add(matchingView.OnMatchAsObservable.Subscribe(_ => titleModel.OnMatch()));
        }
        
        public void Dispose()
        {
            disposables.ForEach(disposable => disposable.Dispose());
        }
    }
}
