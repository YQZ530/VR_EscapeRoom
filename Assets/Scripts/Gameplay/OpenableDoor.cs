using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class OpenableDoor : MonoBehaviour
{
    // [Tooltip("The transform that acts as the hinge for the door.")]
    // public Transform doorPivot;
    //
    // [Tooltip("The minimum rotation angle of the door when it's closed.")]
    // public float minAngle = 0f;
    //
    // [Tooltip("The maximum rotation angle of the door when it's fully open.")]
    // public float maxAngle = 120f;
    //
    // [Tooltip("The axis around which the door rotates in the doorPivot's local space.")]
    // public Vector3 rotationAxis = Vector3.up; // Typically Vector3.up for a door
    //
    // [Tooltip("Set to 1 for left-swinging doors (0 to 120). Set to -1 for right-swinging doors (0 to -120).")]
    // public int rotationDirection = 1; // 1 for standard, -1 for reverse swing
    //
    //private XRBaseInteractor interactor = null;
    // private Quaternion initialDoorPivotRotation;
    // private Vector3 initialHandDirectionLocal; // Hand direction relative to pivot's initial orientation
    //
    // [Tooltip("The maximum distance from the pivot where pulling has an effect.")]
    // public float maxPullDistance = 0.5f; // In meters, adjust as needed
    // public float pullSensitivity = 50f; // Adjust this value in the Inspector
    // private float initialHandDistance; // Distance of hand from pivot at grab time
    private OpenDoorAnimation _openDoorAnimation;
    void Start()
    {
        //if(doorPivot == null) Debug.LogError("Did not assign door pivot");
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
        _openDoorAnimation = this.transform.parent.GetComponent<OpenDoorAnimation>();
        if(_openDoorAnimation == null) Debug.LogError("Did not assign script");
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("onGrab");
       _openDoorAnimation.ToggleDoor();
        
    }

    // void OnGrab(SelectEnterEventArgs args)
    // {
    //     interactor = args.interactorObject.transform.GetComponent<XRBaseInteractor>();
    //     if (interactor == null) return;
    //
    //     // Store the door's initial rotation when grabbed
    //     initialDoorPivotRotation = doorPivot.localRotation;
    //
    //     // Calculate the initial hand direction relative to the doorPivot's initial orientation
    //     // This is crucial for consistent rotation
    //     Vector3 handPosition = interactor.transform.position;
    //     Vector3 pivotPosition = doorPivot.position;
    //     Vector3 worldHandDirection = handPosition - pivotPosition;
    //
    //     // Project the world hand direction onto the plane perpendicular to the rotationAxis
    //     // and transform it into the doorPivot's local space.
    //     // This ensures we're calculating rotation around the correct axis.
    //     initialHandDirectionLocal = Quaternion.Inverse(doorPivot.rotation) * worldHandDirection;
    //     initialHandDirectionLocal = Vector3.ProjectOnPlane(initialHandDirectionLocal, rotationAxis).normalized;
    //     
    //     initialHandDistance = Vector3.Distance(handPosition, pivotPosition);
    //
    // }
    //
    void OnRelease(SelectExitEventArgs args)
    {
        //interactor = null;
    }
    //
    // void Update()
    // {
    //     if (interactor == null) return;
    //
    //     // Get the current hand position and transform it into the doorPivot's initial local space.
    //     Vector3 currentHandPosition = interactor.transform.position;
    //     Vector3 pivotPosition = doorPivot.position;
    //     Vector3 worldCurrentHandDirection = currentHandPosition - pivotPosition;
    //
    //     Vector3 currentHandDirectionLocal = Quaternion.Inverse(doorPivot.rotation) * worldCurrentHandDirection;
    //     currentHandDirectionLocal = Vector3.ProjectOnPlane(currentHandDirectionLocal, rotationAxis).normalized;
    //
    //     // Calculate the angle difference between the initial and current hand directions.
    //     // We use the local rotationAxis for the signed angle.
    //     float angleDelta = rotationDirection * Vector3.SignedAngle(initialHandDirectionLocal, currentHandDirectionLocal, rotationAxis);
    //
    //     float currentHandDistance = Vector3.Distance(currentHandPosition, pivotPosition);
    //     float distanceDelta = currentHandDistance - initialHandDistance;
    //     // Map distanceDelta to an angle contribution
    //     // We want pulling away to open the door more.
    //     // Clamp distanceDelta to a reasonable range relative to maxPullDistance
    //     float clampedDistanceDelta = Mathf.Clamp(distanceDelta, -maxPullDistance, maxPullDistance);
    //
    //     // Normalize distanceDelta to a 0-1 range (or -1 to 1) relative to maxPullDistance
    //     // and then scale by pullSensitivity
    //     float pullAngleContribution = (clampedDistanceDelta / maxPullDistance) * pullSensitivity * rotationDirection;
    //
    //     // Get the current door angle from its initial rotation
    //     float currentLocalAngle = GetAngleFromQuaternion(initialDoorPivotRotation, rotationAxis);
    //
    //     // Combine angular and pull contributions
    //     float newAngle = currentLocalAngle + angleDelta + pullAngleContribution;
    //     
    //     
    //     
    //     float targetAngle =  Mathf.Clamp(newAngle, minAngle, maxAngle);
    //
    //     // Apply the new rotation to the door pivot.
    //     // We create a new quaternion from the target angle around the local rotationAxis.
    //     doorPivot.localRotation = Quaternion.AngleAxis(targetAngle, rotationAxis);
    // }
    //
    // // Helper function to extract a specific angle from a Quaternion around a given axis
    // // This assumes the rotation is primarily around the specified axis.
    // // For more complex rotations, this might need refinement.
    // private float GetAngleFromQuaternion(Quaternion rot, Vector3 axis)
    // {
    //     // Convert to Euler and extract the component based on the axis.
    //     // This is still using Euler internally for the *initial* state, but we're careful.
    //     Vector3 euler = rot.eulerAngles;
    //     float angle = 0f;
    //     if (axis == Vector3.up)
    //     {
    //         return NormalizeAngle(euler.y);
    //     }
    //     else if (axis == Vector3.right)
    //     {
    //         return NormalizeAngle(euler.x);
    //     }
    //     else if (axis == Vector3.forward)
    //     {
    //         return NormalizeAngle(euler.z);
    //     }
    //     // Adjust angle for negative ranges, if needed.
    //     // If your minAngle is negative, this normalization needs to be smarter.
    //     // For example, if maxAngle is 0 and minAngle is -120,
    //     // an euler.y of 300 should be interpreted as -60.
    //     if (angle > 180f) // Unity's eulerAngles are 0-360. Convert to -180 to 180 for easier mental mapping.
    //     {
    //         angle -= 360f;
    //     }
    //     
    //     return angle;
    // }
    //
    // // Normalizes an angle to be between -180 and 180 or 0 and 360, depending on context.
    // // Here, 0-360 is generally safer for clamping.
    // private float NormalizeAngle(float angle)
    // {
    //     while (angle > 360) angle -= 360;
    //     while (angle < 0) angle += 360;
    //     return angle;
    // }
    //
    //
}
