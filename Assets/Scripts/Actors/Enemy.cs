using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public LayerMask blockingLayer;
    public float moveTime = 0.1f;
    public float maxViewDistance  = 8f;
    public float maxShootDistance = 5f;
    public float shootCooldown = 3f;
    public Sprite spriteNormalMode;
    public Sprite spriteGuardMode;
    public Sprite spriteAlertMode;
    public GameObject shotPrefab;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private bool isMoving;
    private float inverseMoveTime;
    private Player player;
    private Vector3? lastKnownPosition;
    private SpriteRenderer sr;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        inverseMoveTime = 1.0f / moveTime;
    }

    void Start()
    {
        player = GameManager.instance.player;
    }

    // Triggers enemy to look for the player (no action taken), updates sprite mode
    public bool Look()
    {
        if (CanSeePlayer())
        {
            sr.sprite = spriteAlertMode;
            lastKnownPosition = player.transform.position;
            return true;
        }
        else if (lastKnownPosition != null)
        {
            sr.sprite = spriteGuardMode;
        }
        else
        {
            sr.sprite = spriteNormalMode;
        }

        return false;
    }

    // Triggers enemy to perform an action
    public void Action()
    {
        if (lastKnownPosition != null && lastKnownPosition.Value == transform.position)
        {
            //Debug.Log("ENEMY arrived at Last Known Position");
            lastKnownPosition = null;
        }

        // AI Logic goes here
        bool seeingPlayer = Look();
        //Debug.Log("SEE PLAYER? " + seeingPlayer);

        if (seeingPlayer)
        {
            if (enemyType == EnemyType.Melee)
            {
                MoveTowardsPosition(player.transform.position);
            }
            else if (enemyType == EnemyType.Ranged)
            {
                var playerDistance = Vector3.Distance(player.transform.position, transform.position);
                if (playerDistance <= maxShootDistance)
                {
                    Debug.Log("ENEMY SHOOT");
                    Instantiate(shotPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    MoveTowardsPosition(player.transform.position);
                }
            }
        }
        else if (lastKnownPosition != null)
        {
            // move towards last known position
            MoveTowardsPosition(lastKnownPosition.Value);
        }
        else
        {
            // not seeing player, or any knowledge of where player is
            DoRandomMove();
        }
    }

    private void MoveTowardsPosition(Vector3 targetPosition)
    {
        Debug.Log("ENEMY Move towards position");
        // approach player
        var diff = targetPosition - transform.position;
        var xDir = diff.x > 0.1f ? 1 : diff.x < -0.1f ? -1 : 0;
        var yDir = diff.y > 0.1f ? 1 : diff.y < -0.1f ? -1 : 0;

        var xDirOrig = xDir;
        var yDirOrig = yDir;

        Debug.Log(diff);

        // check for walls (avoid failing movement because unit wanted to go diagonally into the wall)
        RaycastHit2D hitWallCheck;
        if (xDir != 0)
        {
            var start = transform.position;
            DoRaycast(start, start + new Vector3(xDir, yDir, 0), out hitWallCheck);
            if (hitWallCheck.transform != null && hitWallCheck.transform.CompareTag("Wall"))
            {
                bool nulledX = false;
                bool nulledY = false;
                Debug.Log("ADJUSTING DIRECTION DUE TO COLLISION (1)");
                if (diff.x > diff.y)
                {
                    yDir = 0;
                    nulledY = true;
                }
                else
                {
                    xDir = 0;
                    nulledX = true;
                }

                // check if we still hit wall despite adjustment
                DoRaycast(start, start + new Vector3(xDir, yDir, 0), out hitWallCheck);
                if (hitWallCheck.transform != null && hitWallCheck.transform.CompareTag("Wall"))
                {
                    Debug.Log("ADJUSTING DIRECTION DUE TO COLLISION (2)");
                    // still hitting wall
                    if (nulledX)
                    {
                        xDir = xDirOrig;
                        yDir = 0;
                    }
                    else if (nulledY)
                    {
                        yDir = yDirOrig;
                        xDir = 0;
                    }
                }
            }
        }

        AttemptMove(xDir, yDir);
    }

    private void DoRandomMove()
    {
        Debug.Log("ENEMY Random move");
        var xDir = Random.Range(-1, 2);
        var yDir = Random.Range(-1, 2);
        AttemptMove(xDir, yDir);
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
            //Debug.Log("ENEMY DOESNT SEE PLAYER");
            return false;
        }
        
        //Debug.Log("ENEMY see: " + hit.transform.gameObject.name);
        var distance = Vector3.Distance(transform.position, hit.transform.position);
        //Debug.Log("Distance:" + distance);
        return hit.transform.CompareTag("Player") && distance < maxViewDistance;
    }

    private void AttemptMove(int xDir, int yDir)
    {
        if (isMoving) return;
        if (xDir == 0 && yDir == 0) return;

        Debug.Log("ENEMY orig: " + xDir + "," + yDir);

        
        //if (yDir != 0)
        //{
        //    var start = transform.position;
        //    DoRaycast(start, start + new Vector3(0, yDir, 0), out hitWallCheck);
        //    if (hitWallCheck.transform != null && hitWallCheck.transform.CompareTag("Wall"))
        //    {
        //        yDir = 0;
        //    }
        //}

        Debug.Log("ENEMY move: " + xDir + "," + yDir);

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

        //Debug.Log("ENEMY collision with: " + hit.transform.gameObject.name);

        if (hit.transform.CompareTag("Player") && enemyType == EnemyType.Melee)
        {
            Debug.Log("MELEE ENEMY ATTACK");
            GameManager.instance.player.AutoGameOver();
            return true;
        }

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
