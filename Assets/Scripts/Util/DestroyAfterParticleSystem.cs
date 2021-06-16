using System;
using UnityEngine;


public class DestroyAfterParticleSystem : MonoBehaviour
{
    public float delay;
    
    private ParticleSystem ps;
    private bool isDestroying;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!ps.IsAlive() && !isDestroying)
        {
            isDestroying = true;
            Destroy(gameObject, delay);
        }
    }
}