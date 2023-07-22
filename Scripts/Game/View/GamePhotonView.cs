using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace Main
{
    public class GamePhotonView : MonoBehaviourPunCallbacks
    {
        public static readonly string ObjectName = "GamePhotonView";
        
        public static GamePhotonView Instance {get; private set;}
        
        Subject<Unit> onShake = new Subject<Unit>();
        public IObservable<Unit> OnShake => onShake;

        void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Shake()
        {
            photonView.RPC(nameof(ShakeRPC), RpcTarget.Others);
        }
        
        [PunRPC]
        void ShakeRPC()
        {
            onShake.OnNext(Unit.Default);
            Debug.Log("ShakeRPC");
        }
    }
}
