using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 1;
    public bool canPlayerControl = true;

    private Quaternion playerStartRotation;
    private Quaternion cameraStartRotation;

    public static CameraController Instance { get; private set; }

    public bool CanPlayerControl
    {
        get { return canPlayerControl; }
        set { canPlayerControl = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraStartRotation = transform.localRotation;
        playerStartRotation = transform.parent.rotation;
    }

    private void Update()
    {
        if (canPlayerControl)
        {
            float xMovement = Input.GetAxis("Mouse X") * sensitivity;
            float yMovement = Input.GetAxis("Mouse Y") * sensitivity;
            Quaternion currentRotation = transform.localRotation;

            transform.parent.rotation *= Quaternion.AngleAxis(xMovement, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(-yMovement, Vector3.right);

            // Clamp Camera rotation.
            if (Quaternion.Angle(transform.localRotation, Quaternion.identity) > 90) 
                transform.localRotation = currentRotation;
        }
    }

    public void ResetRotationToStart()
    {
        transform.localRotation = cameraStartRotation;
        transform.parent.rotation = playerStartRotation;
    }
}
