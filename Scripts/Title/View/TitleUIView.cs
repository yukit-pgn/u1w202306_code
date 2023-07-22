using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data;
using Main.Service;
using TMPro;
using UnityEngine;

namespace Main.Title
{
    public class TitleUIView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI roomNameText;
        [SerializeField] TextMeshProUGUI clickText;
        [SerializeField] List<DialogData> dialogDataList;
        
        Dialog currentDialog;

        void Awake()
        {
            // クリックテキストの点滅
            var seq = DOTween.Sequence();
            seq.Append(clickText.DOFade(0f, 1f).SetEase(Ease.Linear));
            seq.Append(clickText.DOFade(1f, 1f).SetEase(Ease.Linear));
            seq.SetLoops(-1);
        }

        public async UniTask ChangeDialog(TitleModel.State state)
        {
            if (currentDialog != null)
            {
                await currentDialog.Hide();
            }

            currentDialog = dialogDataList.Find(data => data.state == state)?.dialog;
            
            if (currentDialog != null)
            {
                await currentDialog.Show();
            }
        }

        public void ShowRoomName(string roomName)
        {
            var dislpayRoomName = $"{roomName.Substring(0, 3)}<space=20>{roomName.Substring(3)}";
            roomNameText.text = dislpayRoomName;
        }

        [Serializable]
        public class DialogData
        {
            public TitleModel.State state;
            public Dialog dialog;
        }
    }
}
