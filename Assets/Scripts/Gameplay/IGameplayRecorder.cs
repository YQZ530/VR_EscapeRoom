using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEventType
{
    SearchKey =0,
    SearchBook = 1,
    SearchPlant=2
}

public class MGameEvent
{
    public MGameplayKeyFrame keyframe;
    public GameEventType eventType;
    public MGameEvent(int startTime, int endTime, GameEventType gameEventType)
    {
        this.eventType = gameEventType;
        this.keyframe = new MGameplayKeyFrame();
        this.keyframe.startKeyframe = startTime;
        this.keyframe.endKeyframe = endTime;
        this.keyframe.dur = endTime - startTime;
    }

    public MGameEvent(MGameplayKeyFrame keyframe,  GameEventType gameEventType)
    {
        this.eventType = gameEventType;
        this.keyframe = keyframe;
     
    }
}

[Serializable]
public struct MGameplayKeyFrame
{
    public string userid;
    public int startKeyframe;
    public int endKeyframe;
    public int dur;

    public MGameplayKeyFrame(string userid, int startKeyframe, int endKeyframe, int dur)
    {
        this.userid = userid;
        this.startKeyframe = startKeyframe;
        this.endKeyframe = endKeyframe;
        this.dur = dur;
    }
   
}

public struct MEyeGazeEvent
{
    public string AreaName;
    public string ParentName;
    public string GazeObject;
    public List<string> GazeObjectArr;
    public int startKeyframe;
    public int endKeyframe;
    public int dur;
}

public enum AnchorType
{
    Chunk,
    evt
}

public struct MChunkDetailEvent
{
    public string chunkName;
    public int startFrame;
    public int endFrame;

}

[Serializable]
public class MEvent
{
   // [TableColumnWidth(100,false)]
    public string name;

   // [TableList(ShowIndexLabels = true, IsReadOnly =true, NumberOfItemsPerPage =3, DrawScrollView = true, MaxScrollViewHeight = 200, MinScrollViewHeight = 100)]
    public  List<MGameplayKeyFrame> mGameplayKeyFrameArr;
     
    //[TableColumnWidth(60, true)]
    public bool isSelected;

    public MEvent()
    {
        mGameplayKeyFrameArr = new List<MGameplayKeyFrame>();
    }
}




// [Serializable]
// public class MChunkEvent : MEvent
// {
//     //chunk info go here
//     public ChunkType chunktype;
// }







public struct PlayerData
{
    public Vector3 ModelPos;
    public Vector3 HeadPos;
    public Vector3 LHandPos;
    public Vector3 RHandPos;
    public Vector3 LHandLocPos;
    public Vector3 RHandLocPos;


    public Quaternion ModelRot;
    public Quaternion HeadRot;
    public Quaternion LHandRot;
    public Quaternion RHandRot;
    public Quaternion LHandLocRot;
    public Quaternion RHandLocRot;

    public Vector3 gazeStartPos;

}

public interface IGameplayRecorder
{
    UnityAction<List<PlayerData>, int, float> SaveGameplayRecord { set; get; }
}

public struct MEyeGazeData
{
    public Vector3 gazePos;
}
public interface IEyeGazeRecorder
{
    UnityAction<List<MEyeGazeData>> SaveRawEyeGazeRecord { set; get; }
}