using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveTime = 0.1f;
    public int energyFromBattery = 20;
    public bool isMoving;
    public LayerMask blockingLayer;

    private int energy;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Text energyText;
    private float inverseMoveTime;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        inverseMoveTime = 1.0f / moveTime;
        GameManager.instance.player = this;
        energyText = GameObject.Find("EnergyText").GetComponent<Text>(); // TODO : no string-based retrieval
        energy = GameManager.instance.energy; // transfer energy value from GameManager to this script
        AlterEnergy(0);
    }

    void Update()
    {
        //// NOTE : direct control for testing only
        //var x = Input.GetAxisRaw("Horizontal");
        //var y = Input.GetAxisRaw("Vertical");

        //// prevent diagonal movements
        //if (x != 0)
        //    y = 0;

        //AttemptMove(x, y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Battery")
        {
            Debug.Log("BATTERY");
            AlterEnergy(energyFromBattery);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            Debug.Log("EXIT");
            enabled = false; // disable this script
            GameManager.instance.NextLevel();
        }
    }

    public void AlterEnergy(int change)
    {
        energy += change;
        CheckIfGameOver();
        energyText.text = "Energy: " + energy;
    }

    public void AttemptMove(int xDir, int yDir)
    {
        if (isMoving) return;
        if (xDir == 0 && yDir == 0) return;
        //if (Math.Abs(xDir) < float.Epsilon && Math.Abs(yDir) < float.Epsilon) return;

        //// normalize input
        //int x = xDir > 0 ? 1 : xDir < 0 ? -1 : 0;
        //int y = yDir > 0 ? 1 : yDir < 0 ? -1 : 0;

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
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
            AlterEnergy(-1);
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
