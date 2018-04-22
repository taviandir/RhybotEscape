using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public LayerMask blockingLayer;
    public float moveTime = 0.1f;
    public float maxViewDistance = 5.5f;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private bool isMoving;
    private float inverseMoveTime;
    private Player player;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        inverseMoveTime = 1.0f / moveTime;
    }

    void Start()
    {
        player = GameManager.instance.player;
    }

    // Triggers enemy to perform an action
    public void Action()
    {
        // AI Logic goes here
        bool seeingPlayer = CanSeePlayer();
        Debug.Log("SEE PLAYER? " + seeingPlayer);
    }

    private bool CanSeePlayer()
    {
        var diff = player.transform.position - transform.position;
        Debug.Log(diff);
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(diff.x, diff.y);
        RaycastHit2D hit;
        DoRaycast(start, end, out hit);

        if (hit.transform == null)
        {
            Debug.Log("ENEMY DOESNT SEE PLAYER");
            return false;
        }
        
        Debug.Log("ENEMY see: " + hit.transform.gameObject.name);
        var distance = Vector3.Distance(transform.position, hit.transform.position);
        Debug.Log("Distance:" + distance);
        return hit.transform.CompareTag("Player") && distance < maxViewDistance;
    }

    private void AttemptMove(int xDir, int yDir)
    {
        if (isMoving) return;
        if (xDir == 0 && yDir == 0) return;

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
    }

    private bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        DoRaycast(start, end, out hit);

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        Debug.Log("ENEMY collision with: " + hit.transform.gameObject.name);
        return false;
    }

    private void DoRaycast(Vector3 start, Vector3 end, out RaycastHit2D hit)
    {
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
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

[System.Serializable]
public enum EnemyType
{
    Melee = 0,
    Ranged = 1
}
