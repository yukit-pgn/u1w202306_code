using System;
using Cysharp.Threading.Tasks;
using Main.Data;
using Main.Service;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Main.Title
{
    public class TitleModel
    {
        int createRoomNameCount = 0;
        const int CreateRoomNameMaxCount = 5; // 5回ルーム名を作成しても作成できなかったら諦める
        
        ReactiveProperty<State> currentState = new ReactiveProperty<State>(State.Start);
        public IReadOnlyReactiveProperty<State> CurrentState => currentState;
        
        Subject<string> onCreateRoomName = new Subject<string>();
        public IObservable<string> OnCreateRoomName => onCreateRoomName;

        public string RoomName { get; private set; }

        public TitleModel()
        {
            PlayBGM().Forget();
        }

        async UniTask PlayBGM()
        {
            await SoundService.Instance.SetBGM(BGMPath.Title);
            SoundService.Instance.PlayBGM();
        }

        /// <summary>
        /// 画面クリック待ち
        /// </summary>
        public async UniTask WaitClick()
        {
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            
            currentState.Value = State.ConnectToMaster;
            
            SoundService.Instance.PlaySE(SEPath.Button).Forget();
        }
        
        public void CreateRoomName()
        {
            if (createRoomNameCount >= CreateRoomNameMaxCount)
            {
                // 接続数がいっぱいの可能性が高いので諦める
                currentState.Value = State.CannotCreateRoom;
                return;
            }
            
            createRoomNameCount++;
            RoomName = Random.Range(100_000, 1_000_000).ToString(); // 6桁の数字
            onCreateRoomName.OnNext(RoomName);
        }

        public void OnCreateRoomSuccess()
        {
            currentState.Value = State.WaitMatching;
        }

        public async UniTask OnMatch()
        {
            SoundService.Instance.StopBGM(1f);
            await SoundService.Instance.PlaySE(SEPath.Success);

            // SEが終わるまで待つ
            await UniTask.Delay(1000);
            
            // ゲームシーンに遷移
            SceneService.ChangeScene(SceneName.GameScene);
        }

        public enum State
        {
            Start,
            ConnectToMaster,
            WaitMatching,
            CannotCreateRoom,
        }
    }
}
