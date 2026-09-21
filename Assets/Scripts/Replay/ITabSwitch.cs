using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ITabSwitch  
{
    UnityAction<string, string> TabSwitchEvent { get; set; }
} 

public interface ICamPlayerSwitch
{
   // UnityAction<GameObject,string> CamPlayerSwitch { get; set; }
    UnityAction<string> CamPlayerSwitch { get; set; }
}

public interface ICamPlayerSwitch2
{
    UnityAction<List<GameObject>, string> CamPlayerSwitch2 { get; set; }
}


public interface ICamOffset
{
    UnityAction UpdateCamOffsetEvent { get; set; }
}