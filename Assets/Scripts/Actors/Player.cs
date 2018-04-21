using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveTime = 0.5f;
    public bool isMoving;

    public void AttemptMove(int xDir, int yDir)
    {
        if (xDir == 0 && yDir == 0)
            return;

        // normalize input
        int x = xDir > 0 ? 1 : xDir < 0 ? -1 : 0;
        int y = yDir > 0 ? 1 : yDir < 0 ? -1 : 0;

        // TODO : move coroutine that moves object from A to B 
        transform.position = new Vector3(transform.position.x + x, transform.position.y, 0f);
    }
}
