using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementForce = 10f;
    [SerializeField] private float moveMultiplier = 100f;
    private Vector3 movementDirection;
    bool canMove = true;


    [Header("SlopeCheck")]
    private bool isOnSlope;
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDir;

    [Header("GroundCheck")]
    [SerializeField] private float playerHeight;
    [SerializeField] private float checkOffset = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Rotating")]
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private float turnSmoothTime = 0.5f;
    private float turnSmoothVel;


    PlayerInput playerInput;
    private Rigidbody rb;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing");
        }
    }

    void Start()
    {
        
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing");
        }
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckSlope();
    }
    private void FixedUpdate()
    {
        Move();
    }


    void CheckSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + checkOffset))
        {
            if (slopeHit.normal != Vector3.up)
            {
                isOnSlope = true;
                slopeMoveDir = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);
            }
            else
            {
                isOnSlope = false;
            }
        }
        else
        {
            isOnSlope = false;
        }
    }

    void Move()
    {
        movementDirection = playerInput.horizontalInput * cam.right + playerInput.verticalInput * cam.forward;
        movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;

        if (movementDirection.magnitude > 0 && canMove)
        {
            RotatePlayer();
            if (isOnSlope)
            {
                rb.AddForce(slopeMoveDir * Time.deltaTime * movementForce * moveMultiplier, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(movementDirection * Time.deltaTime * movementForce * moveMultiplier, ForceMode.Acceleration);
            }
        }


    }
    void RotatePlayer()
    {
        float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
