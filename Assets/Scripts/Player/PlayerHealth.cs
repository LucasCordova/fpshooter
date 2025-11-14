using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealthSubject
{
    [Range(1, 10)]
    [SerializeField] int startingHealth = 5;
    [SerializeField] CinemachineVirtualCamera deathVirtualCamera;
    [SerializeField] Transform weaponCamera;
    [SerializeField] GameObject gameOverContainer;
    [SerializeField] MonoBehaviour[] defaultObservers = new MonoBehaviour[0];
    [SerializeField] bool autoRegisterInspectorObservers = true;

    readonly List<IHealthObserver> observers = new List<IHealthObserver>(8);
    int currentHealth;
    bool isInvulnerable;
    int gameOverVirtualCameraPriority = 20;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => startingHealth;
    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        ResetHealth();
    }

    void OnEnable()
    {
        if (autoRegisterInspectorObservers)
        {
            RegisterDefaultObservers();
        }
    }

    void OnDisable()
    {
        if (autoRegisterInspectorObservers)
        {
            UnregisterDefaultObservers();
        }
    }

    public void ResetHealth()
    {
        currentHealth = Mathf.Clamp(startingHealth, 1, int.MaxValue);
        NotifyObservers(currentHealth, currentHealth, 0, DamageType.Unknown);
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, DamageType.Unknown);
    }

    public void TakeDamage(int amount, DamageType cause)
    {
        if (amount <= 0 || isInvulnerable || IsDead)
        {
            return;
        }

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);
        NotifyObservers(previous, currentHealth, currentHealth - previous, cause);

        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        NotifyObservers(previous, currentHealth, currentHealth - previous, DamageType.Unknown);
    }

    public void SetInvulnerable(bool shouldBeInvulnerable)
    {
        isInvulnerable = shouldBeInvulnerable;
    }

    public void Register(IHealthObserver observer)
    {
        if (observer == null || observers.Contains(observer))
        {
            return;
        }

        observers.Add(observer);
    }

    public void Unregister(IHealthObserver observer)
    {
        if (observer == null)
        {
            return;
        }

        observers.Remove(observer);
    }

    void NotifyObservers(int previousValue, int currentValue, int delta, DamageType cause)
    {
        PurgeDestroyedObservers();

        if (observers.Count == 0)
        {
            return;
        }

        HealthChangePayload payload = new HealthChangePayload(this, previousValue, currentValue, delta, cause);
        var snapshot = observers.ToArray();

        for (int i = 0; i < snapshot.Length; i++)
        {
            snapshot[i].OnHealthChanged(payload);
        }
    }

    void PurgeDestroyedObservers()
    {
        for (int i = observers.Count - 1; i >= 0; i--)
        {
            if (observers[i] == null)
            {
                observers.RemoveAt(i);
            }
        }
    }

    void RegisterDefaultObservers()
    {
        for (int i = 0; i < defaultObservers.Length; i++)
        {
            if (defaultObservers[i] is IHealthObserver healthObserver)
            {
                Register(healthObserver);
            }
            else if (defaultObservers[i] != null)
            {
                Debug.LogWarning($"{name} default observer at index {i} does not implement IHealthObserver.", this);
            }
        }
    }

    void UnregisterDefaultObservers()
    {
        for (int i = 0; i < defaultObservers.Length; i++)
        {
            if (defaultObservers[i] is IHealthObserver healthObserver)
            {
                Unregister(healthObserver);
            }
        }
    }

    void PlayerGameOver()
    {
        if (weaponCamera != null)
        {
            weaponCamera.parent = null;
        }

        if (deathVirtualCamera != null)
        {
            deathVirtualCamera.Priority = gameOverVirtualCameraPriority;
        }

        if (gameOverContainer != null)
        {
            gameOverContainer.SetActive(true);
        }

        StarterAssetsInputs starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.SetCursorState(false);
        }

        Destroy(gameObject);
    }
}
