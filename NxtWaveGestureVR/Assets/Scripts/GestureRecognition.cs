using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureRecognition : MonoBehaviour
{
    // OVR Hand tracking references
    public OVRHand leftHand;
    public OVRHand rightHand;

    // Thresholds for gestures
    public float pinchThreshold = 0.8f; // For the grab gesture
    public float overlapSphereConst = 0.1f; // For collider search radius
    public float swipeThreshold = 0.05f; // Movement distance for the swipe gesture
    private float rotationSpeed = 100000f; // speed to rotate per swipe

    private Vector3 previousSwipePosition;
    private bool isSwiping;

    public Transform targetObject;  // The object to rotate
    private Quaternion targetRotation;

    [SerializeField]private SwipeableObject swipeable;
    private GrabbableObject grabbedObjectLeft;

 

    private void Update()
    {
        // Smoothly rotate the object toward the target rotation
        if (targetObject != null)
        {
            targetObject.rotation = Quaternion.Lerp(targetObject.rotation, targetRotation, Time.deltaTime * 5f);
        }

        //Use Left hand to Pinch and Grab
        HandleGesture(leftHand, ref grabbedObjectLeft);

        // Use Right hand to Swipe and Rotate (only if hand is touching the object)
        if (swipeable != null && swipeable.IsHandTouching)
        {
            DetectSwipeGesture(rightHand, Vector3.up);
        }
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
        Collider[] colliders = Physics.OverlapSphere(hand.transform.position, overlapSphereConst);
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
        if (hand.IsTracked)
        {
            Vector3 currentPosition = hand.transform.position;

            // Detect swipe direction only if the movement is large enough
            float swipeDistance = Vector3.Distance(previousSwipePosition, currentPosition);

            if (swipeDistance > swipeThreshold)
            {
                // Calculate swipe direction based on movement between frames
                Vector3 swipeDirection = currentPosition - previousSwipePosition;
            if(swipeDirection.x >(2 * swipeThreshold))
                {
                    RotateObject(swipeDirection, -swipeAxis);
                }
                // Rotate the object based on the swipe direction

                // Update the previous position for the next frame
                previousSwipePosition = currentPosition;
            }
            else if (!isSwiping)
            {
                // Set initial swipe position when the swipe starts
                previousSwipePosition = currentPosition;
                isSwiping = true;
            }
        }
    }

    private void RotateObject(Vector3 swipeDirection, Vector3 axis)
    {
        if (targetObject != null)
        {
          // Normalize the swipe direction to avoid extreme movement
        float rotationAmount = swipeDirection.magnitude * rotationSpeed * Time.deltaTime;

            // Calculate the target rotation based on the swipe axis and direction
            targetRotation = targetObject.rotation * Quaternion.AngleAxis(rotationAmount, axis);

            // Start the coroutine to change the object's color
            StartCoroutine(ChangeObjectColorForSeconds(Color.green, 2f));
        }
        else
        {
            Debug.LogWarning("No target object assigned for rotation.");
        }
    }
    private IEnumerator ChangeObjectColorForSeconds(Color color, float duration)
    {
        if (swipeable != null)
        {
            Renderer renderer = swipeable.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Save the original color
                Color originalColor = renderer.material.color;

                // Change to the new color
                renderer.material.color = color;

                // Wait for the specified duration
                yield return new WaitForSeconds(duration);

                // Revert to the original color
                renderer.material.color = originalColor;
            }
        }
        else
        {
            Debug.LogWarning("Swipeable object is not assigned.");
        }
    }

}

