using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO : make singleton

    public static GameManager instance;

    public Player playerScript;

    private BoardManager boardScript;
    public int level = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        //InitGame();
    }

    void OnEnable()
    {
        //Debug.Log("GAME MANAGER ON ENABLE");
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    void OnDisable()
    {
        //Debug.Log("GAME MANAGER ON DISABLE");
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void InitGame()
    {
        level += 1;
        boardScript.LevelSetup(level);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GAME MANAGER - OnSceneFinishedLoading");
        InitGame();
    }
}
