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
    public float rotationAngle = 45f; // Degrees to rotate per swipe

    private Vector3 initialSwipePosition;
    private bool isSwiping;

    public Transform targetObject;  // The object to rotate

    private GrabbableObject grabbedObjectLeft;

    private void Update()
    {
        //Use Left hand to Pinch and Grab
        HandleGesture(leftHand, ref grabbedObjectLeft);

        // Use Right hand to Swipe and Rotate
        DetectSwipeGesture(rightHand, Vector3.up);
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

    private void DetectSwipeGesture(OVRHand hand, Vector3 swipeAxis)
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

                // Normalize the swipe direction and check if it aligns with the swipe axis
                float dotProduct = Vector3.Dot(swipeDirection.normalized, swipeAxis);

                if (dotProduct > 0.15f) // Ensure the swipe aligns with the specified axis
                {
                    RotateObject(swipeAxis);
                }

                isSwiping = false; // Reset swipe detection
            }
        }
    }

    private void RotateObject(Vector3 axis)
    {
        if (targetObject != null)
        {
            targetObject.Rotate(axis, rotationAngle, Space.World);
        }
        else
        {
            Debug.LogWarning("No target object assigned for rotation.");
        }
    }
}

