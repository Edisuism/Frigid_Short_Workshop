using System;
using UnityEngine;


public class IceMovement : MonoBehaviour
{
    public float boostPower = 0.1f;
    public float addedMomentum;
    public float ammoRemoval;
    
    private KinematicCharacterController characterController;
    private PlayerController playerController;

    private void Start()
    {
        characterController = GetComponent<KinematicCharacterController>();
        playerController = GetComponent<PlayerController>();
        characterController.MovementCollisionEvent += OnMovementCollision;
        characterController.PushoutEvent += OnPushoutEvent;
    }

    private void OnPushoutEvent(PushoutContext context)
    {
        if (context.Collider.CompareTag("Structure"))
        {
            Vector3 counterDisplacement = transform.position - context.Collider.transform.position;
            Vector3 counterVelocity = counterDisplacement / Time.fixedDeltaTime;
            Vector3 pushVelocity = counterVelocity * boostPower;
            context.AddPushVelocity(pushVelocity);
            playerController.Momentum += addedMomentum;
            ParticleLauncher.Instance.Ammo -= ammoRemoval;
        }
    }

    private void OnMovementCollision(MovementCollisionContext collision, MovementContext movement)
    {
        
    }

    private void OnDestroy()
    {
        if (characterController)
        {
            characterController.MovementCollisionEvent -= OnMovementCollision;
            characterController.PushoutEvent -= OnPushoutEvent;
        }
    }
}