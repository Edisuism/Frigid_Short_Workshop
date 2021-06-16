using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recharge : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //visual effect
        Destroy(gameObject);
    }
}
