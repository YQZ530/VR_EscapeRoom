using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class PushableDrawer : MonoBehaviour
{
    
    public Vector3 movementDirection = Vector3.zero; // Local direction the drawer moves (e.g. Z+)

    private XRSimpleInteractable _interactable;
    public Transform drawerParent; // Reference to the drawer's base
    [FormerlySerializedAs("minX")] public float minDist = 0f;        // Min local position.x
    [FormerlySerializedAs("maxX")] public float maxDist = 0.4f;      // Max local position.x

    private XRBaseInteractor interactor;
    private Vector3 attachOffset;
    private bool isGrabbed = false;
    void Start()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactable.selectEntered.AddListener(OnPushStart);
        _interactable.selectExited.AddListener(OnPushExit);

        drawerParent = this.gameObject.transform.parent;
    }

    private void OnPushStart(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform.GetComponent<XRBaseInteractor>();
        attachOffset = transform.localPosition - drawerParent.InverseTransformPoint(interactor.transform.position);
        isGrabbed = true;
    }

    private void Update()
    {
        
        if (!isGrabbed || interactor == null) return;

        Vector3 worldInteractorPos = interactor.transform.position;
        Vector3 localInteractorPos = drawerParent.InverseTransformPoint(worldInteractorPos);

        Vector3 newLocalPos = transform.localPosition;
        if (movementDirection.z != 0)
        {
            float targetZ = localInteractorPos.z + attachOffset.z;
            float clampedZ = Mathf.Clamp(targetZ, minDist, maxDist);
            newLocalPos.z = clampedZ;
        }
        else if (movementDirection.x != 0)
        {
            float targetX = localInteractorPos.x + attachOffset.x;
            float clampedX = Mathf.Clamp(targetX, minDist, maxDist);
            newLocalPos.x = clampedX;
        }
        
        transform.localPosition = newLocalPos;
    }

    private void OnPushExit(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactor = null;
    }
}
