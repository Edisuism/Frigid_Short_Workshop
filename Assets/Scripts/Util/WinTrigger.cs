using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider other)
    {
        // Check if has hit the player
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.WinLevel();
        }
    }
}