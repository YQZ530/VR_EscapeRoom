using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using DCXR.Graph;

namespace DCXR.Clustering
{

    #region enumDeclaration
    public enum ClusterCommand
    {
        poses,
        eyegaze,
    }

    public enum EyeGazeClusterFeature
    {
        Fixation,
        SumDur,
        StdDur,
        AvgDur,
        NumFixation,
        Tot,
    }
    public enum EyeGazeClusterMetric
    {
        Euclidean,
        HausdorffDist,

    }
    public enum EyeGazeClusterAlgor
    {
        KMean,
        PAM,
        Agglomerative,

    }
    public enum PoseClusterMetric
    {
        Euclidean,
        HausdorffDist,

    }

    public enum PoseClusterAlgor
    {
        ElasticNet,
        KMean,
        HDBSCAN,
    }
    #endregion
    public class ClusterPython : MonoBehaviour
    {
        public static event Action<int, int> SetEyeGaze_ChunkRangeEvent;
        
        public PythonEditor editor;

        //[ShowInInspector]
        List<KeyValuePair<PosesClusterFeature, bool>> selected_PoseFeatureArr;
        List<KeyValuePair<EyeGazeClusterFeature, bool>> selected_EyeGazeFeatureArr;

        public EyeGazeClusterMetric selected_EyeGaze_CMetric;
        public EyeGazeClusterAlgor selected_EyeGaze_CAlgo;

        public PoseClusterMetric selected_Pose_CMetric;
        public PoseClusterAlgor selected_Pose_CAlgo;


        public int ueyeGaze_numCluster = 3; //user setting for number of eyegaze  cluster
        public int tarEvtType = 0;
       
        public int minFixation;

        public void Start()
        {
            Init();
            //automatic set walkpath feature to be selected so that match with default UI
            this.selected_PoseFeatureArr[0] = new KeyValuePair<PosesClusterFeature, bool>(PosesClusterFeature.WalkPath, true);
            this.selected_EyeGazeFeatureArr[0] =
                new KeyValuePair<EyeGazeClusterFeature, bool>(EyeGazeClusterFeature.Fixation, true);
        }

        private void Init()
        {
            //initialize pose feature selection arr
            if (this.selected_PoseFeatureArr == null)
            {
                this.selected_PoseFeatureArr = new List<KeyValuePair<PosesClusterFeature, bool>>();

                int tot = (int)PosesClusterFeature.Tot;

                this.selected_PoseFeatureArr.Add(new KeyValuePair<PosesClusterFeature, bool>(PosesClusterFeature.WalkPath, true));
                for (int i = 1; i < tot; i++)
                {
                    this.selected_PoseFeatureArr.Add(new KeyValuePair<PosesClusterFeature, bool>((PosesClusterFeature)i, false));
                }

                
            }

            //initialize eyegaze feature selection arr
            if (this.selected_EyeGazeFeatureArr == null)
            {
                this.selected_EyeGazeFeatureArr = new List<KeyValuePair<EyeGazeClusterFeature, bool>>();
                int tot = (int)EyeGazeClusterFeature.Tot;

                for (int i = 0; i < tot; i++)
                {
                    this.selected_EyeGazeFeatureArr.Add(new KeyValuePair<EyeGazeClusterFeature, bool>((EyeGazeClusterFeature)i, false));
                }
            }


        }


        public List<KeyValuePair<PosesClusterFeature, bool>> Get_PoseClusterFeature()
        {
            return this.selected_PoseFeatureArr;
        }

        public List<KeyValuePair<EyeGazeClusterFeature, bool>> Get_GazeClusterFeature()
        {
            return this.selected_EyeGazeFeatureArr;
        }

        public void Set_EyeGazeClusterFeature(in List<bool> buttStatus)
        {
            for (int i = 0; i < buttStatus.Count; i++)
            {
                this.selected_EyeGazeFeatureArr[i] = new KeyValuePair<EyeGazeClusterFeature, bool>((EyeGazeClusterFeature)i, buttStatus[i]);
            }
        }




        [ShowInInspector]
        public void SetTarEvtType(int newVale,int dummy)
        {
            this.tarEvtType = newVale;
           // SetEyeGaze_ChunkRangeEvent?.Invoke(startChunk, endChunk);
           Debug.Log("targetEvtType = " + tarEvtType);
        }


        static string KVToString<TEnum>(List<KeyValuePair<TEnum, bool>> arr) where TEnum : struct, Enum
        {
            string s = "{";

            for (int i = 0; i < arr.Count; i++)
            {
                var p = arr[i];
                if (i != arr.Count - 1)
                {
                    s += $" \"{p.Key.ToString()}\":{JsonConvert.SerializeObject(p.Value)},";
                }
                else
                {
                    s += $" \"{p.Key.ToString()}\":{JsonConvert.SerializeObject(p.Value)}";
                    s += "}";
                }
            }
            return s;
        }

        [ShowInInspector]
        public void RunCluster(ClusterCommand command)
        {
            ParameterChecking();
            OutputToJson(command);
            string c = command == ClusterCommand.poses ? "-c" : "-er_eyec"; // so that we can match with python args.
            editor.Call(c);

            Debug.Log("RunCluster() called");
        }

        void ParameterChecking()
        {
            int totEventtype = 3;
            if(tarEvtType > totEventtype)
            {
                Debug.LogWarning($"eye  tarEvtType{tarEvtType} > totEventtype {totEventtype}");
            }
        }
        void OutputToJson(ClusterCommand command)
        {
            string fileName = "ClusterSetting.json";
            string filePath = Application.dataPath + "/../../VR-Test-python/Setting/" + fileName;
            string json = "";
            if (command == ClusterCommand.poses)
            {
                // json = " {" + $" \"Features\": {KVToString(this.selected_PoseFeatureArr)},"
                //          + $" \"Pose_Metric\": \"{this.selected_Pose_CMetric.ToString()}\","
                //      + $" \"Pose_Algo\":  \"{this.selected_Pose_CAlgo.ToString()}\","
                //        + $" \"startChunk\" : \"{ upose_startChunk-1}\","
                //       + $" \"endChunk\" : \"{ upose_endChunk-1}\","
                //      + $"\"Min_PlayerPerCluster\" : \"{ (int)min_PlayerPerCluster}\", "
                //   + $"\"NumCluster\" : \"{ (int)upose_numCluster}\" "
                // + "}";
                Debug.Log(("todo"));
            }
            else if (command == ClusterCommand.eyegaze)
            {
                json = " {" + $" \"EyeGaze_Features\": {KVToString(this.selected_EyeGazeFeatureArr)},"
                    + $" \"EyeGaze_Metric\": \"{this.selected_EyeGaze_CMetric.ToString()}\","
                     + $" \"EyeGaze_Algo\":  \"{this.selected_EyeGaze_CAlgo.ToString()}\","
                    + $" \"tarEvtType\" : \"{ tarEvtType-1}\","
                        + $" \"minFixation\" : \"{minFixation}\","
                 + $" \"NumCluster\" : \"{ (int)ueyeGaze_numCluster}\" "
               + "}";
                
            }
            else
            {
                Debug.Log("Undefine cluster command");
            }

            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"JSON data saved to {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }


        }




    }
}