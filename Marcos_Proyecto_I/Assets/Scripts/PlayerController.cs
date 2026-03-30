using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector3 movementDirection;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    [Header("Collisions")]
    public Transform movePoint;
    public LayerMask Obstacles;

    [Header("CoinsCounter")]
    public int coinAmount;

    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;
    private Label coinText;

    public void Start()
    {
        coinAmount = 0;
        coinText = uiDocument.rootVisualElement.Q<Label>("CoinCounter");
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        movePoint.parent = null;
    }



    void Update()
    {
        //El Player sigue al MovePoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        
        //comprobar si el player ya esta en el movepoint antes de volver a moverlo
        if (Vector3.Distance(transform.position, movePoint.position) == 0 )
        {
            //Comprobamos si el input es +1(derecha) o -1(izquierda)
            if (Mathf.Abs(movementDirection.x) == 1f)
            {
                //Comprobamos no hay obstaculos esa dirección
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(movementDirection.x, 0f, 0f), 0.2f, Obstacles))
                {
                    movePoint.position += new Vector3(Mathf.RoundToInt(movementDirection.x), 0f, 0f);
                }
            }

            //Igual en Vertical
            else if (Mathf.Abs(movementDirection.y) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, movementDirection.y, 0f), 0.2f, Obstacles))
                {
                    movePoint.position += new Vector3(0f, Mathf.RoundToInt(movementDirection.y), 0f);
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            coinAmount += 1;
            coinText.text = "Coins: " + coinAmount;
            Destroy(collision.gameObject);
        }
    }


    public void OnMove(InputValue value)
    {
        movementDirection = value.Get<Vector2>();
    }

    public void OnOpenMenu(InputValue value)
    {

    }


}
