using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public InputAction playerControls;

    Vector2 moveDirection = Vector2.zero;

    public DialogueActivator Interactable { get; internal set; }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        moveDirection = playerControls.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        while (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }
}
