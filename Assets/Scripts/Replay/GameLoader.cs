using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DCXR.Util;
using System;
using System.Linq;

namespace DCXR.Replayer
{

    public class GameLoader : MonoBehaviour
    {
        public static event Action DoneLoadingEvent;
        public static event Action DoneLoadingGazeDiscreteEvent;
        public int totPlayer = 2;
        public CSVManager csvManager;
        public GameObject areaParent;


        private void Start()
        {
            if (csvManager == null) {
                Debug.LogError("Did not assign scriptable csv manager");
            }
            else
            {
               // csvManager.ClearAllDatabase();
               
                Loading();
            }

        }

        //string filePath = Application.dataPath + "/../../data/EscapeRoomData/";
        string filePath = Application.dataPath + "/../../EscapeRoomData/";

        #region loading 
        public async void Loading()
        {
            await LoadData();

            await Task.Delay(2000); //delay 1s so that all on start() method and initialization is done before calling event

            DoneLoadingEvent?.Invoke();
            Debug.Log("Finish Loading");
        }
       
        public async Task LoadData()
        {
            //Load player
           // await LoadPlayersData();
       
            await LoadFileToDatabase(filePath, "EyeGazeEvent");
          
            //await LoadFileToDatabase(filePath, "rawEyeGaze");
        }

        private void OnEnable()
        {
            
            PythonEditor.onEyeGazeClusterDone += LoadEyeGazeAggregateFile;
         
        }

        private void OnDisable()
        {
           PythonEditor.onEyeGazeClusterDone -= LoadEyeGazeAggregateFile;
        }

        
        async void LoadEyeGazeAggregateFile(int _)
        {
            await LoadGeneralAggregateData("FinalEyeGazeGroup");
            await LoadGeneralAggregateData("FinalEyeGazeEvent");
            DoneLoadingGazeDiscreteEvent?.Invoke();
        }

        async Task LoadGeneralAggregateData(string fileSuffix)
        {
            string targetDatabase = fileSuffix;
            if (csvManager.HasDatabase(targetDatabase))
            {
                  csvManager.RemoveDatabase(targetDatabase);
               // Debug.LogWarning($"target database {targetDatabase} already exit; skip loading");
            }
          
            await Task.Run(() =>
            {
                CSVReader reader = new CSVReader();
                string finalPath = filePath + $"{fileSuffix}.csv";
                if (!reader.LoadFromFile(finalPath))
                {
                    Debug.LogError($"Fail to load {finalPath}");
                    return;
                }
                csvManager.RemoveDatabase($"{fileSuffix}");
                csvManager.AddDatabase($"{fileSuffix}", reader);
                Debug.Log($"loaded dataset {fileSuffix}");
            });
            
        }
      
        async Task LoadFileToDatabase(string filePath, string fileSuffix)
        {
            for (int playerID = 0; playerID < totPlayer; playerID++)
            {
                string targetDatabase = $"player{playerID}_{fileSuffix}";
                if (csvManager.HasDatabase(targetDatabase))
                {
                    Debug.LogWarning($"target database {targetDatabase} already exit; skip loading");
                    continue;
                }
                else
                {
                    await Task.Run(() =>
                    {
                        CSVReader reader = new CSVReader();
                        string finalPath = filePath + $"player{playerID}_{fileSuffix}.csv";
                        if (!reader.LoadFromFile(finalPath))
                        {
                            Debug.LogError($"Fail to load player::{finalPath}");
                            return;
                        }
                        csvManager.RemoveDatabase($"player{playerID}_{fileSuffix}");
                        csvManager.AddDatabase($"player{playerID}_{fileSuffix}", reader);
                        Debug.Log($"loaded player{finalPath}");
                    });
                }

               
            }

        }



        #endregion

        #region Getter

        public List<Area> GetAreaList()
        {
            if(areaParent == null)
            {
                Debug.LogError("not assign");
                return null;
            }
           return areaParent.GetComponentsInChildren<Area>().ToList();

        }

