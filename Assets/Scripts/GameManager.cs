using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // TODO : make singleton

    public static GameManager instance;

    [HideInInspector] public Player player;
    [HideInInspector] public ActionManager actionManager;
    public List<Enemy> enemies;
    public int level = 1;
    public int energy = 100;
    public float levelStartDelay = 3f;

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
        timingManager.onTimingCircleEnter -= TriggerEnemy;
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
        timingManager.onTimingCircleEnter += TriggerEnemy;
    }

    private void TriggerEnemy()
    {
        // move on every other trigger
        if (!nextTriggerEnemyMove)
        {
            enemies.ForEach(e => e.Look());
            nextTriggerEnemyMove = true;
            return;
        }

        enemies.ForEach(e => e.Action());
        nextTriggerEnemyMove = false;
    }

    private void InitGame()
    {
        // TODO : dont find by name
        GameObject.Find("FloorTextIntro").GetComponent<Text>().text = "Floor " + level;
        Invoke("StartLevel", levelStartDelay);

        enemies.Clear();
        actionManager.Init();
        levelBuilder.LevelSetup(level);
    }

    private void StartLevel()
    {
        GameObject.Find("FloorIntro").SetActive(false); // TODO : dont find by name
        timingManager.Activate();
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("GAME MANAGER - OnSceneFinishedLoading");
        InitGame();
    }
}
