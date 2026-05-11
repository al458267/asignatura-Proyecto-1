using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHealthBarScript : MonoBehaviour
{
    [SerializeField] private Gradient HealthGradient;
    private ProgressBar healthBar;


    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q<ProgressBar>("HealthBar");
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        Debug.Log("updated healthbar");
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
        Debug.Log("Coroutine");
        await Awaitable.EndOfFrameAsync();
        if (PlayerHealth.Instance != null)
        {
            Debug.Log("PlayerHealth");
            PlayerHealth.Instance.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(PlayerHealth.Instance.stats.currentHealth, PlayerHealth.Instance.stats.maxHealth); ;
        }
    }

    public void OnDisable()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnHealthChanged -= UpdateHealthBar;
        }
    }
}
