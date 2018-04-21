using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RhythmManager : MonoBehaviour 
{
    // NOTE : An alternative (and perhaps cleaner) approach than colliders would be to
    //        keep a list of all spawned arrows,  and on player input, check if any
    //        arrow's "Mathf.Abs(transform.position.y - this.transform.position.y) <= yWithinThreshold".

    [Range(-1, 1)] public int xDirMove = 0;
    [Range(-1, 1)] public int yDirMove = 0;
    public float ySpawnPosition = -20f;
    public float arrowMoveTime = 0.5f;
    public float spawnWaitMin = 1f;
    public float spawnWaitMax = 5f;
    public bool withinTiming = false; // public for debug only
    public GameObject withingTimingObject;
    public GameObject arrowToSpawn;

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
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x == xDirMove && y == yDirMove)
        {
            spriteRenderer.color = Color.cyan;
            Invoke("RevertColor", 0.2f);

            if (withinTiming)
            {
                Destroy(withingTimingObject);
                withingTimingObject = null;
                withinTiming = false;
                playerScript.AttemptMove(xDirMove, yDirMove);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        withinTiming = true;
        withingTimingObject = other.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        withinTiming = false;
        withingTimingObject = null;
    }

    private void SetNextSpawnTime()
    {
        timeNextSpawn = Time.time + Random.Range(spawnWaitMin, spawnWaitMax);
    }

    private void SpawnArrow()
    {
        var spawnedArrow = Instantiate(
                arrowToSpawn, 
                new Vector3(transform.position.x, ySpawnPosition, 0f),
                Quaternion.identity) as GameObject;

        var arrowScript = spawnedArrow.GetComponent<MoveArrow>();
        arrowScript.Init(arrowMoveTime);
        spawnedArrow.transform.parent = transform;
    }

    private void RevertColor()
    {
        spriteRenderer.color = Color.white;
    }
}
