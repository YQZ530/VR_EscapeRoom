
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DCXR.GameLogic;
using UnityEngine.XR.Interaction.Toolkit;
using System;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.XRGazeInteractor))]
public class GazeBallManager : MonoBehaviour,IEyeGazeRecorder
{
    //public UnityAction<string,string> MGazeHoverEnter { set; get; } = null;
        public static event Action<string, string, string> MGazeHoverEnter;
        public static event Action MGazeHoverExit;
        public UnityAction<List<MEyeGazeData>> SaveRawEyeGazeRecord { set; get; } = null;
        List<MEyeGazeData> mEyeGazeData;
        public Transform mEyeGazeSphere;
        XRGazeInteractor _interactor;
        GameObject lastTargetObj;
        public void Start()
        {
           
            _interactor = GetComponent<XRGazeInteractor>();
         
            mEyeGazeSphere.position = _interactor.rayEndPoint;
            mEyeGazeData = new List<MEyeGazeData>();

            _interactor.hoverEntered.AddListener(OnHoverGameObject);
            _interactor.hoverExited.AddListener(OnHoverExitGameObj);
        }

      

         void OnHoverGameObject(HoverEnterEventArgs args)
        {
            GameObject obj = args.interactableObject.transform.gameObject;

            var script = obj.GetComponent<GazeHoverableObj>();
            if ( script == null)
            {
                Debug.LogWarning($"this obj_{obj.name}_{obj.transform.root} does not have GazeHOverable Script");
                return;
            }
            
            MGazeHoverEnter.Invoke(script.AreaName, script.ParentName, script.GazeObjName);
            lastTargetObj = obj;
            
        }
        void OnHoverExitGameObj(HoverExitEventArgs args)
        {
        
            GameObject obj = args.interactableObject.transform.gameObject;
            if (obj == null)
            {
                Debug.LogError($"hoverExitObj is null,  lasttargetobj is {lastTargetObj}");
                return;
            }
            if(obj.name != lastTargetObj.name)
            {
                Debug.LogError($"hoverExitObj{obj.name} is not same as lasttargetobj{lastTargetObj}");
                return;   
            }
            MGazeHoverExit();
            if (lastTargetObj != null)
            {
                lastTargetObj = null;
            }
        }
    
        void Update()
        {
            mEyeGazeSphere.position = _interactor.rayEndPoint;
        }

    private void OnEnable()
    {
        GameManager.onGameStart += StartRecord;
        GameManager.onGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        GameManager.onGameStart -= StartRecord;
        GameManager.onGameEnd -= OnGameEnd;
    }
    //
    public void StartRecord()
    {
        if (mEyeGazeData != null) mEyeGazeData.Clear();

        mEyeGazeData = new List<MEyeGazeData>();

        Debug.Log("start record eye gaze raw data");
        InvokeRepeating("AddGazeData", 0f, 0.02f);
    }


    public void AddGazeData()
    {
        MEyeGazeData s = new MEyeGazeData
        {
            gazePos = _interactor.rayEndPoint,
        };

        mEyeGazeData.Add(s);
    }


    void OnGameEnd()
    {
        CancelInvoke();
        SaveRawEyeGazeRecord?.Invoke(mEyeGazeData);
    }


}



