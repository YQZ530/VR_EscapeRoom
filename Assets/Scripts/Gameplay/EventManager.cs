using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

using DCXR.Replayer;


[RequireComponent(typeof(GameLoader))]
public class EventManager : MonoBehaviour
{

    [Title("View event details", HorizontalLine = true)]
    GameLoader gameLoader;

    [TableList(ShowIndexLabels = true)]
    
    public int totEvent;
    public void Start()
    {
        if (gameLoader == null) gameLoader = this.GetComponent<GameLoader>();
       // Debug.Log("event manager: need to check predefine chunk event if have change in level  ");
      
      
    }
    private void OnEnable()
    {
        GameLoader.DoneLoadingEvent += LoadAllChunkEvent;
    }
    private void OnDisable()
    {
        GameLoader.DoneLoadingEvent -= LoadAllChunkEvent;
    }
    //[HorizontalGroup("ChunkEvent", GroupID = "1", Order = 0), Button]
    public void LoadAllChunkEvent()
    {
       //if(gameLoader != null) gameLoader.GetAllChunkEvent(out cEvents);
       Debug.Log("TODO");
    }

  
    public List<MGameEvent> GetSelectedGameplayEvent(int eventIdx)
    {
        // if (eventIdx >= event_chunkTypes.Count) {
        //     Debug.LogWarning($"Request event Idx{eventIdx} is not match with # of prefefined event{event_chunkTypes.Count} "); 
        //     return null;
        // }
        //
        // List<MGameEvent> mEvents;
        //
        // if (gameLoader == null) gameLoader = this.GetComponent<GameLoader>(); 
        // gameLoader.GetSelectedGameEvent(event_chunkTypes[eventIdx],  totEvent, out mEvents);
        // return mEvents;
        Debug.Log("TODO");
        return null;
    }

   






}




///// <summary>
/////
///// 
/////  Discrete graph
/////
///// </summary>
//PythonEditor editor;
//[Title("Plot Graph")]
//public string Description = "Select a graph type and attribute to plot";



//[InlineButton("Ini")]
//[TabGroup("Discrete Data")]
//public DiscreteAttToggle[] DiscAttributeList;
//private void Ini()
//{
//    if (DiscAttributeList != null)
//    {
//        Array.Clear(DiscAttributeList, 0, DiscAttributeList.Length);
//    }
//    DiscAttributeList = new DiscreteAttToggle[]
//    {
//        new DiscreteAttToggle( DiscreteAttribute.startKeyframe) { },
//        new DiscreteAttToggle(DiscreteAttribute.endKeyframe) {  },
//        new DiscreteAttToggle(DiscreteAttribute.eventDuration) { },

//    };
//}

//[TabGroup("Discrete Data")]
//public GraphType graphType = GraphType.BarGraph;

//[TabGroup("Discrete Data")]
//public string X_Axis = "Users";
//[InlineButton("PlotDiscreteGraph")]
//[TabGroup("Discrete Data")]
//public string Y_Axis = "Rating Scores/Time(s)";

//[TabGroup("Discrete Data")]
////public string title = "graph title";
//public void PlotDiscreteGraph()
//{
//    if (editor == null)
//    {
//        editor = this.GetComponent<PythonEditor>();
//    }
//    OutputEventDataToCSV();
//    string fileName = ConvertToCmd();
//    string cmd = "-g ";  // + "./"+ fileName;
//    editor.Call(cmd);
//}

//void OutputEventDataToCSV()
//{
//    string filePath = Application.dataPath + "/../../data/all/event.csv";

//    StreamWriter writer = new StreamWriter(filePath, true);
//    writer.WriteLine("Event,userID,levelDiff,turn,startKeyframe,endKeyframe,dur,");

//    int totEventCount = mEvents.Count;
//    int rowIdx = 0;
//    for (int ithEvent = 0; ithEvent < totEventCount; ithEvent++)
//    {
//        if (!mEvents[ithEvent].isSelected) continue;
//        //
//        //writer.WriteLine("",,,);
//        //
//        List<MGameplayKeyFrame> record = mEvents[ithEvent].mGameplayKeyFrameArr;
//        for (int r = 0; r < record.Count; r++)
//        {
//            string[] parts = record[r].userid.Split('-');
//            int userID = int.Parse(Regex.Match(parts[0], @"\d+").Value);
//            int turn = int.Parse(Regex.Match(parts[1], @"\d+").Value);
//            int level = int.Parse(Regex.Match(parts[2], @"\d+").Value);
//            writer.WriteLine($"{ithEvent},{userID},{level},{turn},{record[r].startKeyframe},{record[r].endKeyframe},{record[r].dur},");
//            rowIdx += 1;
//        }

//    }


//    writer.Close();
//    Debug.Log($"File saved to: {filePath}");

//}
//string ConvertToCmd()
//{
//    string fileName = "SimpleGraph.json";
//    //string filePath = Application.dataPath + "../../../python-graph/graphSetting/" + fileName;
//    //string json = $" {{ \"SimpleGraphSetting\": { JsonConvert.SerializeObject(DiscAttributeList)}"
//    //        + $", \"graphType\" : \"{ (int)graphType}\" "
//    //        + $", \"X_Axis\" :\"{X_Axis}\" " + $", \"Y_Axis\" :\"{Y_Axis}\" }}";

//    //try
//    //{
//    //    File.WriteAllText(filePath, json);
//    //    Debug.Log($"JSON data saved to {filePath}");
//    //}
//    //catch (Exception ex)
//    //{
//    //    Debug.LogError($"An error occurred: {ex.Message}");
//    //}

//    return fileName;
//}


/// <summary>
///
/// 
/// Continuous graph
///
/// </summary>


//[Button, HorizontalGroup("Plot")]
//public void ViewContinuousGraph()
//{
//    Debug.Log("ViewContGraph");
//}
