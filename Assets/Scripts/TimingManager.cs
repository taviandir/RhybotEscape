﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TimingManager : MonoBehaviour 
{
    // NOTE : An alternative (and perhaps cleaner) approach than colliders would be to
    //        keep a list of all spawned arrows,  and on player input, check if any
    //        arrow's "Mathf.Abs(transform.position.y - this.transform.position.y) <= yWithinThreshold".

    //[Range(-1, 1)] public int xDirMove = 0;
    //[Range(-1, 1)] public int yDirMove = 0;
    public float ySpawnPosition = -20f;
    public float arrowMoveTime = 0.5f; // NOTE : the lower the value, the faster it goes!
    //public float spawnWaitMin = 1f;
    //public float spawnWaitMax = 5f;
    public float spawnWaitTime = 1.5f;
    public bool withinTiming = false; // NOTE : public for debug only
    public GameObject timingCircleObject; // where the sprite is
    public GameObject withinTimingObject;
    public GameObject normalTimingObject;
    public GameObject turboTimingObject;
    [HideInInspector] public event UnityAction onTimingCircleEnter;

    private SpriteRenderer spriteRenderer;
    private bool canDoAction = false;
    private float timeNextSpawn;

    void Start()
    {
        //canDoAction = true;
        spriteRenderer = timingCircleObject.GetComponent<SpriteRenderer>();
        SetNextSpawnTime();
        GameManager.instance.SetTimingManager(this);
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
        if (!canDoAction) return;

        var isActionDown = Input.GetKeyDown(KeyCode.Space);
        if (isActionDown)
        {
            Invoke("ReactivateTiming", 0.25f);
            canDoAction = false;

            timingCircleObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Invoke("RevertTimingScale", 0.1f);

            if (withinTiming)
            {
                spriteRenderer.color = Color.cyan;

                var isTurbo = withinTimingObject.GetComponent<MoveTimingObject>().isTurbo;
                Destroy(withinTimingObject);
                withinTimingObject = null;
                withinTiming = false;

                GameManager.instance.actionManager.PerformAction(isTurbo);
            }
            else
            {
                spriteRenderer.color = Color.red;
                GameManager.instance.player.AlterEnergy(-1);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        withinTiming = true;
        withinTimingObject = other.gameObject;
        onTimingCircleEnter.Invoke();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        withinTiming = false;
        withinTimingObject = null;
    }

    public void Activate()
    {
        canDoAction = true;
    }

    private void SetNextSpawnTime()
    {
        //timeNextSpawn = Time.time + Random.Range(spawnWaitMin, spawnWaitMax);
        timeNextSpawn = Time.time + spawnWaitTime;
    }

    private void SpawnArrow()
    {
        var spawnedArrow = Instantiate(
                normalTimingObject, 
                new Vector3(transform.position.x, ySpawnPosition, 0f),
                Quaternion.identity) as GameObject;

        var arrowScript = spawnedArrow.GetComponent<MoveTimingObject>();
        var levelBoost = GameManager.instance.level / (1 + 1) * -0.1f;
        var speed = arrowMoveTime + levelBoost;
        arrowScript.Init(speed);
        spawnedArrow.transform.parent = transform;
    }

    private void RevertTimingScale()
    {
        timingCircleObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void ReactivateTiming()
    {
        canDoAction = true;
        spriteRenderer.color = Color.white;
    }
}
