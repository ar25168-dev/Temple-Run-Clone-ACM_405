using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpExample : MonoBehaviour
{
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckDistance = 0.6f;


    Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    public bool IsGrounded()
    {
        // Check if a raycast downwards hits the ground layer
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
