using UnityEngine;

public class KinematicCharacterController : MonoBehaviour
{
    [Header("Gizmos")]
    [SerializeField]
    private bool showGroundCollider;
    [Min(0)]
    public float drag = 1f;

    public delegate void MovementCollisionHandler(MovementCollisionContext collision, MovementContext movement);
    public event MovementCollisionHandler MovementCollisionEvent;
    public delegate void PushoutHandler(PushoutContext pushoutContext);
    public event PushoutHandler PushoutEvent;
    [HideInInspector]
    public Vector3 velocity;
    
    private new CapsuleCollider collider;
    private Rigidbody rb;
    private bool isGrounded;
    private const float SkinSize = 0.01f;
    private const float GroundColliderDistance = 0.1f;
    private readonly Collider[] overlappingBuffer = new Collider[128];
    private int nonSelfMask;
    private bool groundedChecked;
    private PushoutContext.Manager pushoutManager = new PushoutContext.Manager();
    private MovementCollisionContext.Manager collisionManager = new MovementCollisionContext.Manager();

    public bool IsGrounded
    {
        get
        {
            if (!groundedChecked)
            {
                isGrounded = CheckGround();
                groundedChecked = true;
            }
            return isGrounded;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        nonSelfMask = ~(1 << gameObject.layer);
    }

    private void OnDrawGizmos()
    {
        if (showGroundCollider)
        {
            if (!collider)
            {
                collider = GetComponent<CapsuleCollider>();
            }
            
            Gizmos.color = new Color(0.2f, 0.5f, 0.2f);
            Vector3 center = GetGroundColliderStart();
            Gizmos.DrawWireSphere(center, collider.radius);
        }
    }

    private void LateUpdate()
    {
        groundedChecked = false;
    }

    public void Move(Vector3 displacement)
    {
        Vector3 groundNormal = GetGroundNormal();
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        Vector3 moveDisplacement = rotation * displacement;
        Debug.DrawRay(transform.position, moveDisplacement, Color.blue, 0.1f);
        // print($"Rotation by {rotation}, from {displacement} to {fakeDisplacement} with normal {groundNormal}");

        Vector3 pushout = CalculatePushOut(transform.position);
        Vector3 slideDisplacement = SweepAndSlide(moveDisplacement + pushout + (velocity * Time.fixedDeltaTime));

        Vector3 verticalSlide = new Vector3(0, slideDisplacement.y, 0);
        Vector3 forwardSlide = new Vector3(0, 0, slideDisplacement.z);
        Vector3 rightSlide = new Vector3(slideDisplacement.x, 0, 0);
        
        Vector3 verticalDisplacement = SweepAndStop(verticalSlide, transform.position);
        Vector3 rightDisplacement = SweepAndStop(rightSlide, transform.position + verticalDisplacement);
        Vector3 forwardDisplacement = SweepAndStop(forwardSlide, transform.position + rightDisplacement);

        Vector3 finalDisplacement = verticalDisplacement + rightDisplacement + forwardDisplacement;

        // print($"grounded {IsGrounded}, displacement {finalDisplacement}");
        VisualDebugger.Instance.Set("Kinematic Character", "Is Touching Ground", IsGrounded);
        VisualDebugger.Instance.Set("Kinematic Character", "velocity", velocity);
        VisualDebugger.Instance.Set("Kinematic Character", "displacement", velocity);
        
        rb.MovePosition(rb.position + finalDisplacement);
        
        // Process velocity
        if (drag > 0)
        {
            velocity /= (drag + 1);
        }
    }

    private bool CheckGround()
    {
        Vector3 start = GetGroundColliderStart();
        Vector3 end = start + Vector3.up * collider.height;

        return Physics.CheckCapsule(start, end, collider.radius, nonSelfMask, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetGroundColliderStart()
    {
        Vector3 startingPosition = transform.position + Vector3.down * GroundColliderDistance;
        
        float pointOffset = (collider.height / 2f) - collider.radius;
        return startingPosition + (collider.center + Vector3.down * pointOffset);
    }

    private Vector3 GetGroundNormal()
    {
        RaycastHit hit;
        float distance = GroundColliderDistance + collider.height / 2f;
        bool found = Physics.Raycast(transform.position, Vector3.down, out hit, distance, nonSelfMask, QueryTriggerInteraction.Ignore);
        return found ? hit.normal : Vector3.up;
    }

    private Vector3 CalculatePushOut(Vector3 startingPosition)
    {
        Vector3 displacement = Vector3.zero;

        float pointOffset = (collider.height / 2f) - collider.radius;
        Vector3 start = startingPosition + (collider.center + Vector3.down * pointOffset);
        Vector3 end = start + Vector3.up * collider.height;

        int overlapCount = Physics.OverlapCapsuleNonAlloc(start, end, collider.radius, overlappingBuffer, nonSelfMask);
        pushoutManager.Reset();

        for (int i = 0; i < overlapCount; i++)
        {
            Collider overlapCollider = overlappingBuffer[i];
            pushoutManager.SetCollision(overlapCollider);
            PushoutEvent?.Invoke(pushoutManager.Context);
        }

        // Note: velocity is later applied in SweepAndSlide
        velocity += pushoutManager.Context.PushVelocity;
        displacement += pushoutManager.Context.PushDisplacement;
        return displacement;
    }

    private Vector3 SweepAndSlide(Vector3 displacement)
    {
        Vector3 counterDisplacement = Vector3.zero;
        Vector3 counterVelocity = Vector3.zero;
        float distance = displacement.magnitude + SkinSize;
        
        Debug.DrawRay(rb.position, displacement, Color.green, 0.1f);

        MovementContext movementContext = new MovementContext(displacement.normalized, distance);

        RaycastHit[] hits = rb.SweepTestAll(displacement.normalized, distance, QueryTriggerInteraction.Ignore);
        collisionManager.Reset();
        
        foreach (RaycastHit hit in hits)
        {
            collisionManager.SetHit(hit);
            MovementCollisionContext context = collisionManager.Context;
            
            float counterDistance = distance - hit.distance;
            context.hitCounterDisplacement = context.CalculateCounterVector(displacement.normalized * counterDistance);
            context.hitCounterVelocity = context.CalculateCounterVector(velocity);
            
            MovementCollisionEvent?.Invoke(context, movementContext);
            
            counterDisplacement += context.hitCounterDisplacement + context.CounterDisplacement;
            counterVelocity += context.hitCounterVelocity + context.CounterVelocity;
            Debug.DrawRay(rb.position, hit.normal, Color.red, 0.1f);
        }

        velocity += counterVelocity;
        Vector3 finalDisplacement = displacement + counterDisplacement;
        return finalDisplacement;
    }

    private Vector3 SweepAndStop(Vector3 displacement, Vector3 startingPosition)
    {
        float startDistance = displacement.magnitude + SkinSize;
        float distance = startDistance;

        RaycastHit hit;
        
        float pointOffset = (collider.height / 2f) - collider.radius;
        Vector3 point1 = startingPosition + (collider.center + Vector3.down * pointOffset);
        Vector3 point2 = point1 + Vector3.up * collider.height;

        if (Physics.CapsuleCast(point1, point2, collider.radius, displacement.normalized, out hit, startDistance, nonSelfMask, QueryTriggerInteraction.Ignore))
        {
            float safeDistance = SkinSize;

            if (safeDistance > hit.distance)
            {
                return Vector3.zero;
            }
        
            if (hit.distance < distance)
            {
                distance = hit.distance - safeDistance;
            }

            Vector3 finalDisplacement = displacement.normalized * distance;
            return finalDisplacement;
        }
        return displacement;
    }
}