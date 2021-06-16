using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject pos1;
    public GameObject pos2;
    private float speed = 0.5f;

    private void Start()
    {

    }

    void Update()
    {
        transform.position = Vector3.Lerp(pos1.transform.position, pos2.transform.position, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }
}
