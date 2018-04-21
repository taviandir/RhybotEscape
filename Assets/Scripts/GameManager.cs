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
    private int level = 1;

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

    public void NextLevel()
    {
        // TODO : incur a small delay so that the switch aint instant
        // TODO : perhaps also show floor number (like roguelike showed Day X)
        level += 1;
        SceneManager.LoadScene("levelScene");
    }

    private void InitGame()
    {
        boardScript.LevelSetup(level);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GAME MANAGER - OnSceneFinishedLoading");
        InitGame();
    }
}
