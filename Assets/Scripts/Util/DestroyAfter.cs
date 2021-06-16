using System;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float delay;
    
    private void Start()
    {
        Destroy(gameObject, delay);
    }
}