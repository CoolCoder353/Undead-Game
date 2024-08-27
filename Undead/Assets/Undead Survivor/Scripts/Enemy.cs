using System.Collections;
using UnityEngine;


public class Enemy : MonoBehaviourID
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private Transform player;
    private Vector2 playerPos;

    public bool invertRotation = false;


    public float damage = 10f;
    public float health = 100f;
    public float maxHealth = 100f;

    public float knockbackForce = 10f;
    public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) { return; }
        playerPos = player.position;
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        if (dead) { return; }
        Move();
    }

    void Move()
    {
        Vector2 direction = playerPos - rb.position;
        direction.Normalize();
        movement = direction;

        //Rotate left and right depending on the direction
        if ((movement.x > 0 && invertRotation) || (movement.x < 0 && !invertRotation))
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        DetectHit(collision);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        DetectHit(collision);
    }

    void DetectHit(Collision2D collision)
    {
        if (dead) { return; }
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(20);
            Destroy(collision.gameObject);
        }
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }



    IEnumerator Die()
    {
        dead = true;

        EnemySpawner.Instance.EnemyDied(this);

        //Disable collider
        GetComponent<Collider2D>().enabled = false;


        animator.SetBool("Dead", true);
        yield return new WaitForSeconds(1f);
        //slowly fade out
        float fadeTime = 1f;
        float elapsedTime = 0f;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeTime);
            sprite.color = color;
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}