        public List<int> GetEyeGazeDur(int playerID)
        {
            List<int> durations = new List<int>();

            string targetDatabase = $"FinalEyeGazeEvent";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("Cannot find database " + targetDatabase);
                return null;
            }
            CSVReader databaseReader = csvManager.GetDatabase(targetDatabase);

            List<string> idrows = databaseReader.GetCol("id");

            for (int row = 0; row < idrows.Count; row++)
            {
                if (databaseReader.GetCell(row, "id") == "") continue; //to avoid bugs in csv tool
                if (HelperMethod.stringToInt(databaseReader.GetCell(row, "id")) != playerID) continue;

                //int cid = HelperMethod.stringToInt(databaseReader.GetCell(row, "id"));

                int dur = Util.HelperMethod.stringToInt(databaseReader.GetCell(row, "duration"));
                
                durations.Add(dur);
            }

            return durations;
        }
        public void GetSelectedEyegazeEvent(int playerID, out List<MEyeGazeEvent> mEyeGazeEvents)
        {
            mEyeGazeEvents = new List<MEyeGazeEvent>();

            string targetDatabase = $"FinalEyeGazeEvent";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("Cannot find database " + targetDatabase);
                return;
            }
            CSVReader databaseReader = csvManager.GetDatabase(targetDatabase);

            List<string> idrows = databaseReader.GetCol("id");

            int totrow = idrows.Count;

