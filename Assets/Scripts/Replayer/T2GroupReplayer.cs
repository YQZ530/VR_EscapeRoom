
using DCXR.Clustering;
//using DCXR.GameComponent;
using DCXR.Graph;
using DCXR.PathDrawing;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DCXR.Replayer
{
    public class T2GroupReplayer : MonoBehaviour, ICamPlayerSwitch
    {
        public UnityAction<string>CamPlayerSwitch { get; set; } = null;
        protected PlayerManager playerManager;
        protected EventManager eventManager;
        protected GameLoader gameLoader;
        protected ClusterPython clusterPython;
        DiscreteDataLoader dataLoader;
        internal AdvanceReplayer mainReplayer;

        int allGroupIdx = 0;
        private GameObject mainCamPlayer;
        private int startChunkIdx;
        private int endChunkIdx;

        List<bool> visTBG;//Visulization toggle button group
     
        private VisualElement clusterResultTextContainer;
        private List<Foldout> clusterResultList;
        private void Start()
        {
            gameLoader = this.GetComponentInParent<GameLoader>();
            if (gameLoader == null) Debug.Log($"{this.gameObject.name} cannot find gameLoader");
            dataLoader = GetComponent<DiscreteDataLoader>();

            if (eventManager == null) eventManager = this.transform.parent.GetComponent<EventManager>();
            if (playerManager == null) playerManager = this.transform.parent.GetComponent<PlayerManager>();
            clusterPython = GetComponent<ClusterPython>();
            if(clusterPython == null) Debug.Log($"{this.gameObject.name} cannot find clusterPython");

            mainReplayer = new AdvanceReplayer(gameLoader, eventManager);


            visTBG = new List<bool>() { false, false, false }; //walk path & rotation

            var uicontroller = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ITabSwitch>();
            foreach (var r in uicontroller)
            {
                r.TabSwitchEvent += onTabSwitch;
            }
            clusterResultList = new List<Foldout>();
        }

        public void AnchorEventClickCallback(List<MGameEvent> gameplayEvent)
        {
            Debug.Log("todo");
        }

        #region clusterResultCallback
        public void OnSelectedGroupIDsCallback(UnityEngine.UIElements.ToggleButtonGroupState state)
        {
          List<int> groupIdList = new List<int> { };
            for (int i = 0; i < this.allGroupIdx; i += 1)
            {
                if (state[i])
                {
                   groupIdList.Add(i);
                }
            }


          //  LoadSelectedGroups(groupIdList);
        }

        void onSetChunkRange(int startChunk, int endChunk)
        {
            this.startChunkIdx = startChunk;
            this.endChunkIdx = endChunk;
            Debug.Log($"[Replayer]:: start chunk {startChunk} end chunk{endChunk}");
        }

        [ShowInInspector]
        public void LoadSelectedGroups(List<int> groupIdList)
        {
            //reinitialize
            playerManager.ResetAllPlayer();
            mainReplayer.ReInitializedPlayerList();

            if (eventManager == null) eventManager = this.transform.parent.GetComponent<EventManager>();
            if (playerManager == null) playerManager = this.transform.parent.GetComponent<PlayerManager>();

            //load for each group
            foreach (int gid in groupIdList)
            {
                OnSelectGroupId(gid);
            }

            
        }

        //call from UI side, when user select a group to what preview
        public GameObject GetMainCamPlayer()
        {
            return mainCamPlayer;
        }

        void OnSelectGroupId(int gid)
        {
            // Debug.Log($"Selected group {gid}");
            // //get and load players to replayer
            // List<int> selectedPIDList = playerManager.GetPlayerIDsFromGroup(gid, allGroupIdx);
            //
            // if (selectedPIDList == null || selectedPIDList.Count == 0)
            // {
            //     Debug.Log("no player in this group, try again");
            //     return;
            // }
            // //Generate List of repalyer objs
            //
            // for (int i = 0; i < selectedPIDList.Count; i++)
            // {
            //     int pid = selectedPIDList[i];
            //     GameObject player = playerManager.GetPlayerObj(pid);
            //     player.SetActive(true);
            //     //set main cam follow player 0
            //     if (i == 0)
            //     {
            //         mainCamPlayer = player;
            //         CamPlayerSwitch.Invoke("T2");
            //
            //     }
            //
            //     mainReplayer.AddPlayerToList(player, pid, startChunkIdx, endChunkIdx);
            //    
            // }

            //display visualization based on toggle butt
            //playerManager.DisplayVis_SpecifixGroup(gid, selectedPIDList, in visTBG);

            Debug.Log("TODO");
        }


        public void SetupTextFieldUI(VisualElement textContainer)
        {
            this.clusterResultTextContainer = textContainer;
        }
        //public void SetupTextFieldUI(TextField textField)
        //{
        //   // this.clusterResultText = textField;
        //}

        public void OnVisualToggleChange(UnityEngine.UIElements.ToggleButtonGroupState state)
        {
            visTBG[0] = state[0];
            visTBG[1] = state[1];
            visTBG[2] = state[2];


            if (visTBG[2])
            {
               this.clusterResultTextContainer.visible = true;

                UpdateUserPerformance();
            }
            else
            {
              this.clusterResultTextContainer.visible = false;
            }
            Debug.Log("T2GroupReplayer::OnVisualToggleChange");
        }
      

        void CreateUIForGroup()
        {
            this.clusterResultTextContainer.Clear();
           this.clusterResultList.Clear();
            this.clusterResultList = new List<Foldout>();
            for (int gid =0; gid < this.allGroupIdx; gid++)
            {
                Foldout gFoldOut = new Foldout { };
               
                gFoldOut.style.unityFontStyleAndWeight = FontStyle.Bold;
                gFoldOut.name = $"Group{gid}";
                gFoldOut.text = $"Group{gid}";
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

            Debug.Log("Done CreateUIForGroup");
        }
        void UpdateUserPerformance()
        {
         
            // //targetGroup, targetUser
            // if (this.allGroupIdx <= 0) { Debug.LogError("[bug] all group idx is less than zero " + this.allGroupIdx); return; }
            // //show all players
            // for (int gid = 0; gid < this.allGroupIdx; gid++)
            // {
            //     string finalText = "";
            //     List<GameObject> userIDs = playerManager.GetPlayersFromGroup(gid, allGroupIdx);
            //     //for all users
            //     foreach (GameObject player in userIDs)
            //     {
            //         string text = ToString_UserPerformanceByGroupHelper(player);
            //         finalText += $"{player.name}::{text}\n";
            //
            //     }
            //
            //     TextField textfield = clusterResultList[gid].Q<TextField>() ;
            //     if(textfield == null) { Debug.LogError("cannot find text field to display resutl for group "+ gid); return; }    
            //     textfield.value = finalText;
            
            Debug.Log("TODO");

            //}

        }

        private string ToString_UserPerformanceByGroupHelper(GameObject player)
        {
            // string s = "";
            // RePlayerStatus playerData = player.GetComponent<RePlayerStatus>();
            // int pid = playerData.playerID;
            // List<KeyValuePair<PosesClusterFeature, bool>> selected_PoseFeatureArr = clusterPython.Get_PoseClusterFeature();
            //
            // foreach (KeyValuePair<PosesClusterFeature, bool> poseFeature in selected_PoseFeatureArr)
            // {
            //     
            //     if (poseFeature.Key == PosesClusterFeature.WalkPath || poseFeature.Key == PosesClusterFeature.Tot) continue;
            //     if (poseFeature.Value)
            //     {
            //         switch (poseFeature.Key)
            //         {
            //             case PosesClusterFeature.GameplayDur:
            //                 s += $"#duration= {dataLoader.GetColInfo(pid, "GameplayDur")}";
            //                 break;
            //             case PosesClusterFeature.NumCoinsCollected:
            //                 s += $"#Coin= {playerData.coin}";
            //                 break;
            //             case PosesClusterFeature.NumTreasureCollected:
            //                 s += $"#Treasure Box= {playerData.box}";
            //                 break;
            //             case PosesClusterFeature.NumBombHit:
            //                 s += $"#Bomb Hit= {playerData.bombHit}";
            //                 break;
            //             case PosesClusterFeature.NumObstaclesHit:
            //                 s += $"#Obstacle Hit= {playerData.obstacleHit}";
            //                 break;
            //         }
            //     }

            //}


            Debug.Log("TODO");
            return "";
            // return s;
        }

      

        #endregion

        private void FixedUpdate()
        {
            switch (mainReplayer.currentMode)
            {
                case ReplayMode.forward:
                    mainReplayer.PlayAFrame();
                    UpdateUserPerformance();
                    break;
                case ReplayMode.stop:
                    break;
            }
        }



        #region ClusterResult
        private void OnEnable()
        {
         
            PythonEditor.onClusterDone += SetClusterResult;
        }

        private void OnDisable()
        {
            PythonEditor.onClusterDone -= SetClusterResult;
           
        }
        void SetClusterResult(int finalNumcluster)
        {
            this.allGroupIdx = finalNumcluster;
            CreateUIForGroup();

        }
        public int GetNumClusterResult()
        {
            return this.allGroupIdx;
        }

        #endregion

        #region CallbacksFromUI
      
        //when user switch to different tab, reset this replayer
        void onTabSwitch(string previoustab, string newtab)
        {
            if(previoustab == "T2")
            {
                this.mainReplayer.PauseAndWindback();
            }
           
        }


       
        #endregion



    }

}


//public void OnAnchorSelected(int idx, AnchorType markerType)
//{
//    if (markerType == AnchorType.Chunk)
//    {
//        MChunkEvent chunkEvent = eventManager.GetSelectedChunkReplayInfo(idx);
//        this.mainReplayer.ReplayChunk(chunkEvent);
//    }
//    else
//    {
//        List<MGameEvent> gameplayEvent = eventManager.GetSelectedGameplayEvent(idx);
//        this.mainReplayer.ReplayGameplayEvent(gameplayEvent);
//    }
//}
