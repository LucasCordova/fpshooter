using UnityEngine;
using StarterAssets;
using Cinemachine;
using TMPro;

[RequireComponent(typeof(PlayerWeaponStateMachine))]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO startingWeapon;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] Camera weaponCamera;
    [SerializeField] GameObject zoomVignette;
    [SerializeField] TMP_Text ammoText;

    WeaponSO currentWeaponSO;
    Animator animator;
    StarterAssetsInputs starterAssetsInputs;
    FirstPersonController firstPersonController;
    Weapon currentWeapon;
    PlayerWeaponStateMachine stateMachine;

    const string SHOOT_STRING = "Shoot";

    float defaultFOV;
    float defaultRotationSpeed;
    int currentAmmo;

    void Awake()
    {
        stateMachine = GetComponent<PlayerWeaponStateMachine>();
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        firstPersonController = GetComponentInParent<FirstPersonController>();
        animator = GetComponent<Animator>();
        defaultFOV = playerFollowCamera.m_Lens.FieldOfView;
        defaultRotationSpeed = firstPersonController.RotationSpeed;
    }

    void Start()
    {
        if (startingWeapon == null) return;

        if (stateMachine != null)
        {
            stateMachine.Initialize(startingWeapon);
        }
        else
        {
            EquipWeapon(startingWeapon);
        }
    }

    public StarterAssetsInputs Inputs => starterAssetsInputs;
    public FirstPersonController FirstPersonController => firstPersonController;
    public Animator WeaponAnimator => animator;
    public WeaponSO CurrentWeaponSO => currentWeaponSO;
    public bool HasAmmo => currentAmmo > 0;

    public void AdjustAmmo(int amount)
    {
        if (currentWeaponSO == null) return;

        currentAmmo += amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, currentWeaponSO.MagazineSize);
        UpdateAmmoDisplay();
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;

        if (stateMachine != null)
        {
            stateMachine.RequestWeaponSwitch(weaponSO);
        }
        else
        {
            EquipWeapon(weaponSO);
        }
    }

    internal void EquipWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;

        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }

        Weapon newWeapon = Instantiate(weaponSO.weaponPrefab, transform).GetComponent<Weapon>();
        currentWeapon = newWeapon;
        currentWeaponSO = weaponSO;
        SetAmmo(currentWeaponSO.MagazineSize);
        ResetZoom();
    }

    internal void FireCurrentWeapon()
    {
        if (!currentWeapon || currentWeaponSO == null || !HasAmmo) return;

        currentWeapon.Shoot(currentWeaponSO);
        animator.Play(SHOOT_STRING, 0, 0f);
        AdjustAmmo(-1);
    }

    internal void ApplyZoom(bool isZooming)
    {
        if (currentWeaponSO == null || !currentWeaponSO.CanZoom)
        {
            ResetZoom();
            return;
        }

        if (isZooming)
        {
            playerFollowCamera.m_Lens.FieldOfView = currentWeaponSO.ZoomAmount;
            weaponCamera.fieldOfView = currentWeaponSO.ZoomAmount;
            if (zoomVignette) zoomVignette.SetActive(true);
            firstPersonController.ChangeRotationSpeed(currentWeaponSO.ZoomRotationSpeed);
        }
        else
        {
            ResetZoom();
        }
    }

    internal void ResetZoom()
    {
        playerFollowCamera.m_Lens.FieldOfView = defaultFOV;
        weaponCamera.fieldOfView = defaultFOV;
        if (zoomVignette) zoomVignette.SetActive(false);
        firstPersonController.ChangeRotationSpeed(defaultRotationSpeed);
    }

    void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, currentWeaponSO != null ? currentWeaponSO.MagazineSize : amount);
        UpdateAmmoDisplay();
    }

    void UpdateAmmoDisplay()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString("D2");
        }
    }
}
