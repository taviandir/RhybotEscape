using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public float speed = 0.2f;
    public int damage = 5;

    private float inverseMoveTime;

    void Awake()
    {
        inverseMoveTime = 1f / speed;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        var end = GameManager.instance.player.transform.position;
        LookAt(end);
        StartCoroutine(SmoothMovement(end));
    }

    private void LookAt(Vector3 target)
    {
        transform.right = target - transform.position;
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 RIGHT = transform.TransformDirection(Vector3.right);
            Vector3 FORWARD = transform.TransformDirection(Vector3.forward);
            transform.localPosition += RIGHT * Time.deltaTime * inverseMoveTime;
            transform.localPosition += FORWARD * Time.deltaTime * inverseMoveTime;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            player.GotShot(damage);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("EnemyShot"))
        {
            // pass through enemies (including self) and other bullets
            return;
        }
        Destroy(gameObject);
    }
}
