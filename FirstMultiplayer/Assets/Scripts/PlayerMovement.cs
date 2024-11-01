
using System;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float jumpForce = 1200f;
    private Rigidbody rb;
    private Transform _transform;
    private InputController inputController;
    private Vector2 input;
    private Vector2 mouseDelta;


    private void OnDestroy()
    {
        if (inputController)
        {
            inputController.OnMoveInputUpdate -= MoveInputUpdate;
            inputController.OnMouseDeltaUpdate -= MouseDeltaUpdate;
            inputController.OnJump -= Jump;
        }
    }

    void Awake()
    {
        inputController = FindFirstObjectByType<InputController>();
         rb = GetComponent<Rigidbody>();    
        _transform = transform;

        if(inputController)
        {
            inputController.OnMoveInputUpdate += MoveInputUpdate;
            inputController.OnMouseDeltaUpdate += MouseDeltaUpdate;
            inputController.OnJump += Jump;
        }
    }

    public void SetActive(bool active, bool disableInput)
    {
        this.active = active;

        if (disableInput)
        {
            if (inputController)
            {
                inputController.OnMoveInputUpdate -= MoveInputUpdate;
                inputController.OnMouseDeltaUpdate -= MouseDeltaUpdate;
                inputController.OnJump -= Jump;
            }
        }
    }

    private void Jump()
    {
        if(active)
            rb.AddForce(Vector3.up * (jumpForce * Time.deltaTime), ForceMode.VelocityChange);
    }

    private void MouseDeltaUpdate(Vector2 vector)
    {
        if (active)
            mouseDelta = vector;    
    }

    private void MoveInputUpdate(Vector2 vector)
    {
        if (active)
            input = vector;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active) return;

        Vector3 x = _transform.right * (input.x * moveSpeed * Time.deltaTime);
        Vector3 z = _transform.forward * (input.y * moveSpeed * Time.deltaTime);

        if (input != Vector2.zero)
        {
            Vector3 rotateY = new Vector3(0, mouseDelta.x * rotationSpeed * Time.deltaTime, 0);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateY));
        }

        rb.MovePosition(rb.position + x + z);
    }
}
