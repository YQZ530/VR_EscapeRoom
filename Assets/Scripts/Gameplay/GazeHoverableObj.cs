using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(OutlineManager))]
public class GazeHoverableObj : MonoBehaviour
{
    private XRSimpleInteractable _xrSimpleInteractable;
    private XRGrabInteractable _xrGrabInteractable;
    private OutlineManager _outline;
    public string AreaName;
    public string GazeObjName;
    public string ParentName;
    void Start()
    {
        this.AreaName = this.transform.root.name;
        this.GazeObjName = this.transform.name;
        this.ParentName = this.transform.parent.name;
        this._outline = GetComponent<OutlineManager>();
        this._xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        if (_xrSimpleInteractable != null)
        {
            _xrSimpleInteractable.hoverEntered.AddListener(OnHoverEnter);
            _xrSimpleInteractable.hoverExited.AddListener(OnHoverExit);
            
        }
        
        this._xrGrabInteractable = GetComponent<XRGrabInteractable>();
        if (_xrGrabInteractable != null)
        {
            _xrGrabInteractable.hoverEntered.AddListener(OnHoverEnter);
            _xrGrabInteractable.hoverExited.AddListener(OnHoverExit);
           
        }
        
        if (_xrGrabInteractable != null)
        {
            _xrGrabInteractable.selectEntered.AddListener(OnSelectEnter);
            _xrGrabInteractable.selectExited.AddListener(OnSelectExit);
            _xrGrabInteractable.forceGravityOnDetach = false;
        }
        
    }

    private Vector3 pos;
    private Quaternion rot;
    void OnSelectEnter(SelectEnterEventArgs args)
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        transform.position = pos;
        transform.rotation =rot;
    }
     void OnHoverEnter(HoverEnterEventArgs args)
    {
        _outline.Hover();
        
    }
    public void HoverEnter()
    {
        _outline.Hover();
    }
    public void HoverExit()
    {
        _outline.HoverCancel();
    }
     void OnHoverExit(HoverExitEventArgs args)
    {
        _outline.HoverCancel();
    }
     
     
     
}
