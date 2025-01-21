using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    private bool isGrabbed = false;
    private bool isTouchingHand = false;
    private Rigidbody rb;
    [SerializeField]private Renderer objectRenderer;
    private Vector3 positionOffset;
    private Quaternion rotationOffset;
    private Transform handTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetColor(Color.red);
    }

    public void Grab(Transform hand)
    {
        isGrabbed = true;
        handTransform = hand;

        // Calculate and store the offset
        positionOffset = transform.position - hand.position;
        rotationOffset = Quaternion.Inverse(hand.rotation) * transform.rotation;

        rb.isKinematic = true; // Disable physics while grabbing

        SetColor(Color.green); // Change color to green when grabbed
    }

    public void Release()
    {
        isGrabbed = false;
        handTransform = null;
        rb.isKinematic = false; // Re-enable physics

        // Change color back to yellow if still touching, otherwise red
        SetColor(isTouchingHand ? Color.yellow : Color.red);
    }

    public void MoveWithHand()
    {
        if (isGrabbed && handTransform != null)
        {
            // Continuously adjust the position and rotation to keep the object close to the hand
            Vector3 targetPosition = handTransform.position + positionOffset;
            Quaternion targetRotation = handTransform.rotation * rotationOffset;

            // Smoothly update the position and rotation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            isTouchingHand = true;

            // Change color to yellow if not already grabbed
            if (!isGrabbed)
            {
                SetColor(Color.yellow);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            isTouchingHand = false;

            // Change color back to red if not grabbed
            if (!isGrabbed)
            {
                SetColor(Color.red);
            }
        }
    }
    private void SetColor(Color color)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = color;
        }
    }
}
