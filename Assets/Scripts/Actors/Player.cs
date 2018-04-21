using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveTime = 0.1f;
    public int energy = 100;
    public int energyFromBattery = 20;
    public bool isMoving;
    public LayerMask blockingLayer;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;

    private float inverseMoveTime;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        inverseMoveTime = 1.0f / moveTime;
    }

    void Start()
    {
        GameManager.instance.playerScript = this;
    }

    void Update()
    {
        // NOTE : direct control for testing only
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        // prevent diagonal movements
        if (x != 0)
            y = 0;

        AttemptMove(x, y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Battery")
        {
            Debug.Log("BATTERY");
            energy += energyFromBattery;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            Debug.Log("EXIT");
            enabled = false; // disable this script
            GameManager.instance.NextLevel();
        }
    }

    public void AttemptMove(float xDir, float yDir)
    {
        if (Math.Abs(xDir) < float.Epsilon && Math.Abs(yDir) < float.Epsilon) return;
        if (isMoving) return;

        // normalize input
        int x = xDir > 0 ? 1 : xDir < 0 ? -1 : 0;
        int y = yDir > 0 ? 1 : yDir < 0 ? -1 : 0;

        RaycastHit2D hit;
        bool canMove = Move(x, y, out hit);

        if (hit.transform == null)
            return;

        //T hitComponent = hit.transform.GetComponent<T>();

        //if (!canMove && hitComponent != null)
        //    OnCantMove(hitComponent);
    }

    private bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            energy -= 1;
            CheckIfGameOver();
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        Debug.Log("PLAYER colliison with: " + hit.transform.gameObject.name);
        return false;
    }

    public void CheckIfGameOver()
    {
        if (energy <= 0)
        {
            // GAME OVER
            enabled = false;
            // TODO : trigger something to indicate that game is over
            Debug.LogWarning("GAME OVER");
        }
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        isMoving = false;
    }
}
