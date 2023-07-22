using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Main.Data;
using Main.Service;
using UniRx;
using UnityEngine;

namespace Main.Mobile
{
    public class MobileModel
    {
        static readonly string RoomNameKey = "id";

        bool isAccelerationUp;
        
        ReactiveProperty<State> currentState = new ReactiveProperty<State>(State.SetupGyro);
        public IReadOnlyReactiveProperty<State> CurrentState => currentState;
        
        Subject<Unit> onShake = new Subject<Unit>();
        public IObservable<Unit> OnShake => onShake;

        public void OnGetGyroInfo()
        {
            currentState.Value = State.ConnectToMaster;
        }
        
        public void OnConnectedToMaster()
        {
            currentState.Value = State.InputRoomName;
        }
        
        public void OnTryConnectToRoom()
        {
            currentState.Value = State.TryConnectToRoom;
        }
        
        public void OnJoinRoomFailed()
        {
            currentState.Value = State.InputRoomName;
        }
        
        public void StartGame()
        {
            currentState.Value = State.Ready;
        }
        
        public void OnAccelerationChanged(Vector3 acceleration)
        {
            if (!(currentState.Value == State.InGame || currentState.Value == State.Ready)) return;
            
            if (acceleration.y > 0.5f && !isAccelerationUp)
            {
                isAccelerationUp = true;
                SoundService.Instance.PlaySE(SEPath.ShakeUp).Forget();
            }
            else if (acceleration.y < -0.5f && isAccelerationUp)
            {
                onShake.OnNext(Unit.Default);
                isAccelerationUp = false;
                SoundService.Instance.PlaySE(SEPath.ShakeDown).Forget();
                
                // 振ってゲームスタート
                if (currentState.Value == State.Ready)
                {
                    currentState.Value = State.InGame;
                }
            }
        }
        
        public void OnDisconnected()
        {
            currentState.Value = State.Disconnected;
        }
        
        public void ReconnectToServer()
        {
            currentState.Value = State.ConnectToMaster;
        }

        public enum State
        {
            SetupGyro,
            ConnectToMaster,
            InputRoomName,
            TryConnectToRoom,
            Ready,
            InGame,
            Disconnected,
        }
    }
}
