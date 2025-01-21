using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureRecognition : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;

    // Thresholds for gestures
    public float pinchThreshold = 0.8f; // For the grab gesture
    public float swipeThreshold = 0.2f; // Movement distance for the swipe gesture
    private Vector3 initialSwipePosition;
    private bool isSwiping;

    private GrabbableObject grabbedObjectLeft;
    private GrabbableObject grabbedObjectRight;

    private void Update()
    {
        HandleGesture(leftHand, ref grabbedObjectLeft);
        HandleGesture(rightHand, ref grabbedObjectRight);
    }

    private void HandleGesture(OVRHand hand, ref GrabbableObject grabbedObject)
    {
        if (hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold)
        {
            if (grabbedObject == null)
            {
                // Try to grab an object
                grabbedObject = TryGrabObject(hand);
            }
            else
            {
                // Move the grabbed object
                grabbedObject.MoveWithHand();
            }
        }
        else
        {
            if (grabbedObject != null)
            {
                // Release the object
                grabbedObject.Release();
                grabbedObject = null;
            }
        }
    }

    private GrabbableObject TryGrabObject(OVRHand hand)
    {
        Collider[] colliders = Physics.OverlapSphere(hand.transform.position, 0.1f);
        foreach (var collider in colliders)
        {
            GrabbableObject grabbable = collider.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                grabbable.Grab(hand.transform);
                return grabbable;
            }
        }
        return null;
    }

    private void DetectSwipeGesture(OVRHand hand)
    {
        if (!isSwiping && hand.IsTracked)
        {
            initialSwipePosition = hand.transform.position;
            isSwiping = true;
        }

        if (isSwiping)
        {
            Vector3 currentPosition = hand.transform.position;
            float swipeDistance = Vector3.Distance(initialSwipePosition, currentPosition);

            if (swipeDistance > swipeThreshold)
            {
                Vector3 swipeDirection = currentPosition - initialSwipePosition;
                Debug.Log($"===Swipe Gesture Detected! Direction: {swipeDirection.normalized}");
                isSwiping = false;

                // Perform swipe logic here (e.g., rotate or move an object)
            }
        }
    }
}

