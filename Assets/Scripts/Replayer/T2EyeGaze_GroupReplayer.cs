using System;
using DCXR.Clustering;
using DCXR.Graph;
using DCXR.Replayer;
using DCXR.Util;
using HeatmapVisualization;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class T2EyeGaze_GroupReplayer : MonoBehaviour
{
    public List<GameObject> groupPlayers;
    public Material defaultEyeGazeMaterial; // use for group id == -1 or user has no group
    protected PlayerManager playerManager;
    protected ClusterPython clusterPython;

    protected DiscreteDataLoader dataLoader;
    protected GameLoader gameLoader;
    protected Heatmap heatmap;

    //UI related vari & setting
    Label totalGroupLabel;
    //private TextField clusterResultText;
    private VisualElement clusterResultTextContainer;
    private List<Foldout> clusterResultList;

    public float heatmap_cutoffMax = 0.4f;

   // [ReadOnlyAttribute]
    int allGroupIdx = 3;//temp setting
   // [ShowInInspector]
    List<bool> levelOfDetail_toggleArr;

  //  [ReadOnlyAttribute]
    int targetGroup = -1;
  //  [ReadOnlyAttribute]
    int targetPlayerID = -1;


    // Start is called before the first frame update
    void Start()
    {
        if (playerManager == null) playerManager = this.transform.parent.GetComponent<PlayerManager>();
        gameLoader = this.GetComponentInParent<GameLoader>();
        if (gameLoader == null) Debug.LogError("Cannot find gameloader from parent script");
        dataLoader = GetComponent<DiscreteDataLoader>();
        heatmap = GetComponentInChildren<Heatmap>();
        if (heatmap == null) Debug.LogError("[!!!] Cannot not get script");

        clusterPython = GetComponent<ClusterPython>();
        if (clusterPython == null) Debug.Log($"{this.gameObject.name} cannot find clusterPython");
        if (defaultEyeGazeMaterial == null) Debug.LogError("Did not assign");

        //hook up listener events


        groupPlayers = new List<GameObject>();
        //line, point,text, heatmap
        levelOfDetail_toggleArr = new List<bool>() { false, false, false, false, false };

    }
    #region OnEnableDisable/Initialize
    private void OnEnable()
    {
        PythonEditor.onEyeGazeClusterDone += SetClusterResult;
         
    }

    private void OnDisable()
    {
        PythonEditor.onEyeGazeClusterDone -= SetClusterResult;
          
    }

       
    #endregion

    #region  UICallback
    public void SetUI (Label totalGroupLabel, IntegerField targetGroupField,
        IntegerField targetPlayerField, VisualElement clusterResultTextContainer, Button showCluster)
    {
        this.totalGroupLabel = totalGroupLabel;
        this.clusterResultTextContainer = clusterResultTextContainer;

        targetGroupField.dataSource = this;
        targetGroupField.SetBinding(nameof(IntegerField.value),
            new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.targetGroup)) });
        targetGroupField.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            this.targetGroup = evt.newValue -1;
            print($"internal target group {this.targetGroup}");
        });

        targetPlayerField.dataSource = this;
        targetPlayerField.SetBinding(nameof(IntegerField.value),
            new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.targetPlayerID)) });
        targetPlayerField.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            this.targetPlayerID = evt.newValue;
        });

        showCluster.RegisterCallback<ClickEvent>((e) => OnLevelOfDetailChange());

    }


    public void SetLevelOfDetails(ToggleButtonGroupState state)
    {
        levelOfDetail_toggleArr[(int)EyeGazeOption.Line] = state[0]; //line
        levelOfDetail_toggleArr[(int)EyeGazeOption.Points] = state[1]; //points
        levelOfDetail_toggleArr[(int)EyeGazeOption.Text] = state[2]; //text
        levelOfDetail_toggleArr[(int)EyeGazeOption.Heatmap] = state[3]; //heatmap
        OnLevelOfDetailChange();
    }

    [ShowInInspector]
    public void OnLevelOfDetailChange()
    {
       
        //if text is turn on
        if (levelOfDetail_toggleArr[(int)EyeGazeOption.Text])
        {
            this.clusterResultTextContainer.visible = true;

            UpdateUserPerformance();
        
        }
        else
        {
            this.clusterResultTextContainer.visible = false;
        }

        //if heatmap
        if (levelOfDetail_toggleArr[(int)EyeGazeOption.Heatmap])
        {
            heatmap.gameObject.GetComponent<MeshRenderer>().enabled = true;
            VisualizeHeatmap();
            
        }
        else
        {
            heatmap.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        //otherwise output 
        VisualizeGazePath();

    }

    #endregion
   

    

    public void VisualizeGazePath() //int gid, int playerIDName
    {
        //1 reset players
        playerManager.ResetAllPlayer();

        //playerManager.UpdateDrawPath(clusterPython.minFixation);

        ////2.get and load players to our main replayer obj
        if (targetGroup > allGroupIdx)
        {
            Debug.LogWarning($"[Input Error] There is no group id={targetGroup}; all group id should less than {allGroupIdx}");
            // return;
            Debug.Log("temp turn off");
        }
        
        
       
        ////3. Displaying target group 
        Debug.Log($"VisualizeGazePath()::target group{targetGroup},player id {targetPlayerID}");
        if (targetGroup == this.allGroupIdx)
        {
            //show all players
            playerManager.ShowGaze_AllGroup(in levelOfDetail_toggleArr, defaultEyeGazeMaterial);
            return;
        }

        ////4. If give groupid only, then show all players of that group
        if (targetPlayerID == -1)
        {
            playerManager.ShowGaze_SpecifixGroup(targetGroup, in levelOfDetail_toggleArr);
            return;
        }

        ////5. Check if the group is loaded

        if (playerManager.CheckValidGroupAndUser(targetGroup, targetPlayerID, allGroupIdx))
        {
            // show Group X player Y 
            playerManager.ShowGaze_SpecificGroupAndUser(targetGroup, targetPlayerID, in levelOfDetail_toggleArr);
        }
        else
        {
            Debug.LogWarning($"[Input Error] cannot find group of player for this gid{ targetGroup}");
        }


    }



 
    #region text

    void SetClusterResult(int finalNumcluster)
    {
        this.allGroupIdx = finalNumcluster;
        Debug.LogWarning($"final cluster group is {allGroupIdx}"); ; 
        if (this.allGroupIdx == 0) { Debug.LogWarning("final cluster group is zero"); return; }

        this.totalGroupLabel.text = "Total Group: " + this.allGroupIdx.ToString();
        CreateUIForGroup();
    }

    void CreateUIForGroup()
    {
        this.clusterResultTextContainer.Clear();
        if (this.clusterResultList != null) this.clusterResultList.Clear();
        this.clusterResultList = new List<Foldout>();
        for (int gid = 0; gid < this.allGroupIdx; gid++)
        {
            Foldout gFoldOut = new Foldout { };

            gFoldOut.style.unityFontStyleAndWeight = FontStyle.Bold;
            gFoldOut.name = $"Group{gid + 1}";
            gFoldOut.text = $"Group{gid + 1}";
            gFoldOut.style.color = playerManager.GetGroupMaterialColor(gid);

            TextField textfield = new TextField();
            textfield.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            textfield.multiline = true;
            textfield.textEdition.hidePlaceholderOnFocus = false;
            //textfield.isReadOnly = true;
            textfield.label = "";
            gFoldOut.Add(textfield);
            clusterResultList.Add(gFoldOut);
            this.clusterResultTextContainer.Add(gFoldOut);
        }

        // Debug.Log("Done CreateUIForGroup");
    }


    
    void UpdateUserPerformance()
    {

        //targetGroup, targetUser
        if (this.allGroupIdx <= 0) { Debug.LogError("[bug] all group idx is less than zero " + this.allGroupIdx + "\n return without update text"); return; }
        //show all players
        for (int gid = 0; gid < this.allGroupIdx; gid++)
        {
            string finalText = "";
            
            List<int> playerIDs = dataLoader.GetUserList(gid);
            //for all users
            foreach (int pid in playerIDs)
            {
                finalText += $"player{pid} with {ToString_UserPerformanceByGroupHelper(pid)}\n";
            }

            TextField textfield = clusterResultList[gid].Q<TextField>();
            if (textfield == null) { Debug.LogError("cannot find text field to display resutl for group " + gid); return; }
            textfield.value = finalText;

        }

    }
     private string ToString_UserPerformanceByGroupHelper(int pid)
        {
            string s = "";
            if (!gameLoader.CheckIfUserHasGroup(pid)) return "";
            List<KeyValuePair<EyeGazeClusterFeature, bool>> selected_PoseFeatureArr = clusterPython.Get_GazeClusterFeature();

            foreach (KeyValuePair<EyeGazeClusterFeature, bool> poseFeature in selected_PoseFeatureArr)
            {

                if ( poseFeature.Key == EyeGazeClusterFeature.Tot) continue;
                if (poseFeature.Value)
                {
                    switch (poseFeature.Key)
                    {
                        case EyeGazeClusterFeature.Fixation:
                            List<MEyeGazeEvent> mEyeGazeEvents = new List<MEyeGazeEvent>();
                            gameLoader.GetSelectedEyegazeEvent(pid, out mEyeGazeEvents);
                          
                           
                            s += ToString_GazeEvent(mEyeGazeEvents)+ "\n\t\t";
                            break;
                        case EyeGazeClusterFeature.SumDur:
                            s += $"SumDur= {gameLoader.GetAggregate_EyeGazeData(pid, "SumDur")} ";
                            break;
                        case EyeGazeClusterFeature.StdDur:
                            s += $"StdDur= {gameLoader.GetAggregate_EyeGazeData(pid, "StdDur")} ";
                            break;
                        case EyeGazeClusterFeature.AvgDur:
                            s += $"AvgDur= {gameLoader.GetAggregate_EyeGazeData(pid, "AvgDur")} ";
                            break;
                        case EyeGazeClusterFeature.NumFixation:
                            s += $"NumFixation= {gameLoader.GetAggregate_EyeGazeData(pid, "NumFixation")} ";
                            break;
                        default:
                            Debug.Log("undefine key" + poseFeature.Key);
                            break;
                    }
                }

            }
            return s;
        }
     
     string ToString_GazeEvent(List<MEyeGazeEvent> mEyeGazeEvents)
     {
         string s = "[";
         for(int i =0; i < mEyeGazeEvents.Count; i++)
         {
             s += $"{mEyeGazeEvents[i].AreaName}_{mEyeGazeEvents[i].ParentName},";
         }
         s.Remove(s.Length - 1); //remove last comma
         s += "] ";
         return s;
     }
     #endregion

     #region heatmap
        public void OnHeatmapValueChange(float v)
        {
            this.heatmap_cutoffMax = v;
            VisualizeHeatmap();

        }

        void VisualizeHeatmap()
        {
            //set max
            heatmap.cutoffPercentage = this.heatmap_cutoffMax;

            //
            if(targetGroup == -1 || targetGroup == this.allGroupIdx)
            {
                OutputHeatmap_ForAll();
                return;
            }
            
          
            OutputHeatmap_ByGroup();
        }

        

        void OutputHeatmap_ByGroup()
        {
                heatmap.Reset();
            
                List<int> userIDs = playerManager.GetPlayerIDsFromGroup(targetGroup, allGroupIdx);
                
                //for all users
                foreach (int uid in userIDs)
                {
                    List<MEyeGazeEvent> mEyeGazeEvents;
                    //Debug.Log($"target group {targetGroup} uid "+ uid);
                    gameLoader.GetSelectedEyegazeEvent(uid, out mEyeGazeEvents);

                    List<Vector3> objpos = ExtraAllTarObjFromEvent(mEyeGazeEvents);
                    heatmap.GenerateHeatmap(objpos);

                }

            Debug.Log("OutputHeatmap_ByGroup()");
            
        }
        List<Vector3> ExtraAllTarObjFromEvent(List<MEyeGazeEvent> mEyeGazeEvents)
        {
            List<Area> chunks = gameLoader.GetAreaList();

                List<Vector3> allTarObjFromEvent = new List<Vector3>();
                foreach (MEyeGazeEvent e in mEyeGazeEvents)
                {
                    
                    GameObject tarObj = null;
                    //HelperMethod.FindParentObjInGame(e.AreaName, e.ParentName, chunks, out tarObj);
                    List<Vector3> tempose=  HelperMethod.FindGazeObjInGame(e.AreaName, e.ParentName, e.GazeObjectArr,chunks);
                    allTarObjFromEvent.AddRange(tempose);
                    //if (tarObj != null)
                    //{
                        // Vector3 v =  tarObj.transform.position;
                        // for(int j =0; j < duration; j ++)
                        // {
                        //     objpos.Add(v);
                        // }
                     
                } 
        



            return allTarObjFromEvent;
        }
        void OutputHeatmap_ForAll()
        {
                heatmap.Reset();
                List<GameObject> playerList = playerManager.playerlist;
        //targetGroup, targetUser
                List<Vector3> objpos = new List<Vector3>();
                for (int uid = 0; uid < playerList.Count; uid++)
                {
                    List<MEyeGazeEvent> mEyeGazeEvents;
                    gameLoader.GetSelectedEyegazeEvent(uid, out mEyeGazeEvents);

                    gameLoader.GetSelectedEyegazeEvent(uid, out mEyeGazeEvents);

                      
                    objpos.AddRange(ExtraAllTarObjFromEvent(mEyeGazeEvents) );
                }

        heatmap.GenerateHeatmap(objpos);
            Debug.Log("OutputHeatmap_ForAll()()");

        }

        #endregion

}
