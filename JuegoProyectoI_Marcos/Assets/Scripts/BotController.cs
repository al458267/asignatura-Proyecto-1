using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BotController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3f;
    private Rigidbody2D rb;
    private float minDistance = 0.5f;
    private bool lookingRight = true;


    [SerializeField] private float detectionDistance = 2.5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float rotationSmoothTime = 0.15f;
    private float avoidanceStickiness = 0.2f;
    private Vector2 currentVelocity;
    private Vector2 chosenAvoidanceDir;
    private float avoidanceTimer;

    [Header("Attack Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float attackCooldown = 1.0f;
    private float nextAttackTime;
    [SerializeField] private float knockbackForce = 15f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Primera Version
    /*void FixedUpdate()
    {
        Vector2 direction =(target.position - target.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2f);
        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            direction += new Vector2(direction.y, -direction.x) * 0.5f;
        }
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > minDistance)
        {
            rb.linearVelocity = direction.normalized * speed;
            ControlRotation();
        }
    }*/


    private void ControlRotation()
    {
        if(rb.linearVelocity.x > 0.2f && !lookingRight)
        {
            RotateY();
        }
        else
        {
            if (rb.linearVelocity.x < 0.2f && lookingRight)
            {
                RotateY();
            }
        }
    }
    private void RotateY()
    {
        lookingRight = !lookingRight;
        transform.Rotate(0, 180, 0);
    }

    void FixedUpdate()
    {
        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > minDistance)
        {
            Vector2 finalDir = CalculateSmartDirection(dirToTarget);
            Vector2 smothDir = Vector2.SmoothDamp(rb.linearVelocity.normalized, finalDir, ref currentVelocity, rotationSmoothTime);
            rb.linearVelocity = smothDir * speed;
            ControlRotation();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            avoidanceTimer = 0;
        }
    }
    private Vector2 CalculateSmartDirection(Vector2 targetDir)
    {
        Vector2 finalDir = targetDir;
        if(avoidanceTimer > 0)
        {
            avoidanceTimer -= Time.deltaTime;
            finalDir = chosenAvoidanceDir;
        }
        else
        {
            RaycastHit2D hitCenter = Physics2D.CircleCast(transform.position, 0.3f, targetDir, detectionDistance, obstacleLayer);
            if(hitCenter.collider != null)
            {
                Vector2 leftRayDir = Quaternion.Euler(0f, 0f, 60f) * targetDir;
                Vector2 rightRayDir = Quaternion.Euler(0f, 0f, -60f) * targetDir;
                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, leftRayDir, detectionDistance, obstacleLayer);
                RaycastHit2D hitRight = Physics2D.Raycast(transform.position, rightRayDir, detectionDistance, obstacleLayer);

                if (hitLeft.collider == null)
                {
                    finalDir = leftRayDir;
                }
                else if (hitRight.collider == null)
                {
                    finalDir = rightRayDir;
                }
                else
                {
                    Vector2 perpendicular = Vector2.Perpendicular(hitCenter.normal);
                    float dot = Vector2.Dot(perpendicular, targetDir);

                }
                chosenAvoidanceDir = finalDir;
                avoidanceTimer = avoidanceStickiness;
            }
        }
        return finalDir;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Time.time >= nextAttackTime)
            {
                PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
                player.TakeDamage(damageAmount);
                nextAttackTime = Time.time + attackCooldown;

                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;//Direccion
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
