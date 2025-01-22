using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    [SerializeField] private Renderer objectRenderer;
    public bool IsHandTouching { get; private set; } = false; // Tracks touch state

    private void Awake()
    {
        SetColor(Color.red);
    }

    // Handle touch detection using trigger colliders
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            IsHandTouching = true;
            SetColor(Color.yellow);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            IsHandTouching = false;
            SetColor(Color.red);
        }
    }

    // Adjust color of the interactive object
    private void SetColor(Color color)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = color;
        }
    }
}

