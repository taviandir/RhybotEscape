using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public float speed = 0.1f;
    public int damage = 5;
    private float inverseMoveTime;

    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / speed;
    }

    void Start()
    {
        Init();
    }

    //void Update()
    //{
    //    LookAt();
    //}

    public void Init()
    {
        var end = GameManager.instance.player.transform.position;
        LookAt(end);
        StartCoroutine(SmoothMovement(end));
    }

    private void LookAt(Vector3 target)
    {
        //Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //Vector3 diff = target - transform.position;
        //diff.Normalize();
        //float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
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
            player.AlterEnergy(-damage);
        }
        else if (other.CompareTag("Enemy"))
        {
            // pass through enemies (including self)
            return;
        }
        Destroy(gameObject);
    }
}
