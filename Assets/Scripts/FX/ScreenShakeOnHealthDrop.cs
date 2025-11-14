using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ScreenShakeOnHealthDrop : MonoBehaviour, IHealthObserver
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] float baseAmplitude = 0.3f;
    [SerializeField] float damageScale = 1.0f;
    [SerializeField] int criticalHealthThreshold = 1;

    CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.Register(this);
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.Unregister(this);
        }
    }

    public void OnHealthChanged(HealthChangePayload payload)
    {
        if (impulseSource == null || payload.Delta >= 0)
        {
            return;
        }

        float normalizedDamage = Mathf.Clamp01(Mathf.Abs(payload.Delta) / payload.Subject.MaxHealth);
        float amplitude = baseAmplitude + normalizedDamage * damageScale;

        if (payload.CurrentHealth <= criticalHealthThreshold)
        {
            amplitude *= 1.5f;
        }

        impulseSource.GenerateImpulse(Vector3.up * amplitude);
    }
}