            for (int row = 0; row < totrow; row++)
            {
                if (databaseReader.GetCell(row, "id") == "") continue; //to avoid bugs in csv tool
                if (HelperMethod.stringToInt(databaseReader.GetCell(row, "id")) != playerID) continue;

                int cid = HelperMethod.stringToInt(databaseReader.GetCell(row, "id"));
                
               
                MEyeGazeEvent eyeGazeEvent = new MEyeGazeEvent
                {
                    AreaName = databaseReader.GetCell(row, "AreaName"),
                    ParentName = databaseReader.GetCell(row, "ParentName"),
                    GazeObjectArr = HelperMethod.stringToStrList(databaseReader.GetCell(row, "GazeObject")) ,
                    startKeyframe = Util.HelperMethod.stringToInt(databaseReader.GetCell(row, "startKeyframe")),
                    endKeyframe = Util.HelperMethod.stringToInt(databaseReader.GetCell(row, "endKeyframe")),
                    dur = Util.HelperMethod.stringToInt(databaseReader.GetCell(row, "duration")),
                };
                
                //Debug.Log($"{eyeGazeEvent.AreaName}_{eyeGazeEvent.ParentName}");
                mEyeGazeEvents.Add(eyeGazeEvent);
            }
        }

    

        public MGameplayKeyFrame GetSelectedUserKeyFrame(int playerID)
        {
           
            int totframe = csvManager.GetDatabase("player" + playerID).RowCount();
            MGameplayKeyFrame keyFrame = new MGameplayKeyFrame(playerID.ToString(), 0, totframe, totframe);
            return keyFrame;

        }

        public MGameplayKeyFrame GetSelectedUserKeyFrame(int playerID, int startChunk, int endChunk)
        {

           
            CSVReader dataset = csvManager.GetDatabase($"player{playerID}_chunk");
           
            int startFrame = int.Parse(dataset.GetCell(startChunk, "startFrame"));
            //Debug.Log($"Start frame {startFrame}");
            int endFrame = int.Parse(dataset.GetCell(endChunk, "endFrame"));
            MGameplayKeyFrame keyFrame = new MGameplayKeyFrame(playerID.ToString(), startFrame, endFrame, endFrame-startFrame+1);
            
            return keyFrame;

           
         }


        public float GetSelectedUserCamOffset(int playerID)
        {
            string targetDatabase = $"player{playerID}";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("Cannot find database " + targetDatabase);
                return 0f;
            }
            CSVReader databaseReader = csvManager.GetDatabase(targetDatabase);
            return Util.HelperMethod.stringTofloat(databaseReader.GetCell(0, "camOffset"));

        }

        public PlayerData GetSelectedUserData(int playerID, int frame)
        {
           
            float totframe = csvManager.GetDatabase("player" + playerID).RowCount();
            //Debug.Log("loading player " + playerID + "  totframe" + totframe + " cur" + frame);
            if (frame >= (totframe-1)) return new PlayerData();

            string targetDatabase = $"player{playerID}";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("Cannot find database "+ targetDatabase);
                return new PlayerData();
            }
            CSVReader databaseReader = csvManager.GetDatabase(targetDatabase);

            PlayerData data = new PlayerData()
            {
                ModelPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "plyr_pos")),
                ModelRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "plyr_rot")),

                HeadPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "head_pos")),
                HeadRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "head_rot")),

                LHandPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "l_hand_pos")),
                LHandRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "l_hand_rot")),
                RHandPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "r_hand_pos")),
                RHandRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "r_hand_rot")),

                LHandLocPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "l_hand_locpos")),
                LHandLocRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "l_hand_locrot")),
                RHandLocPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "r_hand_locpos")),
                RHandLocRot = Util.HelperMethod.stringToQuaternion(databaseReader.GetCell(frame, "r_hand_locrot")),
            };
          
           
            return data;

        }

        
        public bool CheckIfUserHasGroup(int pid)
        {
            string targetDatabase = "FinalEyeGazeGroup";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("cannot find database" + targetDatabase);
                return false;
            }

            CSVReader database = csvManager.GetDatabase(targetDatabase);

            List<int> userIDS = HelperMethod.stringArrToIntList(database.GetCol("userID"));
            int u_idx = userIDS.FindIndex(elem => elem == pid);
            return u_idx != -1;
        }

        public float GetAggregate_EyeGazeData(int pid, string tarCol)
        {
            string targetDatabase = "FinalEyeGazeGroup";
            if (!csvManager.HasDatabase(targetDatabase))
            {
                Debug.LogError("cannot find database" + targetDatabase);
                return 0f;
            }

            CSVReader database = csvManager.GetDatabase(targetDatabase);

            List<int> userIDS = HelperMethod.stringArrToIntList(database.GetCol("userID"));
            int u_idx = userIDS.FindIndex(elem => elem == pid);
            float value = -1;
            if (u_idx != -1)
            {
                value = Util.HelperMethod.stringTofloat(database.GetCell(u_idx, tarCol));
            }

            return value;

        }
        
       
       

        #endregion

    }
}




  // public void GetSelectedUserEyegazeData(int playerID, int frame, ref PlayerData data)
  //       {
  //           string targetDatabase = $"player{playerID}_rawEyeGaze";
  //           if (!csvManager.HasDatabase(targetDatabase))
  //           {
  //               Debug.LogError("Cannot find database " + targetDatabase);
  //               return;
  //           }
  //           CSVReader databaseReader = csvManager.GetDatabase(targetDatabase);
  //
  //           float totframe = databaseReader.RowCount();
  //           //Debug.Log("loading player " + playerID + "  totframe" + totframe + " cur" + frame);
  //
  //           if (frame >= (totframe - 1)) return;
  //           Vector3 gazeStartPos = Util.HelperMethod.stringToVector(databaseReader.GetCell(frame, "gazeStartPos"));
  //           data.gazeStartPos = gazeStartPos;
  //
  //       }
  //
  //
  //       public List<Vector3> GetPosDataForPlayer(int playerID)
  //       {
  //           string targetDatabase = "player" + playerID;
  //           if (!csvManager.HasDatabase(targetDatabase))
  //           {
  //               Debug.LogError("Cannot find database " + targetDatabase);
  //               return new List<Vector3>();
  //           }
  //
  //           List<Vector3> player_pos = Util.HelperMethod.stringArrToVectorList(csvManager.GetDatabase(targetDatabase).GetCol("plyr_pos"));
  //           return player_pos;
  //
  //       }
  //
  //       public List<Vector3> GetEyeGazeDataForPlayer(int playerID)
  //       {
  //           List<Vector3> player_EyeGazepos = Util.HelperMethod.stringArrToVectorList(csvManager.GetDatabase($"player{playerID}_rawEyeGaze").GetCol("gazeStartPos"));
  //           return player_EyeGazepos;
  //
  //       }
  //
  //       
  //
  //       public List<Vector2> GetEventAllUsers(int chunkEvent)
  //       {
  //           List<Vector2> moments = new List<Vector2>();
  //           for (int playerID = 0; playerID < totPlayer; playerID++)
  //           {
  //               int start = int.Parse(csvManager.GetDatabase($"player{playerID}_chunk").GetCell(chunkEvent, "startFrame"));
  //               int end = int.Parse(csvManager.GetDatabase($"player{playerID}_chunk").GetCell(chunkEvent, "endFrame"));
  //               Vector2 v = new Vector2(start, end);
  //               moments.Add(v);
  //           }
  //
  //           return moments;
  //       }
  //
  //
  //    
  //
  //       public int GetTotChunk()
  //       {
  //           int playerID = 0;
  //           List<string> chunkTypeList = csvManager.GetDatabase($"player{playerID}_chunk").GetCol("chunkType");
  //           int totChunk = chunkTypeList.Count -1; //-1 bc of bug in csv manager
  //           return totChunk;
  //       }
  //
  //       public int GetTotEvents()
  //       {
  //           int totEvent = 3;
  //           Debug.LogWarning("Hard code tot event to be "+ totEvent);
  //           //int playerID = 0;
  //           //List<string> eventTypeList = csvManager.GetDatabase($"player{playerID}_event").GetCol("eventType");
  //           //int totEvent = eventTypeList.Count - 1; //-1 bc of bug in csv manager
  //           return totEvent;
  //       }








