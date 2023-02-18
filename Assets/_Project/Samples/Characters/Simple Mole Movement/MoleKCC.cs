using nickmaltbie.OpenKCC.Character;
using nickmaltbie.OpenKCC.Character.Action;
using nickmaltbie.OpenKCC.Character.Attributes;
using nickmaltbie.OpenKCC.Character.Config;
using nickmaltbie.OpenKCC.Character.Events;
using nickmaltbie.OpenKCC.MoleKCCSample;
using nickmaltbie.OpenKCC.netcode.Utils;
using nickmaltbie.OpenKCC.Utils;
using nickmaltbie.TestUtilsUnity;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

// IGetKCCConfig and IGetKCCGrounded are required for the KCCMovenentEngine Abstraction
public class MoleKCC : MonoBehaviour
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
    private MoleMovementEngine _movementEngine;

    [SerializeField]
    private Vector3 relativeUp;

    [SerializeField]
    private Vector3 previousPosition;

    private void Awake()
    {
        playerMovement = movementReference.ToInputAction();
        playerMovement.Enable();

        jumpInput = jumpInputReference.ToInputAction();
        jumpInput.Enable();
    }

    /// <summary>
    /// The the player's desired velocity for their current input value.
    /// </summary>
    /// <returns>Vector of player velocity based on input movement rotated by player view and projected onto the
    /// ground.</returns>
    public Vector3 GetDesiredMovement()
    {
        var moveDir = Quaternion.FromToRotation(Vector3.up, _movementEngine.GroundedState.SurfaceNormal);
        Vector3 rotatedMovement = moveDir * (transform.forward * playerMovement.ReadValue<Vector2>());
        float speed = RunSpeed;
        Vector3 scaledMovement = rotatedMovement * speed;
        return scaledMovement;
    }

    private void TickMoleMovement()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        // jumpAction.ApplyJumpIfPossible(_movementEngine.GroundedState);
        _movementEngine.MovePlayer(
        GetDesiredMovement() * Time.fixedDeltaTime,
                Velocity * Time.fixedDeltaTime);
        // UpdateGroundedState();

            // Apply gravity if needed
       if (_movementEngine.GroundedState.Falling || _movementEngine.GroundedState.Sliding)
       {
           Velocity += Physics.gravity * Time.fixedDeltaTime;
       }
       else if (_movementEngine.GroundedState.StandingOnGround)
       {
           Velocity = Vector3.zero;
       }

       relativeUp = _movementEngine.Up;


        if (_movementEngine.GroundedState.StandingOnGround)
        {
            // Set the player's rotation to follow the ground
            var rotation = Quaternion.FromToRotation(Vector3.up, relativeUp);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.fixedDeltaTime);
        }
        else
        {
            // or Velocity if not grounded
            transform.rotation = Quaternion.Lerp(
            transform.rotation,
                Quaternion.FromToRotation(Vector3.up, (transform.position - previousPosition).normalized),
                10 * Time.fixedDeltaTime);
        }

        previousPosition = transform.position;

        // GetComponent<NetworkRelativeTransform>()?.UpdateState(relativeParentConfig);
        // base.FixedUpdate();
    }




    void FixedUpdate()
    {

        TickMoleMovement();

        /*
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
        */
    }

    void ApplyMovement(Vector3 velocity, float deltaTime)
    {
        // MovePlayer based on custom velocity
        _movementEngine.MovePlayer(velocity * deltaTime);
    }
}
