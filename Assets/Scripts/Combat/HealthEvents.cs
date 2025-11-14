using System;
using UnityEngine;

/// <summary>
/// Represents the type of damage applied to a health subject.
/// </summary>
public enum DamageType
{
    Unknown = 0,
    Bullet = 1,
    Explosion = 2,
    Environmental = 3,
    Melee = 4
}

/// <summary>
/// Bundles contextual data for an observer notification.
/// </summary>
public readonly struct HealthChangePayload
{
    public readonly PlayerHealth Subject;
    public readonly int PreviousHealth;
    public readonly int CurrentHealth;
    public readonly int Delta;
    public readonly DamageType Cause;

    public HealthChangePayload(PlayerHealth subject, int previous, int current, int delta, DamageType cause)
    {
        Subject = subject;
        PreviousHealth = previous;
        CurrentHealth = current;
        Delta = delta;
        Cause = cause;
    }
}

/// <summary>
/// Implemented by anything interested in reacting to health changes.
/// </summary>
public interface IHealthObserver
{
    void OnHealthChanged(HealthChangePayload payload);
}

/// <summary>
/// Implemented by any component that wishes to broadcast health changes.
/// </summary>
public interface IHealthSubject
{
    void Register(IHealthObserver observer);
    void Unregister(IHealthObserver observer);
}

