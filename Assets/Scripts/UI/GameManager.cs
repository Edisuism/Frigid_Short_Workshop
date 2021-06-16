using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Object mainMenuScene;
    [SerializeField]
    private List<Object> scenes;

    private int currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene.name);
    }
    
    public void PlayLevel(int level)
    {
        if (level > scenes.Count)
        {
            PlayMainMenu();
            return;
        }
        
        string sceneName = scenes[level].name;
        currentLevel = level;

        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void PlayNextLevel()
    {
        if (currentLevel < scenes.Count)
        {
            PlayLevel(currentLevel + 1);
        }
        else
        {
            currentLevel = 0;
            PlayMainMenu();
        }
    }
    
    public void StartPlayingNextLevel(float delay)
    {
        StartCoroutine(PlayNextLevelWithDelay(delay));
    }

    public IEnumerator PlayNextLevelWithDelay(float delay)
    {
        // Use realtime so Time.timeScale does not matter.
        yield return new WaitForSecondsRealtime(delay);
        PlayNextLevel();
    }
}