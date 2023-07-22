using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Game
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] Button backButton;
        [SerializeField] List<DialogData> dialogDataList;
        
        Dialog currentDialog;

        public IObservable<Unit> OnBack => backButton.OnClickAsObservable();
        
        public async UniTask ChangeDialog(GameModel.State state)
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

        public void UpdateScore(float score)
        {
            scoreText.text = $"{score:F3}m";
        }

        [Serializable]
        class DialogData
        {
            public GameModel.State state;
            public Dialog dialog;
        }
    }
}
