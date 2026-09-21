
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;
using UnityEngine.UIElements;

using DCXR.Graph;
using UnityEditor;
using UnityEngine.Events;
using DCXR.Clustering;
//using DCXR.UI;

namespace DCXR.Replayer.UI
{
    struct ReplayerSliderGroup
    {
        public GameObject replayerGameObj;
        public BasicReplayer replayerComponent;
        public VisualElement replayerUI;
    }
    public class UIManager : MonoBehaviour, ITabSwitch
    {
        public UnityAction<string, string> TabSwitchEvent { get; set; } = null;
        //PlayerManager playerManager;
       
        T2EyeGaze_GroupReplayer T2EyeGaze_groupReplayer;
        
        VisualElement root;

        ClusterPython clusterPython;
        GraphManager graphManager;

        void Start()
        {
            //1. Get components
            //mainCameraControl = GameObject.Find("CameraSet").GetComponent<CameraControlManager>();
            clusterPython = GetComponentInChildren<ClusterPython>();
            if (clusterPython == null)
            {
                Debug.LogError("Cannot find clusterPython");
            }
            //graphManager = this.transform.GetChild(1).GetComponent<GraphManager>();


            T2EyeGaze_groupReplayer = this.GetComponentInChildren<T2EyeGaze_GroupReplayer>();
            if(T2EyeGaze_groupReplayer == null)
            {
                Debug.LogError("Cannot find");
            }
            //2.Link USS to this main visual element
            InitializedRootElem();

            //InitializedTab(); should wait for gameloader finish to load
        }
        void InitializedRootElem()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            var mainView_styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/MainView.uss");
            root.styleSheets.Add(mainView_styleSheet);
         
        }

        void OnActiveTabChange(Tab previoustab, Tab newtab)
        {
            TabSwitchEvent?.Invoke(previoustab.name, newtab.name);
        }
        //after all database is loaded and receive the event signal, then initialized the UI
        void InitializedTab()
        {
            if (root == null) InitializedRootElem();
            TabView mainTabView = root.Q<TabView>("MainTabView");
            mainTabView.activeTabChanged += OnActiveTabChange;

            //initialized this tab even though user has not click on that tab
            Construct_T2b_Result();
           

            Tab T2b = root.Q<Tab>("T2b");
            T2b.RegisterCallbackOnce<ClickEvent>((e) => { Construct_T2b_EyeGaze();  });

            Debug.Log("Done Initialized Tab");
        }

 
      
        #region T2_EyeGaze

