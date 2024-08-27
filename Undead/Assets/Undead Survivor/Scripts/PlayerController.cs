using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;


    public GameObject DeadScreen;

    public Vector2 maxDistance = new Vector2(15, 15);

    private bool shoot = false;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public float bulletsPerSecond = 2f;

    private float timeBtwShots;

    public float pointsGained = 0;



    public float health = 100f;
    public float maxHealth = 100f;
    public bool dead = false;
    public bool disableMovement = false;

    public static PlayerController Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;



        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) { return; }


        // Get input from player
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        shoot = Input.GetButton("Fire1");
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        if (dead) { return; }

        if (!disableMovement) Move();
        Shoot();
    }

    public void AddScore(float points)
    {
        pointsGained += points;
        PointsCounter.Instance.AddPoints(points);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        DetectHit(collision);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        ////DetectHit(collision);
    }

    void DetectHit(Collision2D collision)
    {
        if (dead) { return; }
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            StartCoroutine(KnockBack(collision, enemy));
            TakeDamage(enemy.damage);

        }
    }

    IEnumerator KnockBack(Collision2D collision, Enemy enemy)
    {
        disableMovement = true;
        Vector2 direction = (transform.position - collision.transform.position).normalized;
        rb.AddForce(direction * enemy.knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.02f);
        rb.velocity = Vector2.zero;
        disableMovement = false;
    }


    void TakeDamage(float damage)
    {
        health -= damage;
        HealthBar.Instance.SetHealth(health, maxHealth);
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }



    void Shoot()
    {
        if (shoot && timeBtwShots <= 0)
        {
            SpawnBullet();
            timeBtwShots = 1 / bulletsPerSecond;
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }
    void SpawnBullet()
    {
        Vector3 screenPosDepth = Input.mousePosition;
        // Give it a depth. Maybe a raycast depth, maybe a clipping plane...
        screenPosDepth.z = -Camera.main.transform.position.z;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(screenPosDepth);

        Vector2 direction = mouse - new Vector2(firePoint.position.x, firePoint.position.y);
        direction.Normalize();

        // Calculate the angle between the fire point and the mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Retrieve the current rotation angle of the firePoint
        float firePointAngle = firePoint.rotation.eulerAngles.z;

        // Calculate the final angle by adding the firePoint's angle to the calculated angle
        float finalAngle = angle + firePointAngle;

        // Create a bullet with the final angle
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(finalAngle, Vector3.forward));
        // Get the rigidbody of the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        // Add force to the bullet
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2f);
    }

    void Move()
    {
        if (movement.x > 0)
        {
            // Flip the player by rotating 0 degrees
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0)
        {
            // Flip the player by rotating 180 degrees
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        //Only move the player if that wont make it go out of bounds
        if (transform.position.x + movement.x * moveSpeed * Time.fixedDeltaTime > maxDistance.x ||
            transform.position.x + movement.x * moveSpeed * Time.fixedDeltaTime < -maxDistance.x)
        {
            movement.x = 0;
        }

        if (transform.position.y + movement.y * moveSpeed * Time.fixedDeltaTime > maxDistance.y ||
            transform.position.y + movement.y * moveSpeed * Time.fixedDeltaTime < -maxDistance.y)
        {
            movement.y = 0;
        }

        //Set the speed for the animation
        animator.SetFloat("Speed", movement.magnitude);

        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Clamp the player's position
        Vector2 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -maxDistance.x, maxDistance.x);
        pos.y = Mathf.Clamp(pos.y, -maxDistance.y, maxDistance.y);
        transform.position = pos;

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, maxDistance * 2);
    }

    IEnumerator Die()
    {
        dead = true;
        TimeCounter.Instance.isCounting = false;

        //Hide all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        //Remove Velocity
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;




        animator.SetBool("Dead", true);
        DeadScreen.SetActive(true);
        yield return null;
    }

}