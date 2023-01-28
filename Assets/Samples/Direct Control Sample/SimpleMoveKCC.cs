using nickmaltbie.OpenKCC.Character;
using nickmaltbie.OpenKCC.Character.Config;
using nickmaltbie.OpenKCC.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

// IGetKCCConfig and IGetKCCGrounded are required for the KCCMovenentEngine Abstraction
public class SimpleMoveKCC : MonoBehaviour
{
    [SerializeField]
    private Vector3 Velocity;

    [SerializeField]
    private bool useTestInput = false;

    [SerializeField]
    private Vector2 testInput;

    [SerializeField]
    private Vector3 gravity = Physics.gravity;

    [SerializeField]
    private float RunSpeed = 5f;

    [SerializeField]
    private float JumpForce = 5f;

    [SerializeField]
    private InputActionReference movementReference;

    [SerializeField]
    private InputActionReference jumpInputReference;

    private InputAction playerMovement;
    private InputAction jumpInput;

    [SerializeField]
    private KCCGroundedState groundedState = new KCCGroundedState();

    public IKCCGrounded kccGrounded => groundedState;

    [SerializeField]
    private KCCMovementEngine _movementEngine;

    private void Awake()
    {
        playerMovement = movementReference.ToInputAction();
        playerMovement.Enable();

        jumpInput = jumpInputReference.ToInputAction();
        jumpInput.Enable();

    }

    void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        var playerInput = playerMovement.ReadValue<Vector2>();

        var desiredVelocity =
            useTestInput ?
                new Vector3(testInput.x, 0, testInput.y) : new Vector3(playerInput.x, 0, playerInput.y);

        // Multiply Planar Input by Run Speed
        desiredVelocity *= RunSpeed;

        // Direct Control Over Planar Movement
        Velocity.x = desiredVelocity.x;
        Velocity.z = desiredVelocity.z;

        // Apply Jump Velocity if Jump Input is Triggered
        if (_movementEngine.GroundedState.OnGround && jumpInput.triggered)
        {
            Velocity.y = JumpForce;
        }

        // Apply Movement to Character based on Calculated Velocity
        ApplyMovement(Velocity, deltaTime);

        // Update Y Velocity Based on Grounded State
        Velocity.y = _movementEngine.GroundedState.StandingOnGround ? 0 : Velocity.y + gravity.y * deltaTime;
    }

    void ApplyMovement(Vector3 velocity, float deltaTime)
    {
        // MovePlayer based on custom velocity
        _movementEngine.MovePlayer(velocity * deltaTime);
    }
}
