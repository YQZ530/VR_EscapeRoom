using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace DCXR.GameLogic
{
    public class GameManager : MonoBehaviour
    {
        public static event Action onGameStart;
        public static event Action onGameEnd;
        public static event Action onOutputData;
        

        public float timer;
        public float waitTime = 0.5f;
        public bool shouldOutputData = false;
        public GameObject VRCanvas;
        void Start()
        {
            if (VRCanvas == null) Debug.LogError("Did not assign vr canvas");
            
        }

        // Update is called once per frame
        void Update()
        {
            if (shouldOutputData)
            {
                timer += Time.deltaTime;

                if (timer > waitTime)
                {
                    onOutputData?.Invoke();

                    this.enabled = false;
                }
            }

        }
        public void ResetGame()
        {
            timer = 0;
            shouldOutputData = false;
        }

        //Respond for button hit on canvas

       
        public void StartGame()
        {
            ResetGame();
            Debug.Log("start Game!");
            onGameStart?.Invoke();
        }

        public void EndGame()
        {
            onGameEnd?.Invoke();  
            GetComponent<AudioSource>()?.Play();
            Debug.Log("End Game!");
            shouldOutputData = true;
            VRCanvas.SetActive(true);
        }
    }
}