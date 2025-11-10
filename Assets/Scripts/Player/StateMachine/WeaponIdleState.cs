public class WeaponIdleState : WeaponState
{
    public WeaponIdleState(PlayerWeaponStateMachine context) : base(context) { }

    public override void HandleShootInput(bool isPressed)
    {
        if (!isPressed) return;
        if (!Context.CanFire) return;
        if (Context.ActiveWeapon == null || !Context.ActiveWeapon.HasAmmo) return;

        Context.SwitchState(Context.FiringState);
    }

    public override void HandleZoomInput(bool isPressed)
    {
        Context.ActiveWeapon?.ApplyZoom(isPressed);
    }

}

