using UnityEngine;

public struct MovementContext
{
    private Vector3 direction;
    private float distance;

    public Vector3 Direction => direction;
    public float Distance => distance;
    public Vector3 Displacement => direction * distance;

    public MovementContext(Vector3 direction, float distance)
    {
        this.direction = direction;
        this.distance = distance;
    }
}

public class MovementCollisionContext
{
    public Vector3 hitCounterDisplacement;
    public Vector3 hitCounterVelocity;
    
    public Vector3 CounterVelocity { get; private set; }
    public Vector3 CounterDisplacement { get; private set; }
    public Vector3 Normal { get; private set; }
    public float Distance { get; private set; }
    public Vector3 Point { get; private set; }
    public Collider Collider { get; private set; }
    public GameObject GameObject => Collider.gameObject;

    public void AddCounterDisplacement(Vector3 counterDisplacement)
    {
        CounterDisplacement += counterDisplacement;
    }

    public void AddCounterVelocity(Vector3 counterVelocity)
    {
        CounterVelocity += counterVelocity;
    }
    
    public Vector3 CalculateCounterVector(Vector3 vector, Vector3 normal)
    {
        float counterSpeed = -Vector3.Dot(normal, vector);
        return normal * counterSpeed;
    }
    
    public Vector3 CalculateCounterVector(Vector3 vector)
    {
        return CalculateCounterVector(vector, Normal);
    }
    
    public class Manager
    {
        public MovementCollisionContext Context { get; }

        public Manager()
        {
            Context = new MovementCollisionContext();
        }
        
        public void SetHit(RaycastHit hit)
        {
            Context.Normal = hit.normal;
            Context.Distance = hit.distance;
            Context.Point = hit.point;
            Context.Collider = hit.collider;
        }
        
        public void Reset()
        {
            Context.hitCounterDisplacement = Vector3.zero;
            Context.hitCounterVelocity = Vector3.zero;
            Context.CounterDisplacement = Vector3.zero;
            Context.CounterVelocity = Vector3.zero;
        }
    }
}

public class PushoutContext
{
    public Collider Collider { get; private set; }
    public Vector3 PushDisplacement { get; private set; }
    public Vector3 PushVelocity { get; private set; }

    public void AddPushDisplacement(Vector3 pushDisplacement)
    {
        PushDisplacement += pushDisplacement;
    }

    public void AddPushVelocity(Vector3 pushVelocity)
    {
        PushVelocity += pushVelocity;
    }

    public class Manager
    {
        public PushoutContext Context { get; }

        public Manager()
        {
            Context = new PushoutContext();
        }
        
        public void SetCollision(Collider collider)
        {
            Context.Collider = collider;
        }

        public void Reset()
        {
            Context.PushDisplacement = Vector3.zero;
            Context.PushVelocity = Vector3.zero;
        }
    }
}