using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour
{
    public float targetYPosition = 3f;

    private float inverseMoveTime;

    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (Mathf.Abs(transform.position.y - targetYPosition) < float.Epsilon)
        {
            Destroy(this.gameObject);
        }
    }

    public void Init(float speed)
    {
        Debug.Log("ARROW MOVE INIT");
        inverseMoveTime = 1f / speed;
        var end = new Vector3(transform.position.x, targetYPosition, 0f);
        StartCoroutine(SmoothMovement(end));
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
}
