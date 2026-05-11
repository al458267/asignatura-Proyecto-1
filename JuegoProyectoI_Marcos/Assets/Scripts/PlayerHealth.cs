using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health")]
    //[SerializeField] int maxHealth = 100;
    //private int currentHealth;
    public bool isInvulnerable;

    public HealthData stats;
    public event Action<float, float> OnHealthChanged;
    public static PlayerHealth Instance;

    void Start()
    {
        stats.currentHealth = stats.maxHealth;
        OnHealthChanged?.Invoke(stats.currentHealth, stats.maxHealth);

    }
    void awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isInvulnerable)
        {
            stats.currentHealth -= amount;
            UnityEngine.Debug.Log("Player's health: " + stats.currentHealth);
            if (stats.currentHealth < 0)
            {
                stats.currentHealth = 0;
                OnHealthChanged?.Invoke(stats.currentHealth, stats.maxHealth);
                //StartCoroutine(DamageEffectsRoutine());
                if(stats.currentHealth <= 0)
                {
                    Die();
                }
            }
        }
    }
    public void Die()
    {
        UnityEngine.Debug.Log("The player has died.");
    }

}
