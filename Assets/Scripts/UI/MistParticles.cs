using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistParticles : MonoBehaviour
{
    public ParticleSystem particleLauncher;
    public float particleLifetimeDuration;

    public GameObject mistEffects;
    public GameObject newMistParticle;

    List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        particleLauncher = GetComponent<ParticleSystem>();
        particleLifetimeDuration = mistEffects.GetComponent<ParticleSystem>().main.duration;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        for(int i = 0; i < collisionEvents.Count; i++)
        {
            EmitAtLocation(collisionEvents[i]);
        }
    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        newMistParticle = Instantiate(mistEffects, particleCollisionEvent.intersection, Random.rotation);
        StartCoroutine(DestroyMistParticle(newMistParticle));
    }

    IEnumerator DestroyMistParticle(GameObject mistParticle)
    {
        yield return new WaitForSeconds(particleLifetimeDuration * 3);
        Destroy(mistParticle);
    }
}
