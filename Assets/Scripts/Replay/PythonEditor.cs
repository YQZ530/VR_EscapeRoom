
using TwoCatsCode.UPython3Pro;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Text.RegularExpressions;

public class PythonEditor : MonoBehaviour
{
    public static event Action<int> onClusterDone;
    public static event Action<int> onEyeGazeClusterDone;

    public void Call(string cmd)
    {
        string finalPath = cmd; 
        print("commands: " + finalPath);
        UPython.Instance.CallCommand(finalPath, Result);
    }

    [ShowInInspector] //debug only
    void DebugClusterResult(string result = "eyegaze-2")
    {

        Result(result);
    }

    //Result called back from Upython
    
     void Result(string result)
    {
       
        string[] parts= result.Split("-");
        string resultCode = parts[0];
        
        if(resultCode == "cluster")
        {
            string numCluster = parts[1];
            int nCluster = 0;
            int.TryParse(numCluster, out nCluster);

            Debug.Log("Get cluster result with nCluster =  "+ nCluster);
            onClusterDone?.Invoke(nCluster);
           
        }
        else if(resultCode == "eyegaze")
        {
            string numCluster = parts[1];
            int nCluster = 0;
            int.TryParse(numCluster, out nCluster);

            Debug.Log("Get eyegaze cluster result with nCluster =  " + nCluster);
            onEyeGazeClusterDone?.Invoke(nCluster);
        }
    }



    [Button("UPython Stop")]
    public void StopUpython()
    {
        UPython.Instance.Stop();
    }

    [Button("UPython Restart")]
    public void RestartUpython()
    {
        UPython.Instance.ReStart();
    }


      

}