        void Construct_T2b_EyeGaze()
        {
            Debug.Log("construct T2 Eye Gaze Tab");
            //TabSwitchEvent?.Invoke();
            VisualElement T2b = root.Q<VisualElement>("T2b");
            VisualElement T2bS1 = T2b.Q<VisualElement>("T2bS1");


            IntegerField TarEvtType = T2bS1.Q<IntegerField>("TarEvtType");
            TarEvtType.dataSource = clusterPython;
            TarEvtType.SetBinding(nameof(IntegerField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(clusterPython.tarEvtType)) });
            TarEvtType.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                clusterPython.SetTarEvtType(evt.newValue, clusterPython.tarEvtType);
            });

            

            IntegerField numFixation = T2bS1.Q<IntegerField>("Minfixation");
            numFixation.dataSource = clusterPython;
            numFixation.SetBinding(nameof(IntegerField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(clusterPython.minFixation)) });

            IntegerField numCluster = T2bS1.Q<IntegerField>("NumCluster");
            numCluster.dataSource = clusterPython;
            numCluster.SetBinding(nameof(IntegerField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(clusterPython.ueyeGaze_numCluster)) });

            ToggleButtonGroup featureGroup = T2bS1.Q<ToggleButtonGroup>("ClusterFeatureGroup");
            featureGroup.allowEmptySelection = false;
            featureGroup.isMultipleSelection = true;
            featureGroup.dataSource = clusterPython;

            featureGroup.RegisterCallback<ChangeEvent<ToggleButtonGroupState>>((evt) =>
            {
                ToggleButtonGroupState state = evt.newValue;
                List<bool> selected = new List<bool>();
                for (int i = 0; i < (int)EyeGazeClusterFeature.Tot; i++)
                {
                    selected.Add(state[i]);
                    //Debug.Log($"state {state[i]}");
                }
                clusterPython.Set_EyeGazeClusterFeature(selected);

            });


            EnumField distMetric = T2bS1.Q<EnumField>("DistMetric");
            distMetric.dataSource = clusterPython;
            distMetric.SetBinding(nameof(EnumField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(clusterPython.selected_EyeGaze_CMetric)) });
            //distMetric.RegisterCallback<ChangeEvent<EyeGazeClusterMetric>>((evt) =>
            //{
            //    clusterPython.selected_EyeGaze_CMetric = evt.newValue;
            //});

            EnumField algorithm = T2bS1.Q<EnumField>("Algorithm");
            algorithm.dataSource = clusterPython;
            algorithm.SetBinding(nameof(EnumField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(clusterPython.selected_EyeGaze_CAlgo)) });
            //distMetric.RegisterCallback<ChangeEvent<EyeGazeClusterAlgor>>((evt) =>
            //{

            //    clusterPython.selected_EyeGaze_CAlgo = evt.newValue; ;
            //});

            //finally run cluster
            VisualElement clusterButt = T2bS1.Q<Button>("Cluster");
            clusterButt.RegisterCallback<ClickEvent>((e) => clusterPython.RunCluster(ClusterCommand.eyegaze));
            
        

        }
       
        void Construct_T2b_Result()
        {
            VisualElement T2bS2 = root.Q<VisualElement>("T2bS2");
            

            Label totalGroupLabel = T2bS2.Q<Label>("TotalGroupLabel");
            
            IntegerField targetGroup = T2bS2.Q<IntegerField>("TargetGroup");
            IntegerField targetPlayer = T2bS2.Q<IntegerField>("TargetPlayer");
            //TextField clusterResultText = root.Q<VisualElement>("T2b").Q<TextField>("ClusterResultText");
            //clusterResultText.visible = false;


            VisualElement T2b = root.Q<VisualElement>("T2b");
            VisualElement clusterResultTextContainer = T2b.Q<VisualElement>("gazeClusterResultTextContainer");
            if (clusterResultTextContainer == null) { Debug.LogError("Cannot find text field for displaying eyegaze text"); }

            ToggleButtonGroup visOption = T2bS2.Q<ToggleButtonGroup>("visOption");
            if (visOption == null) Debug.LogError("cannot find visOPtion in T2B");
            visOption.allowEmptySelection = true;
            visOption.isMultipleSelection = true;
            visOption.RegisterCallback<ChangeEvent<ToggleButtonGroupState>>(
                (e) => this.T2EyeGaze_groupReplayer.SetLevelOfDetails(e.newValue)
            );

            Button showCluster = T2bS2.Q<Button>("ShowCluster");
            
           
            T2EyeGaze_groupReplayer.SetUI(totalGroupLabel, targetGroup, targetPlayer, clusterResultTextContainer, showCluster);



            FloatField heatmapMaxCutoff = T2bS2.Q<FloatField>("HeatmapMaxCut");
            heatmapMaxCutoff.dataSource = T2EyeGaze_groupReplayer;
            heatmapMaxCutoff.SetBinding(nameof(FloatField.value),
               new DataBinding() { dataSourcePath = new PropertyPath(nameof(T2EyeGaze_groupReplayer.heatmap_cutoffMax)) });
            //heatmapMaxCutoff.RegisterCallback<ChangeEvent<float>>((evt) =>
            //{
            //    T2EyeGaze_groupReplayer.OnHeatmapValueChange(evt.newValue);
            //    Debug.Log("OnHeatmapValueChange" + evt.newValue);
            //});

        }
        #endregion

        private void OnClusterDone(int numGroup)
        {
            VisualElement afterCluster = root.Q<VisualElement>("AfterCluster");
            ToggleButtonGroup clusterTBG = afterCluster.Q<ToggleButtonGroup>("ClusterButtGroup");
            // RemoveButtonFromButtonGroup(clusterTBG);


            for (int gid = 0; gid < numGroup; gid++)
            {
                int buttonID = gid;
                var button = new Button() { text = "G" + gid, tooltip = "Group" + gid };
                button.name = gid.ToString();
                // button.clicked += (() => OnClusterGroupClick(buttonID));

                button.AddToClassList("ClusterTBG");
                clusterTBG.Add(button);
            }
            //
            // clusterTBG.RegisterCallback<ChangeEvent<ToggleButtonGroupState>>((evt) =>
            // {
            //     ToggleButtonGroupState state = evt.newValue;
            //     T2_groupReplayer.OnSelectedGroupIDsCallback(state);
            // });
            //
            // ToggleButtonGroup VisToggleGroup = afterCluster.Q<ToggleButtonGroup>("VisToggleGroup");
            // VisToggleGroup.RegisterCallback<ChangeEvent<ToggleButtonGroupState>>((evt) =>
            // {
            //     ToggleButtonGroupState state = evt.newValue;
            //     T2_groupReplayer.OnVisualToggleChange(state);
            // });


            
            Debug.Log("TODO::update cluster result UI");
        }

        private void OnEnable()
        {
            PythonEditor.onClusterDone += OnClusterDone;
            GameLoader.DoneLoadingEvent += InitializedTab;
        }

        private void OnDisable()
        {
            PythonEditor.onClusterDone -= OnClusterDone;
            GameLoader.DoneLoadingEvent -= InitializedTab;
        }

       
    }

}



