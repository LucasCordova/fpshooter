using UnityEngine;

public abstract class WeaponState
{
    protected readonly PlayerWeaponStateMachine Context;

    protected WeaponState(PlayerWeaponStateMachine context)
    {
        Context = context;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
    public virtual void HandleShootInput(bool isPressed) { }
    public virtual void HandleZoomInput(bool isPressed) { }
}

