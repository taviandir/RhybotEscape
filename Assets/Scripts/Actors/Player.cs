using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveTime = 0.1f;
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

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        Debug.Log("PLAYER colliison with: " + hit.transform.gameObject.name);
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
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
