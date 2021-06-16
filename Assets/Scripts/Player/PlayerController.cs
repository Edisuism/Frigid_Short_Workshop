using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = 1;
    public float gravityScale = 1;
    [Header("Momentum")]
    public float maxMomentum = float.MaxValue;
    [Min(0)]
    public float momentumDrag = 1;
    [Min(0)]
    public float momentumPower = 1;
    [Header("Jumping")]
    public float jumpPower = 1;
    public float initialJumpSpeed;
    public float coyoteTime = 0.3f;
    public float coyoteJumpTime = 0.4f;
    public float jumpSensitivity = 4f;
    [Header("Other")]
    // Put other stuff here

    private AudioSource audioSource;
    private bool isJumping;
    private float jumpInputTime;
    private float currentJumpSensitivity;
    private bool jumpLock;
    private Vector3 verticalVelocity;
    // private CharacterController characterController;
    private KinematicCharacterController characterController;
    private float currentCoyoteTime;
    private float momentum;

    public bool IsCoyoteGrounded => currentCoyoteTime < coyoteTime;
    public bool CanJump => jumpInputTime < coyoteJumpTime;
    public bool IsJumping => isJumping;
    
    public float Momentum
    {
        get { return momentum; }
        set { momentum = Mathf.Clamp(value, 0, maxMomentum); }
    }
    
    private void Awake()
    {
        // characterController = GetComponent<CharacterController>();
        characterController = GetComponent<KinematicCharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        characterController.MovementCollisionEvent += OnMovementCollision;
    }

    private void FixedUpdate()
    {
        HandleJumping();
        HandleMovement();
    }
    
    private void OnMovementCollision(MovementCollisionContext collision, MovementContext movement)
    {
        verticalVelocity += collision.CalculateCounterVector(verticalVelocity, Vector3.up);
    }

    private void HandleMovement()
    {
        float horizontalSpeed = Input.GetAxis("Horizontal") * speed;
        float forwardSpeed = Input.GetAxis("Vertical") * speed;

        // Apply Gravity;
        verticalVelocity += Physics.gravity * (gravityScale * Time.fixedDeltaTime);

        // Don't apply gravity if not moving in the air, a Much more friendly approach.
        if (characterController.IsGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity = Vector3.zero;
        }

        Vector3 forwardVelocity = transform.forward * forwardSpeed;
        Vector3 rightVelocity = transform.right * horizontalSpeed;
        
        Vector3 instantVelocity = forwardVelocity + rightVelocity;
        Vector3 displacement = (verticalVelocity + instantVelocity) * Time.fixedDeltaTime;


        // Apply momentum, movement and gravity.
        AddMomentum(instantVelocity);
        characterController.Move(displacement);
        

        if (horizontalSpeed != 0 || forwardSpeed != 0)
        {
            audioSource.volume = UnityEngine.Random.Range(0.1f, 1f);
            audioSource.pitch = UnityEngine.Random.Range(0.1f, 1f);
            audioSource.Play();
        }

    }

    private void AddMomentum(Vector3 velocity)
    {
        Vector3 addedVelocity = velocity * (Time.fixedDeltaTime * (Momentum * momentumPower));
        characterController.velocity += addedVelocity;
        Momentum /= 1 + momentumDrag;
        VisualDebugger.Instance.Set("Momentum", "momentum", $"{Momentum:0.0000}");
    }

    private void CalculateJumpSensitivity(bool hasJumpInput)
    {
        if (hasJumpInput)
        {
            float toAdd = jumpSensitivity * Time.fixedDeltaTime;
            currentJumpSensitivity = Mathf.Clamp(currentJumpSensitivity + toAdd, 0, 1);
        }
        else
        {
            currentJumpSensitivity = 0;
        }
    }

    private void HandleJumping()
    {
        bool hasJumpInput = Input.GetButton("Jump");

        CalculateJumpSensitivity(hasJumpInput);

        // Calculate how long the player has pressed jump.
        jumpInputTime = hasJumpInput ? Mathf.Min(coyoteJumpTime, jumpInputTime + Time.fixedDeltaTime) : 0;
        
        // Calculate Coyote time
        currentCoyoteTime = characterController.IsGrounded ? 0 : Mathf.Min(coyoteTime, currentCoyoteTime + Time.fixedDeltaTime);
        
        if (hasJumpInput && !jumpLock && CanJump && IsCoyoteGrounded)
        {
            // print($"Starting to jump with lock {jumpLock} and coyote jump time {currentCoyoteTime}, jump input time {jumpInputTime}");
            isJumping = true;
            jumpLock = true;
            verticalVelocity.y += initialJumpSpeed;
            currentCoyoteTime = coyoteTime;
        }
        
        if (hasJumpInput && !CanJump)
        {
            // print("Jump locked!");
            jumpLock = true;
        }

        if (isJumping)
        {
            verticalVelocity.y += jumpPower * currentJumpSensitivity;
            
            // Stop jumping if player releases jump button or player has reached our max jump height.
            if (currentJumpSensitivity == 1 || !hasJumpInput)
            {
                // print($"Finished jumping with lock {jumpLock}");

                isJumping = false;
            }
        }

        if (!hasJumpInput)
        {
            // print("Jump unlocked");
            jumpLock = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            LevelManager.Instance.LoseLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Recharge"))
        {
            ParticleLauncher.Instance.Ammo += 100;
            Destroy(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (characterController)
        {
            characterController.MovementCollisionEvent -= OnMovementCollision;
        }
    }
}