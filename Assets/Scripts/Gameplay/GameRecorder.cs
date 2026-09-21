using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using DCXR.GameLogic;
using DCXR.Util;


namespace DCXR.Recorder
{
    public class GameRecorder : MonoBehaviour
    {
        List<PlayerData> playerRecords;
        public int playerID;
        private float camOffset;
        private List<MEyeGazeEvent> eyeGazeEvents;

        public List<MGameEvent> eventRecords;
        private List<MEyeGazeData> rawEyeGazeData;

        private void Start()
        {
            //var chunkObjs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<IChunkRecorder>();
            ////var chunkObjs = FindObjectsByType<MonoBehaviour>().OfType<IChunkRecorder>();
            //foreach (var r in chunkObjs)
            //{
            //    r.sendChunkRecord += SaveChunkRecord;
            //}


            var player = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameplayRecorder>();
            foreach (var r in player)
            {
                r.SaveGameplayRecord += SaveGamePlayRecord;
            }


            var eyegazeRecorder = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEyeGazeRecorder>();
            foreach (var r in eyegazeRecorder)
            {
                r.SaveRawEyeGazeRecord += SaveRawEyeGazeRecord;
            }

            

            var player2 = FindObjectsByType<PlayerEventManager>(FindObjectsSortMode.None);
            foreach (var r in player2)
            {
                r.SaveGameplayEvent += SaveGamePlayEvent;
            }

        }

        private void OnEnable()
        {
            GameManager.onOutputData += OutputAll;
            
        }

        private void OnDisable()
        {
            GameManager.onOutputData -= OutputAll;
        }

       

        void SaveGamePlayRecord(List<PlayerData> playerRecords, int playerID, float camOffset)
        {
            this.playerRecords = playerRecords;
            this.playerID = playerID;
            this.camOffset = camOffset;
        }
        void SaveGamePlayEvent(List<MGameEvent> eventRecords, List<MEyeGazeEvent> eyeGazeEvents)
        {
            this.eyeGazeEvents = eyeGazeEvents;
            this.eventRecords = eventRecords;
        }

        void SaveRawEyeGazeRecord(List<MEyeGazeData> rawEyeGazeData)
        {
            this.rawEyeGazeData = rawEyeGazeData;
        }
        void OutputAll()
        {
            
            OutputPlayerData();
            OutputEventData();
            OutputEyeGazeEvent();
            OutputRawEyeGazeData();

            Reset();
        }
        private void Reset()
        {
            
            this.playerRecords.Clear();
            
            this.eyeGazeEvents.Clear();
            this.rawEyeGazeData.Clear();

            
            Debug.Log("reset game recorder");
        }
        

        void OutputEyeGazeEvent()
        {
            string finalPath = Application.dataPath + "/../../EscapeRoomData/" + $"player{this.playerID}_EyeGazeEvent.csv";
            StreamWriter sw = new StreamWriter(finalPath);
            string title = "AreaName,ParentName,GazeObject,startKeyframe,endKeyframe,duration,";

            sw.WriteLine(title);
            foreach (var eyeRecord in eyeGazeEvents)
            {
                string s = $"{eyeRecord.AreaName},{eyeRecord.ParentName},{eyeRecord.GazeObject}, {eyeRecord.startKeyframe},{eyeRecord.endKeyframe},{eyeRecord.dur},";
                sw.WriteLine(s);
            }
            sw.Close();
            Debug.Log("Save to " + finalPath);
        }

        void OutputEventData()
        {

            string finalPath = Application.dataPath + "/../../EscapeRoomData/" + $"player{this.playerID}_event.csv";
            StreamWriter sw = new StreamWriter(finalPath);
            string title = "eventType,startFrame,endFrame,dur,";
            sw.WriteLine(title);
            foreach (var eventRecord in eventRecords)
            {
                
                string s = $"{(int)eventRecord.eventType},{eventRecord.keyframe.startKeyframe},{eventRecord.keyframe.endKeyframe},{eventRecord.keyframe.dur},";
                sw.WriteLine(s);
            }
            sw.Close();
            Debug.Log("Save to " + finalPath);
        }

        void OutputRawEyeGazeData()
        {
            if (rawEyeGazeData == null) return;

            string finalPath = Application.dataPath + "/../../EscapeRoomData/" + $"player{this.playerID}_rawEyeGaze.csv";
            StreamWriter sw = new StreamWriter(finalPath);
            sw.WriteLine("frame,gazeStartPos,");

            for (int i = 0; i < rawEyeGazeData.Count; i++)
            {
                var record = rawEyeGazeData[i];
                string s = $"{i},{Util.HelperMethod.Vector3ToString(record.gazePos)},";
                sw.WriteLine(s);
            }
            sw.Close();
            Debug.Log("Save to " + finalPath);

        }

        void OutputPlayerData()
        {
            if (playerRecords == null) return;
            string finalPath = Application.dataPath + "/../../EscapeRoomData/" + $"player{this.playerID}.csv";
            while (File.Exists(finalPath))
            {
                this.playerID += 1;
                finalPath = Application.dataPath + "/../../EscapeRoomData/" + $"player{this.playerID}.csv";
            }

           
            StreamWriter sw = new StreamWriter(finalPath);
            sw.WriteLine("frame,plyr_pos,head_pos,l_hand_pos,r_hand_pos,l_hand_locpos,r_hand_locpos," +
                "plyr_rot,head_rot,l_hand_rot,r_hand_rot,l_hand_locrot,r_hand_locrot,camOffset,");

            for (int i = 0; i < playerRecords.Count; i++)
            {
                var record = playerRecords[i];
                string s = $"{i},{Util.HelperMethod.Vector3ToString(record.ModelPos)},{Util.HelperMethod.Vector3ToString(record.HeadPos)}," +
                    $"{Util.HelperMethod.Vector3ToString(record.LHandPos)},{Util.HelperMethod.Vector3ToString(record.RHandPos)}," +
                    $"{Util.HelperMethod.Vector3ToString(record.LHandLocPos)},{Util.HelperMethod.Vector3ToString(record.RHandLocPos)},";
                s += $"{Util.HelperMethod.QuaterionToString(record.ModelRot)},{Util.HelperMethod.QuaterionToString(record.HeadRot)}," +
                    $"{Util.HelperMethod.QuaterionToString(record.LHandRot)},{Util.HelperMethod.QuaterionToString(record.RHandRot)}," +
                    $"{Util.HelperMethod.QuaterionToString(record.LHandLocRot)},{Util.HelperMethod.QuaterionToString(record.RHandLocRot)},";

                if (i == 0) s += $"{this.camOffset},";


                sw.WriteLine(s);
            }
            sw.Close();
            Debug.Log("Save to " + finalPath);

        }


    }

}