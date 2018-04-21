using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    [Range(-1, 1)] public int xDirMove = 0;
    [Range(-1, 1)] public int yDirMove = 0;
    public bool withinTiming = false; // public for debug only

    public GameObject arrowToSpawn;

    private Player playerScript;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerScript = GameManager.instance.playerScript;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x == xDirMove && y == yDirMove)
        {
            spriteRenderer.color = Color.cyan;
            Invoke("RevertColor", 0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        withinTiming = true;
        throw new System.NotImplementedException();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        withinTiming = false;
        throw new System.NotImplementedException();
    }

    private void RevertColor()
    {
        spriteRenderer.color = Color.white;
    }
}
