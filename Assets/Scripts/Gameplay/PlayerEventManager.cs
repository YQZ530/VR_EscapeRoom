using DCXR.GameLogic;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Events;



namespace DCXR.Recorder
{

    public class PlayerEventManager : MonoBehaviour
    {
        List<MGameEvent> eventList = new List<MGameEvent>();
        Timer timer;
        List<MEyeGazeEvent> eyeGazeEvents;
        MEyeGazeEvent curGazeEvent;


        public UnityAction<List<MGameEvent>, List<MEyeGazeEvent>> SaveGameplayEvent { set; get; }
        private void OnEnable()
        {
          
            GazeBallManager.MGazeHoverEnter += OnGazeHoverReceive;
            GazeBallManager.MGazeHoverExit += OnGazeHoverExitGameObj;

            VerifiedObjectMachine.ReceiveObjectEvent += OnCompleteGameEvent;

            GameManager.onGameStart += ResetGame;
            GameManager.onGameEnd += OnGameEnd;
        }
        private void OnDisable()
        {
            GazeBallManager.MGazeHoverEnter -= OnGazeHoverReceive;
            GazeBallManager.MGazeHoverExit -= OnGazeHoverExitGameObj;

            VerifiedObjectMachine.ReceiveObjectEvent -= OnCompleteGameEvent;

            GameManager.onGameEnd -= OnGameEnd;
            GameManager.onGameStart -= ResetGame;
        }
        void Start()
        {
            ResetGame();
            timer = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Timer>();
            if (timer == null) Debug.LogError("cannot find obj with game manager tag or cannot find timer script");
            eventList = new List<MGameEvent>();
        }
        private void ResetGame()
        {
            if (eventList != null) eventList.Clear();
            eventList = new List<MGameEvent>();
            if (eyeGazeEvents != null) eyeGazeEvents.Clear();
            eyeGazeEvents = new List<MEyeGazeEvent>();
            
        }

       
  

        void OnGazeHoverReceive(string areaName, string parentName, string gazeObject)
        {
            curGazeEvent = new MEyeGazeEvent();
            curGazeEvent.AreaName = areaName;
            curGazeEvent.ParentName = parentName;
            curGazeEvent.GazeObject = gazeObject;
            curGazeEvent.startKeyframe = timer.GetTime();
        }

        void OnGazeHoverExitGameObj()
        {
            curGazeEvent.endKeyframe = timer.GetTime();
            curGazeEvent.dur = curGazeEvent.endKeyframe - curGazeEvent.startKeyframe;
            eyeGazeEvents.Add(curGazeEvent);
        }

        void OnCompleteGameEvent(GameEventType eventType)
        {
            int startTime = 0;
            int endTime = timer.GetTime();
            if (eventList != null && eventList.Count >=1)
                startTime = eventList[eventList.Count - 1].keyframe.endKeyframe + 1;
            MGameEvent e = new MGameEvent(startTime, endTime, eventType);
            eventList.Add(e);
        }

        void OnGameEnd()
        {
            
            SaveGameplayEvent?.Invoke(eventList, eyeGazeEvents);
        }
    }

}