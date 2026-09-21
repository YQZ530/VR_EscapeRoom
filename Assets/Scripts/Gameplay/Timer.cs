using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DCXR.GameLogic
{
    public class Timer : MonoBehaviour
    {
        public int time;

        private void Start()
        {
            time = 0;

        }
        private void OnEnable()
        {
            GameManager.onGameStart += StartTimer;
            GameManager.onGameEnd += StopTimer;
        }

        private void OnDisable()
        {
            GameManager.onGameStart -= StartTimer;
            GameManager.onGameEnd -= StopTimer;
        }
        public int GetTime()
        {
            return time;
        }
        void StopTimer()
        {

            CancelInvoke();
        }

        void StartTimer()
        {
            time = 0;

            InvokeRepeating("TickTimer", 0f, 0.02f);
        }


        void TickTimer()
        {

            time++;
        }
    }
}