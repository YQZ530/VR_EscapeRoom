using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorAnimation : MonoBehaviour
{
    private Animator doorAnimator;
    public bool isOpen = false; // Tracks the current state of the door
   
    void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        if (doorAnimator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

   
    public void ToggleDoor()
    {
        if (doorAnimator == null) return;

        isOpen = !isOpen; // Flip the state

     
        doorAnimator.SetBool("IsOpen", isOpen);

   
    }
}

