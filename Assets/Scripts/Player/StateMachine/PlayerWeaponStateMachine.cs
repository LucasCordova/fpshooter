using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
public class PlayerWeaponStateMachine : MonoBehaviour
{
    public WeaponState CurrentState { get; private set; }
    public WeaponIdleState IdleState { get; private set; }
    public WeaponFiringState FiringState { get; private set; }
    public WeaponSwitchingState SwitchingState { get; private set; }

    public ActiveWeapon ActiveWeapon { get; private set; }
    public StarterAssetsInputs Inputs => ActiveWeapon != null ? ActiveWeapon.Inputs : null;

    float fireCooldownTimer;
    WeaponSO pendingWeapon;

    public bool IsShootHeld { get; private set; }
    public bool IsZoomHeld { get; private set; }

    public bool CanFire => ActiveWeapon != null &&
                           ActiveWeapon.CurrentWeaponSO != null &&
                           fireCooldownTimer >= ActiveWeapon.CurrentWeaponSO.FireRate;

    void Awake()
    {
        ActiveWeapon = GetComponent<ActiveWeapon>();

        IdleState = new WeaponIdleState(this);
        FiringState = new WeaponFiringState(this);
        SwitchingState = new WeaponSwitchingState(this);
    }

    void Update()
    {
        if (ActiveWeapon?.CurrentWeaponSO != null)
        {
            fireCooldownTimer += Time.deltaTime;
        }

        if (Inputs != null)
        {
            IsShootHeld = Inputs.shoot;
            IsZoomHeld = Inputs.zoom;
        }
        else
        {
            IsShootHeld = false;
            IsZoomHeld = false;
        }

        CurrentState?.HandleShootInput(IsShootHeld);
        CurrentState?.HandleZoomInput(IsZoomHeld);
        CurrentState?.Tick();
    }

    public void Initialize(WeaponSO startingWeapon)
    {
        if (startingWeapon == null) return;

        ActiveWeapon.EquipWeapon(startingWeapon);
        fireCooldownTimer = ActiveWeapon.CurrentWeaponSO != null ? ActiveWeapon.CurrentWeaponSO.FireRate : 0f;
        SwitchState(IdleState);
    }

    public void SwitchState(WeaponState newState)
    {
        if (newState == null || newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void RequestWeaponSwitch(WeaponSO weapon)
    {
        if (weapon == null) return;
        pendingWeapon = weapon;
        SwitchState(SwitchingState);
    }

    public WeaponSO ConsumePendingWeapon()
    {
        WeaponSO weapon = pendingWeapon;
        pendingWeapon = null;
        return weapon;
    }

    public void RegisterShot()
    {
        ActiveWeapon.FireCurrentWeapon();
        fireCooldownTimer = 0f;
    }
}

