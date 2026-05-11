using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHealthBarScript : MonoBehaviour
{
    [SerializeField] private Gradient HealthGradient;
    private ProgressBar healthBar;
    public PlayerHealth playerHealth;


    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q<ProgressBar>("HealthBar");
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.highValue = maxHealth;
        healthBar.value = currentHealth;
        float pct = currentHealth / maxHealth;
        Color currentColor = HealthGradient.Evaluate(pct);
        var barElement = healthBar.Q(className: "unity-progress-bar__progress");

        barElement.style.backgroundColor = Color.clear;
        barElement.style.backgroundColor = new StyleColor(currentColor);
        healthBar.title = Mathf.RoundToInt(currentHealth).ToString();
    }

    public void OnEnable()
    {
        StartCoroutine(InitializeUI());
    }

    private async Awaitable InitializeUI()
    {
        await Awaitable.EndOfFrameAsync();
        if (playerHealth!= null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(playerHealth.stats.currentHealth, playerHealth.stats.maxHealth); ;
        }
    }

    public void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}