//public void GetSelectedGameEvent(int totEventCount, GameEventType tarEventType, out List<MGameEvent> mEvents)
//{
//    mEvents = new List<MGameEvent>();

//    for (int playerID = 0; playerID < totPlayer; playerID++)
//    {
//        CSVReader targetFile = csvManager.GetDatabase($"player{playerID}_event");
//        int numEventsInPlayer = targetFile.RowCount() - 1; //-1 for header
//        if (numEventsInPlayer == 0)
//        {
//            mEvents.Add(null);
//            continue;
//        }
//        MGameEvent gameplayEvent = null;

//        for (int eventIdx = 0; eventIdx < totEventCount; eventIdx++)
//        {
//            if (eventIdx >= targetFile.RowCount()) break;

//            int eventType = HelperMethod.stringToInt(csvManager.GetDatabase($"player{playerID}_event").GetCell(eventIdx, "eventType"));

//            if (eventType.Equals((int)tarEventType))
//            {
//                int startFrame = Util.HelperMethod.stringToInt(csvManager.GetDatabase($"player{playerID}_event").GetCell(eventIdx, "startFrame"));
//                int endFrame = Util.HelperMethod.stringToInt(csvManager.GetDatabase($"player{playerID}_event").GetCell(eventIdx, "endFrame"));
//                int dur = Util.HelperMethod.stringToInt(csvManager.GetDatabase($"player{playerID}_event").GetCell(eventIdx, "dur"));
//                MGameplayKeyFrame gameplayKeyFrame = new MGameplayKeyFrame(playerID.ToString(), startFrame, endFrame, dur);



//                gameplayEvent = new MGameEvent(gameplayKeyFrame, (GameEventType)eventType);
//                mEvents.Add(gameplayEvent);
//                break; // go to next player, no need to loop to nexxt event and check
//            }



//        }


//        if (gameplayEvent == null)
//        {
//            mEvents.Add(null);
//        }

//    }

//    Debug.Log("");
//}


