using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSystem : MonoBehaviour
{
    public float range = 30f;
    public float fireRate = 2f;
    private float fireCountdown = 0f;
    public Transform firePoint;
    public GameObject projectile;
    public float speed = 50.0f;
    public GameObject m_target = null;
    Vector3 m_lastKnownPosition = Vector3.zero;
    Quaternion m_lookAtRotation;

    // Update is called once per frame
    void Update()
    {
        if (m_target)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, m_target.transform.position);
            if (m_lastKnownPosition != m_target.transform.position && distanceToPlayer < range)
            {
                m_lastKnownPosition = m_target.transform.position;
                m_lookAtRotation = Quaternion.LookRotation(m_lastKnownPosition - transform.position);
            }

            if (transform.rotation != m_lookAtRotation && distanceToPlayer < range)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_lookAtRotation, speed * Time.deltaTime);
            }
            else if (fireCountdown <= 0f && distanceToPlayer < range)
            {
                shoot();
                fireCountdown = 1f / fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    bool SetTarget(GameObject target)
    {
        if (!target)
        {
            return false;
        }

        m_target = target;

        return true;
    }

    void shoot()
    {
        Instantiate(projectile, firePoint.position, firePoint.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
