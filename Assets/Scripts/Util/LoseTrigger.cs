using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if has hit the player
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.LoseLevel();
        }
    }
}
