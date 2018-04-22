using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimingManager : MonoBehaviour 
{
    // NOTE : An alternative (and perhaps cleaner) approach than colliders would be to
    //        keep a list of all spawned arrows,  and on player input, check if any
    //        arrow's "Mathf.Abs(transform.position.y - this.transform.position.y) <= yWithinThreshold".

    [Range(-1, 1)] public int xDirMove = 0;
    [Range(-1, 1)] public int yDirMove = 0;
    public float ySpawnPosition = -20f;
    public float arrowMoveTime = 0.5f; // NOTE : the lower the value, the faster it goes!
    public float spawnWaitMin = 1f;
    public float spawnWaitMax = 5f;
    public bool withinTiming = false; // NOTE : public for debug only
    public GameObject withinTimingObject;
    public GameObject normalTimingObject;
    public GameObject turboTimingObject;

    private Player playerScript;
    private SpriteRenderer spriteRenderer;

    private float timeNextSpawn;

    void Start()
    {
        playerScript = GameManager.instance.playerScript;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetNextSpawnTime();
    }

    void Update()
    {
        if (Time.time > timeNextSpawn)
        {
            SpawnArrow();
            SetNextSpawnTime();
        }
    }

    void LateUpdate()
    {
        var isActionDown = Input.GetKeyDown(KeyCode.Space);
        if (isActionDown)
        {
            spriteRenderer.color = Color.cyan;
            Invoke("RevertColor", 0.2f);

            if (withinTiming)
            {
                Destroy(withinTimingObject);
                withinTimingObject = null;
                withinTiming = false;
                playerScript.AttemptMove(xDirMove, yDirMove);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        withinTiming = true;
        withinTimingObject = other.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        withinTiming = false;
        withinTimingObject = null;
    }

    private void SetNextSpawnTime()
    {
        //timeNextSpawn = Time.time + Random.Range(spawnWaitMin, spawnWaitMax);
        timeNextSpawn = Time.time + 2f;
    }

    private void SpawnArrow()
    {
        var spawnedArrow = Instantiate(
                normalTimingObject, 
                new Vector3(transform.position.x, ySpawnPosition, 0f),
                Quaternion.identity) as GameObject;

        var arrowScript = spawnedArrow.GetComponent<MoveTimingObject>();
        arrowScript.Init(arrowMoveTime);
        spawnedArrow.transform.parent = transform;
    }

    private void RevertColor()
    {
        spriteRenderer.color = Color.white;
    }
}
