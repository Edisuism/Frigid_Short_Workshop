using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject contact;
    private float m_Speed = 1000f;   
    private float m_Lifespan = 3f; 
    private Rigidbody m_Rigidbody;
    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_Rigidbody.AddForce(m_Rigidbody.transform.forward * m_Speed);
        Destroy(gameObject, m_Lifespan);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        ContactPoint contactPos = collision.GetContact(0);
        Instantiate(contact, contactPos.point, collision.transform.rotation);
    }
}
