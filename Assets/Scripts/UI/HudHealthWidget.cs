using UnityEngine;
using UnityEngine.UI;

public class HudHealthWidget : MonoBehaviour, IHealthObserver
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Image[] shieldBars = new Image[0];
    [SerializeField] Animator animator;
    [SerializeField] string damageTriggerName = "Damage";
    [SerializeField] string healTriggerName = "Heal";

    void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.Register(this);
            UpdateShieldBars(playerHealth.CurrentHealth);
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
        UpdateShieldBars(payload.CurrentHealth);
        TriggerAnimation(payload.Delta);
    }

    void UpdateShieldBars(int currentHealth)
    {
        if (shieldBars == null)
        {
            return;
        }

        for (int i = 0; i < shieldBars.Length; i++)
        {
            if (shieldBars[i] == null)
            {
                continue;
            }

            shieldBars[i].gameObject.SetActive(i < currentHealth);
        }
    }

    void TriggerAnimation(int delta)
    {
        if (animator == null || delta == 0)
        {
            return;
        }

        if (delta < 0 && !string.IsNullOrEmpty(damageTriggerName))
        {
            animator.SetTrigger(damageTriggerName);
        }
        else if (delta > 0 && !string.IsNullOrEmpty(healTriggerName))
        {
            animator.SetTrigger(healTriggerName);
        }
    }
}

