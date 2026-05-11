using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float velocidad = 5f;
    private Vector2 direccionMovimiento;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private int score;
    //private const int TargetScore = 5;

    private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    public static PlayerController instance;

    public Animator animator;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions = inputActions;
        playerInput.defaultActionMap = "defaultActionMap";
    }
    void Start()
    {
        score = 0;
    }



    void OnMove(InputValue value)
    {
        direccionMovimiento = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direccionMovimiento * velocidad;
        if (direccionMovimiento.x > 0 && sprite.flipX)
        {
            sprite.flipX = false;//der
        }
        else if (direccionMovimiento.x < 0 && !sprite.flipX)
        {
            sprite.flipX = true;//izq
        }

        if(rb.linearVelocity.magnitude < velocidad * 1.1f)
        {
            rb.linearVelocity = direccionMovimiento * velocidad;
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PickMeUp"))
        {
            UnityEngine.Object.Destroy(other.gameObject);
            score++;
            //UnityEngine.Debug.Log("score");
            /*if(score >= TargetScore)
            {
                //SceneManager.LoadScene("Scene2");
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if(nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    UnityEngine.Debug.Log("Congrats!");
                }
            }*/
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerInput.ActivateInput();
        rb.WakeUp();
    }
}
