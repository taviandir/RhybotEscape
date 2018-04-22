using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO : make singleton

    public static GameManager instance;

    [HideInInspector] public Player player;
    [HideInInspector] public ActionManager actionManager;
    public List<Enemy> enemies;
    public int level = 1;
    public int energy = 100;

    private bool nextTriggerEnemyMove = false;
    private LevelBuilder levelBuilder;
    private TimingManager timingManager;

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
        enemies = new List<Enemy>();
        levelBuilder = GetComponent<LevelBuilder>();
        actionManager = GetComponent<ActionManager>();
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
        timingManager.onTimingCircleEnter -= EnemyAction;
    }

    public void NextLevel()
    {
        // TODO : incur a small delay so that the switch aint instant
        // TODO : perhaps also show floor number (like roguelike showed Day X)
        level += 1;
        SceneManager.LoadScene("levelScene");
    }

    public void SetTimingManager(TimingManager tm)
    {
        timingManager = tm;
        timingManager.onTimingCircleEnter += EnemyAction;
    }

    private void EnemyAction()
    {
        // move on every other trigger
        if (!nextTriggerEnemyMove)
        {
            nextTriggerEnemyMove = true;
            return;
        }

        enemies.ForEach(e => e.Action());
        nextTriggerEnemyMove = false;
    }

    private void InitGame()
    {
        enemies.Clear();
        actionManager.Init();
        levelBuilder.LevelSetup(level);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GAME MANAGER - OnSceneFinishedLoading");
        InitGame();
    }
}
