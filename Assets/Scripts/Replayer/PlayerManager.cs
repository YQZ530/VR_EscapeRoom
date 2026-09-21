
using System.Collections.Generic;
using UnityEngine;
using DCXR.PathDrawing;
using Sirenix.OdinInspector;
using DCXR.Graph;
using System.Linq;
//using DCXR.UI;

namespace DCXR.Replayer
{
    public class PlayerManager : MonoBehaviour
    {
        public List<GameObject> playerlist; 
        List<Material> line_materials;
        Material default_material;
        
        int maxNumMaterial = 27;
        DiscreteDataLoader dataLoader;
        GameLoader gameLoader;
        public GameObject playerPrefab;
        public Transform playersHolder;

       
        void Start()
        {
            if (playersHolder == null ) Debug.LogError("Did not assign");

            playerlist = new List<GameObject>();
            dataLoader = GetComponentInChildren<DiscreteDataLoader>();
            gameLoader = GetComponent<GameLoader>();    
            LoadLineMaterial();
        }

        public void OnEnable()
        {
            GameLoader.DoneLoadingEvent += InitializedAllPlayerInfo;
            //PythonEditor.onEyeGazeClusterDone += LoadEyegazeToPlayer;
            GameLoader.DoneLoadingGazeDiscreteEvent += LoadEyegazeToPlayer;
        }

        public void OnDisable()
        {
            GameLoader.DoneLoadingEvent -= InitializedAllPlayerInfo;
            //PythonEditor.onEyeGazeClusterDone -= LoadEyegazeToPlayer;
            GameLoader.DoneLoadingGazeDiscreteEvent -= LoadEyegazeToPlayer;
        }
        #region Initialization
        void LoadLineMaterial()
        {
            line_materials = new List<Material>();
            for (int i = 0; i < maxNumMaterial; i++)
            {
                string materialName = $"Line{i}";
                //Debug.Log("try to get " + materialName + " in "+ Application.dataPath + "Materials/" + materialName);
                var line_material = Resources.Load("Materials/" + materialName, typeof(Material)) as Material;
                line_materials.Add(line_material);
            }

            default_material = Resources.Load("Materials/defaultMaterial", typeof(Material)) as Material;
          
        }
        public Color GetGroupMaterialColor(int gid)
        {
            if (gid >= line_materials.Count)
            {
                Debug.LogWarning($"gid {gid} is >> than # of line materials");
                return default_material.color;
            }
          return  line_materials[gid].color;
        }

        void InitializedAllPlayerInfo()
        {
            for (int pid = 0; pid < gameLoader.totPlayer; pid++)
            {
                GameObject player = GameObject.Instantiate(playerPrefab, playersHolder);
                player.SetActive(true);
                player.name = "Player" + pid.ToString();
                player.GetComponent<RePlayerStatus>().playerID = pid;
                playerlist.Add(player);

               
                var drawScript = player.GetComponent<DrawWalkPath>();
               
                Debug.Log($"Done Add player {pid}");
            }
        }

        #endregion


        public void LoadEyegazeToPlayer()
        {
            for (int playerID = 0; playerID < playerlist.Count; playerID++)
            {
                List<MEyeGazeEvent> mEyeGazeEvents;
                gameLoader.GetSelectedEyegazeEvent(playerID, out mEyeGazeEvents);
                DrawWalkPath draw = playerlist[playerID].GetComponent<DrawWalkPath>();
                draw.UpdateEyeGazeEventAndRedraw( playerID, mEyeGazeEvents);
            }
            
            
            int min; int max;
            CalEyeGazeGroupMinMax(out min, out max);
            for (int playerID = 0; playerID < playerlist.Count; playerID++)
            {
                DrawWalkPath draw = playerlist[playerID].GetComponent<DrawWalkPath>();
                draw.simEyeGazePathDrawer.SetMinMaxDur(min, max);
            }
            Debug.Log("Load gaze data to drawer");
        }
        
        void CalEyeGazeGroupMinMax(out int min, out int max)
        {
            min = int.MaxValue;
            max = int.MinValue;

            //min max are based on all user performance
            for (int playerID = 0; playerID < playerlist.Count; playerID++)
            {
                List<int> dur = gameLoader.GetEyeGazeDur(playerID);
                int tempMin = dur.Min();
                int tempMax = dur.Max();
                min = tempMin < min ? tempMin : min;
                max = tempMax > max ? tempMax : max;

            }
        }
 
        public List<int> GetPlayerIDsFromGroup(int gid, int allGroupIDx)
        {
            if (playerlist == null || playerlist.Count == 0)
            {
                Debug.LogError("[Error] player list in player manager have not been initialized");
                return null;
            }

            if (allGroupIDx <= 0)
            {
                Debug.LogError($"[Error] all Group IDX is invalid: {allGroupIDx}");
                return null;
            }
            List<int> playerIDs = new List<int> ();
            if (gid == allGroupIDx)  //gid =  show all group index, add all players to return list
            {
                for(int i = 0; i < playerlist.Count; i++)
                {
                    playerIDs.Add(i);
                }

            }
            else
            {
                playerIDs = dataLoader.GetUserList(gid);
            }

         
            return playerIDs;

        }

       

