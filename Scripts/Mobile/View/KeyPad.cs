using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mobile
{
    public class KeyPad : MonoBehaviour
    {
        const int MaxNumberCount = 6;
        
        [SerializeField] List<Button> numberButtons;
        [SerializeField] Button deleteButton;
        [SerializeField] TextMeshProUGUI displayText;
        
        List<int> numberList = new List<int>();
        StringBuilder builder;
        
        Subject<string> onEnterID = new Subject<string>();
        public IObservable<string> OnEnterID => onEnterID;

        StringBuilder DefaultBuilder
        {
            get
            {
                var builder = new StringBuilder();
                for (var i = 0; i < MaxNumberCount; i++)
                {
                    builder.Append('_');
                }

                return builder;
            }
        }

        void Awake()
        {
            var number = 0;
            foreach (var button in numberButtons)
            {
                var numCopy = number;
                button.OnClickAsObservable().Subscribe(_ => EnterNumber(numCopy)).AddTo(this);
                number++;
            }
            
            deleteButton.OnClickAsObservable().Subscribe(_ => DeleteNumber()).AddTo(this);

            builder = DefaultBuilder;
            displayText.text = builder.ToString();
        }
        
        void EnterNumber(int number)
        {
            if (numberList.Count == MaxNumberCount)
            {
                return;
            }
            
            numberList.Add(number);
            builder[numberList.Count - 1] = number.ToString()[0];
            displayText.text = builder.ToString();
            
            CheckFull();
        }
        
        void DeleteNumber()
        {
            if (numberList.Count == 0)
            {
                return;
            }
            
            numberList.RemoveAt(numberList.Count - 1);
            builder[numberList.Count] = '_';
            displayText.text = builder.ToString();
        }

        void CheckFull()
        {
            if (numberList.Count == MaxNumberCount)
            {
                onEnterID.OnNext(builder.ToString());
            }
        }
    }
}
