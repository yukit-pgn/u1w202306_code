using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mobile
{
    public class MobileUIView : MonoBehaviour
    {
        [SerializeField] KeyPad roomNameKeyPad;
        [SerializeField] TextMeshProUGUI joinFailureText;
        [SerializeField] Button backButton;
        [SerializeField] List<DialogData> dialogDataList;
        
        Dialog currentDialog;
        
        public IObservable<Unit> OnBack => backButton.OnClickAsObservable();

        /// <summary>
        /// ルームID入力完了時
        /// </summary>
        public IObservable<string> OnTryConnectToRoom => roomNameKeyPad.OnEnterID;

        void Awake()
        {
            joinFailureText.gameObject.SetActive(false);
        }
        
        public async UniTask ChangeDialog(MobileModel.State state)
        {
            if (currentDialog != null)
            {
                await currentDialog.Hide();
            }

            currentDialog = dialogDataList.FirstOrDefault(data => data.state == state)?.dialog;
            
            if (currentDialog != null)
            {
                await currentDialog.Show();
            }
        }

        public void ShowJoinFailureMessage()
        {
            joinFailureText.gameObject.SetActive(true);
        }

        [Serializable]
        class DialogData
        {
            public MobileModel.State state;
            public Dialog dialog;
        }
    }
}