        public void ShowGaze_AllGroup(in List<bool> levelOfDetailOptions, Material defaultEyegazeMat)
        {
            //2. change all players' color and their eyegaze path color
            for (int playeridx = 0; playeridx < playerlist.Count; playeridx++)
            {
                DrawWalkPath draw = playerlist[playeridx].GetComponent<DrawWalkPath>();
                int groupID = dataLoader.GetColInfo(playeridx);

                //2a first update duration point in draw 

                if (groupID < 0)
                {
                    draw.ShowHideSimEyeGazePath(true, levelOfDetailOptions, defaultEyegazeMat);
                    Debug.LogWarning($"player{playeridx} has neg group id, show default line color instead");
                }
                else
                    draw.ShowHideSimEyeGazePath(true, levelOfDetailOptions, line_materials[groupID]);
                
                // draw.OnPointSizeChange();
                // draw.OnLineWidthChange();
            }

            Debug.Log("SHOW ALL GROUP");
        }
        
        public void ShowGaze_SpecifixGroup(int tarGroupID, in List<bool> levelOfDetailOptions)
        {
          
            for (int playerID = 0; playerID < playerlist.Count; playerID++)
            {
                DrawWalkPath draw = playerlist[playerID].GetComponent<DrawWalkPath>();
                int groupID = dataLoader.GetColInfo(playerID);
                if (groupID == tarGroupID)
                {
                    //2. update duration point in draw 
                   
                    draw.ShowHideSimEyeGazePath(true, levelOfDetailOptions, null); 
                }
                else
                {
                    draw.ShowHideSimEyeGazePath(false);
                }

                // draw.OnPointSizeChange();
                // draw.OnLineWidthChange();
            }

        }
        
        
        
        public void ShowGaze_SpecificGroupAndUser(int tGroupID, int targetPlayerIDx, in List<bool> levelOfDetailOptions )
        {
            DrawWalkPath draw = playerlist[targetPlayerIDx].GetComponent<DrawWalkPath>();
            draw.ShowHideSimEyeGazePath(true, levelOfDetailOptions, null);

            Debug.Log("SHOW specific user");
        }

        public bool CheckValidGroupAndUser(int targetGroup, int targetPlayerIDx, int allGroupIdx)
        {
            if(allGroupIdx <= 0)
            {
                Debug.LogError($"[Error] all Group IDX is invalid: {allGroupIdx}");
                return false;
            } 
            if (targetGroup > allGroupIdx)
            {
                Debug.LogError($"[Error] targetGroup IDX  {targetGroup} > allGroupIdx{allGroupIdx}");
                return false;
            }
            
            
            List<int> playerIDs = dataLoader.GetUserList(targetGroup);
            if (playerIDs.Contains(targetPlayerIDx))
            {
                return true;
            }

            return false;
          

        }
        
        





        public void ResetAllPlayer()
        {
            if (playerlist == null || playerlist.Count < 1) return;
            foreach (GameObject player in playerlist)
            {
              
                player.GetComponent<DrawWalkPath>().ShowHideSimEyeGazePath(false);

                //player.SetActive(false);
            }
            
             Debug.Log("ResetAllPlayer");
            //Debug.Log("TODO");
        }

        public void SetAllPlayerActive()
        {
            if (playerlist == null || playerlist.Count < 1) return;
            foreach (GameObject player in playerlist)
            {
                player.SetActive(true);
            }
        }
    }
}



//

//public void DisplayPlayerStatus(UnityEngine.UIElements.ToggleButtonGroupState state)
//{
//    foreach (GameObject re in playerlist)
//    {
//        DrawWalkPath draw = re.GetComponent<DrawWalkPath>();


//        draw.ShowHideWalkPath(state[0]);
//        draw.ShowHideWalkRotation(state[1]);
//        draw.ShowHideEyeGazePath(state[2]);
//    }

//}



//walk path only
//public void ShowWalkPath_SpecifixGroup(int targetGroupID, bool isShowAll)
//{
//    //if show all users walk path as in groups
//    if (isShowAll)
//    {
//        for (int i = 0; i < playerlist.Count; i++)
//        {
//        DrawWalkPath draw = playerlist[i].GetComponent<DrawWalkPath>();
//        int groupID = dataLoader.GetGroupID(i);


//        draw.ShowHidePath(true, line_materials[groupID].color);
//        ChangePlayerMaterial(playerlist[i], line_materials[groupID]);

//        }
//        return;
//    }

//    //otherwise, show specific  group users' walk path
//    for (int i = 0; i < playerlist.Count; i++)
//    {
//        DrawWalkPath draw = playerlist[i].GetComponent<DrawWalkPath>();
//        int groupID = dataLoader.GetGroupID(i);
//        if (groupID == targetGroupID)
//        {

//            draw.ShowHidePath(true, line_materials[groupID].color);
//            ChangePlayerMaterial(playerlist[i], line_materials[groupID]);
//        }
//        else //hide other group ppl
//        {
//            ChangePlayerMaterial(playerlist[i], default_material);
//            draw.ShowHidePath(false, line_materials[groupID].color);

//        }
//    }
//}