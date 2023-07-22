using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Main.Service
{
    public class SceneService : MonoBehaviour
    {
        static Subject<Unit> OnLoadComplete = new Subject<Unit>();

        public static float transitionTime = 1f;

        public static async void ChangeScene(string name, float time = 0)
        {
            if (time <= 0)
            {
                await SceneManager.LoadSceneAsync(name);
            }
            else
            {
                transitionTime = time;
                var currentScene = SceneManager.GetActiveScene();
                await SceneManager.LoadSceneAsync("TransitionScene", LoadSceneMode.Additive);
                await UniTask.Delay((int)(time * 500));
                await SceneManager.UnloadSceneAsync(currentScene);
                await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
                OnLoadComplete.OnNext(Unit.Default);
                await UniTask.Delay((int)(time * 500));
                await SceneManager.UnloadSceneAsync("TransitionScene");
            }
        }

        public static async void LoadSubScene(string name)
        {
            await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }

        public static async void UnloadSubScene(string name)
        {
            await SceneManager.UnloadSceneAsync(name);
        }

        public static IObservable<Unit> OnLoadCompleteAsObservable()
        {
            return OnLoadComplete;
        }
    }
}
