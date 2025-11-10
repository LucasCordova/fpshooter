public class WeaponSwitchingState : WeaponState
{
    public WeaponSwitchingState(PlayerWeaponStateMachine context) : base(context) { }

    public override void Enter()
    {
        WeaponSO pendingWeapon = Context.ConsumePendingWeapon();
        if (pendingWeapon != null)
        {
            Context.Inputs?.ShootInput(false);
            Context.ActiveWeapon?.EquipWeapon(pendingWeapon);
        }

        Context.SwitchState(Context.IdleState);
    }
}

