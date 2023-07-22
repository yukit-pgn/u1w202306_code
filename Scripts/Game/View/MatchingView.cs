using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;

namespace Main
{
    public class MatchingView : MonoBehaviourPunCallbacks
    {
        Subject<Unit> onConnectedToMaster = new Subject<Unit>();
        public IObservable<Unit> OnConnectedToMasterAsObservable => onConnectedToMaster;
        
        Subject<Unit> onJoinedRoom = new Subject<Unit>();
        public IObservable<Unit> OnJoinedRoomAsObservable => onJoinedRoom;
        
        Subject<Unit> onJoinRoomFailed = new Subject<Unit>();
        public IObservable<Unit> OnJoinRoomFailedAsObservable => onJoinRoomFailed;

        Subject<Unit> onMatch = new Subject<Unit>();
        public IObservable<Unit> OnMatchAsObservable => onMatch;
        
        Subject<Unit> onDisconnected = new Subject<Unit>();
        public IObservable<Unit> OnDisconnectedAsObservable => onDisconnected;

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public override void OnConnectedToMaster()
        {
            onConnectedToMaster.OnNext(Unit.Default);
        }

        public void CreateRoom(string roomName)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            
            PhotonNetwork.CreateRoom(roomName, roomOptions: roomOptions);
        }
        
        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        
        /// <summary>
        /// ルーム入室時
        /// </summary>
        public override void OnJoinedRoom()
        {
            onJoinedRoom.OnNext(Unit.Default);
        }
        
        /// <summary>
        /// ルーム入室失敗時
        /// </summary>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            onJoinRoomFailed.OnNext(Unit.Default);
        }

        public async UniTask WaitMatching()
        {
            while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                await UniTask.Delay(100);
            }
            
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
            onMatch.OnNext(Unit.Default);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            onDisconnected.OnNext(Unit.Default);
        }
    }
}
