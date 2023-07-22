using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main
{
    public class InputObserver : MonoBehaviour
    {
        public bool hasAccelerometer { get; private set; }
        
        Vector3ReactiveProperty acceleration = new Vector3ReactiveProperty();
        public IReadOnlyReactiveProperty<Vector3> Acceleration => acceleration;
        
        public void Setup()
        {
            var accelerometer = Accelerometer.current;
            if (accelerometer == null)
            {
                hasAccelerometer = false;
                return;
            }

            hasAccelerometer = true;
            InputSystem.EnableDevice(accelerometer);

            this.UpdateAsObservable().Subscribe(_ =>
            {
                acceleration.Value = accelerometer.acceleration.ReadValue();
            }).AddTo(this);
        }

        void Update()
        {
#if UNITY_EDITOR
            var y = Input.GetAxis("Vertical");
            acceleration.Value = new Vector3(0, -1f + y * 2f, 0f);
#endif
        }
    }
}