//void ConstructRepalyerSlider(VisualElement tab, AdvanceReplayer replayer, GameObject replayerObj)
//{

//    VisualElement playButt = tab.Q<Button>("PlayButt");

//    playButt.RegisterCallback<ClickEvent>((e) => { replayer.playForward(); EnableDisableCamView(false); });

//    VisualElement pauseButt = tab.Q<Button>("PauseButt");
//    pauseButt.RegisterCallback<ClickEvent>((e) => { replayer.PauseReplay(); EnableDisableCamView(true); });

//    VisualElement speedSlider = tab.Q<SliderInt>("SpeedMultiSlider");
//    speedSlider.dataSource = replayer;
//    //Debug.Log($"{nameof(SliderInt.value)}");
//    speedSlider.SetBinding(nameof(SliderInt.value),
//        new DataBinding() { dataSourcePath = new PropertyPath(nameof(replayer.speedMultiplier)) });
//    speedSlider.RegisterCallback<ClickEvent>((e) => replayer.OnSpeedChanged());

//    //replayer componnent
//    VisualElement replayerSlider = tab.Q<VisualElement>("ReplayerSlider");

//    ToggleButtonGroup progressbar = replayerSlider.Q<ToggleButtonGroup>("ProgressbarButtGroup");
//    progressbar.allowEmptySelection = true;

//    SliderInt replayerSliderbar = replayerSlider.Q<SliderInt>("SliderBar");
//    replayerSliderbar.dataSource = replayer;

//    replayerSliderbar.SetBinding(nameof(SliderInt.value),
//        new DataBinding() { dataSourcePath = new PropertyPath(nameof(replayer.curProgress)) });
//    replayerSliderbar.RegisterCallback<ClickEvent>((e) => replayer.OnCurProgrssChanged());

//    replayerSliderList.Add(new ReplayerSliderGroup()
//    {
//        replayerComponent = replayer,
//        replayerUI = replayerSlider,
//        replayerGameObj = replayerObj,
//    });
//}



//private void WalkPathRotCallback(ChangeEvent<ToggleButtonGroupState> e)
//{
//    ToggleButtonGroupState state = e.newValue;

//    playerManager.DisplayPlayerStatus(state);

//    //Debug.Log($"{}");
//}//public void AddMarker(AnchorType markerType, int ithButton)
//{
//    if (replayerSliderList == null) return;
//    for(int i = 0; i < replayerSliderList.Count; i++)
//    {
//        VisualElement replayerSlider = replayerSliderList[i].replayerUI;
//        BasicReplayer replayer = replayerSliderList[i].replayerComponent;

//        ToggleButtonGroup progressbar = 
//            replayerSlider.Q<ToggleButtonGroup>("ProgressbarButtGroup");

//        var buttonTooltip = markerType == AnchorType.Chunk ? "c" + ithButton.ToString() : "e" + ithButton.ToString();
//        var button = new Button() { text = buttonTooltip, tooltip = buttonTooltip };
//        button.name = ithButton.ToString();

//        button.clicked += (() => replayer.OnAnchorSelected(ithButton, markerType));
//        if (markerType == AnchorType.Chunk)
//        {
//            //hex color 66E7F8
//            button.style.backgroundColor = new Color(0.3987629f, 0.9046196f, 0.9716981f);
//        }
//        else
//        {
//            button.style.backgroundColor = new Color(0.4f, 0.528477f, 0.972549f);
//        }
//        button.AddToClassList("ProgressbarButt");

//        progressbar.Add(button);
//    }


//    ithButton++;
//}

//public void DestoryAllMarkers()
//{
//    if (replayerSliderList == null) {
//        Debug.LogWarning("Cannot find replayer slider UI");
//        return; 
//    }

//    for (int rid = 0; rid < replayerSliderList.Count; rid++)
//    {
//        VisualElement replayerSlider = replayerSliderList[rid].replayerUI;

//        ToggleButtonGroup progressbar = replayerSlider.Q<ToggleButtonGroup>("ProgressbarButtGroup");
//        RemoveButtonFromButtonGroup(progressbar);

//    }
//}