public class WeaponFiringState : WeaponState
{
    public WeaponFiringState(PlayerWeaponStateMachine context) : base(context) { }

    public override void Enter()
    {
        TryFire();
    }

    public override void Tick()
    {
        if (Context.ActiveWeapon == null || Context.ActiveWeapon.CurrentWeaponSO == null)
        {
            Context.SwitchState(Context.IdleState);
            return;
        }

        if (!Context.IsShootHeld || !Context.ActiveWeapon.CurrentWeaponSO.isAutomatic)
        {
            Context.SwitchState(Context.IdleState);
            return;
        }

        if (!Context.ActiveWeapon.HasAmmo)
        {
            Context.SwitchState(Context.IdleState);
            return;
        }

        if (Context.CanFire)
        {
            TryFire();
        }
    }

    public override void HandleShootInput(bool isPressed)
    {
        if (!isPressed)
        {
            Context.SwitchState(Context.IdleState);
        }
    }

    public override void HandleZoomInput(bool isPressed)
    {
        Context.ActiveWeapon?.ApplyZoom(isPressed);
    }

    void TryFire()
    {
        if (Context.ActiveWeapon == null) return;
        if (!Context.CanFire) return;
        if (!Context.ActiveWeapon.HasAmmo) return;
        if (Context.ActiveWeapon.CurrentWeaponSO == null) return;

        Context.RegisterShot();

        if (!Context.ActiveWeapon.CurrentWeaponSO.isAutomatic && Context.Inputs != null)
        {
            Context.Inputs.ShootInput(false);
        }
    }
}

