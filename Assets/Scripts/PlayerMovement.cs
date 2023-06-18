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

    [Header("Speed Check")]
    [SerializeField] private float walkLimit = 6f;
    private float currentSpeedLimit;

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
    PlayerCam playerCam;
    private Rigidbody rb;
    Player player;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerCam = GetComponent<PlayerCam>();
        player = GetComponent<Player>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing");
        }
    }

    void Start()
    {
        
        cam = Camera.main.transform;
        currentSpeedLimit = walkLimit;
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
        cam.position = transform.position;
        
    }
    private void FixedUpdate()
    {
        CheckSlope();
        SetSpeedLimit();
        LimitPlayerSpeed();
        if (!player.disabled)
        {
            Move();
        }
    }
    void SetSpeedLimit()
    {
        currentSpeedLimit = walkLimit;
    }
    void LimitPlayerSpeed()
    {
        Vector3 horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalSpeed.magnitude > currentSpeedLimit)
        {
            horizontalSpeed = horizontalSpeed.normalized * currentSpeedLimit;
            rb.velocity = new Vector3(horizontalSpeed.x, rb.velocity.y, horizontalSpeed.z);
        }

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
        movementDirection = playerCam.playerBody.forward * playerInput.verticalInput + playerCam.playerBody.right * playerInput.horizontalInput;
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
       

    }
}
