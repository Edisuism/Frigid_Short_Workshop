using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField]
    private GameObject winUI;
    [SerializeField]
    private GameObject loseUI;
    [SerializeField]
    private TextMeshProUGUI levelTimeText;
    [SerializeField]
    private TextMeshProUGUI bestTimeText;

    private const string BestTimePref = "BestTime";

    private float levelTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        winUI.gameObject.SetActive(false);
        loseUI.gameObject.SetActive(false);
        VisualDebugger.Instance.isVisible = false;
        CameraController.Instance.canPlayerControl = true;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    private void FixedUpdate()
    {
        levelTime += Time.fixedDeltaTime;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        winUI.gameObject.SetActive(false);
        loseUI.gameObject.SetActive(false);
    }

    public void PlayNextLevel()
    {
        GameManager.Instance.StartPlayingNextLevel(0.1f);
    }

    public void PlayMainMenu()
    {
        GameManager.Instance.PlayMainMenu();
    }

    public void WinLevel()
    {
        winUI.gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        // Need to use double here to get the decimal places properly. Then converted to a float.
        float fairLevelTime = (float) (Mathf.Round(levelTime * 1000f) / 1000.0);
        string pref = SceneManager.GetActiveScene().name + " " + BestTimePref;

        float bestTime = PlayerPrefs.HasKey(pref) ? PlayerPrefs.GetFloat(pref) : float.MaxValue;
        
        if (fairLevelTime < bestTime)
        {
            PlayerPrefs.SetFloat(pref, fairLevelTime);
            bestTimeText.text = $"NEW BEST TIME!";
        }
        else
        {
            bestTimeText.text = $"PREVIOUS BEST    {bestTime:00.000}";
        }
        
        levelTimeText.text = $"TIME  {levelTime:00.000}";
        DisableControls();
        PlayerPrefs.Save();
    }

    public void LoseLevel()
    {
        loseUI.gameObject.SetActive(true);
        DisableControls();
    }

    private void DisableControls()
    {
        Time.timeScale = 0;
        VisualDebugger.Instance.isVisible = false;
        CameraController.Instance.canPlayerControl = false;
    }
}