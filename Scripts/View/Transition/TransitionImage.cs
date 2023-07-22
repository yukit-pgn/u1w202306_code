using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Service;

namespace Main.View.Transition
{
    public class TransitionImage : MonoBehaviour
    {
        Image image;
        float time;
        
        void Start()
        {
            image = GetComponent<Image>();
            time = SceneService.transitionTime;
            SceneService.OnLoadCompleteAsObservable().Subscribe(_ => FadeOut()).AddTo(this);
            
            FadeIn();
        }

        void FadeIn()
        {
            image.color = Color.clear;
            image.DOColor(Color.black, time / 2);
        }

        void FadeOut()
        {
            image.DOColor(Color.clear, time / 2);
        }
    }
}
