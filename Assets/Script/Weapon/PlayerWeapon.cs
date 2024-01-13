using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;





public enum EWeaponType
{
    None,
    Pistol,
    Rifle,
    Shotgun,
}
public class PlayerWeapon : NetworkBehaviour
{
    // �ѱ� ����
    public EWeaponType Type;

    [Header("Fire Setup")]
    public bool IsAutomatic = true; // �ڵ� �߻� ����
    public float Damage = 10f; // ������
    public int FireRate = 100; // �߻� �ӵ�
    [Range(1, 20)]
    public int ProjectilesPerShot = 1; // �� �߿� �߻�Ǵ� ������Ÿ�� ��
    public float Dispersion = 0f; // �л�
    public LayerMask HitMask; // ��Ʈ ����ũ
    public float MaxHitDistance = 100f; // �ִ� ��Ʈ �Ÿ�

    [Header("Ammo")]
    public int MaxClipAmmo = 12; // �ִ� ź�� ��
    public int StartAmmo = 25; // ���� ź�� ��
    public float ReloadTime = 2f; // ������ �ð�

    //[Header("Visuals")]
    //public Sprite Icon; // ������
    //public string Name; // �̸�
    //public Animator Animator; // �ִϸ�����

    [Header("Fire Effect")]
    //public Transform MuzzleTransform; // 3��Ī �������� �߻�Ǵ� ��ġ
    //public GameObject MuzzleEffectPrefab; // �߻� ȿ�� ������
    public Projectile ProjectilePrefab; // ������Ÿ�� �ð�ȭ ������

    //[Header("Sounds")]
    //public AudioSource FireSound; // �߻� ����
    //public AudioSource ReloadingSound; // ������ ����
    //public AudioSource EmptyClipSound; // �� źâ ����

    public bool HasAmmo => ClipAmmo > 0 || RemainingAmmo > 0; // ź���� �ִ��� ����

    [Networked]
    public NetworkBool IsCollected { get; set; } // ȹ�� ����
    [Networked]
    public NetworkBool IsReloading { get; set; } // ������ ����
    [Networked]
    public int ClipAmmo { get; set; } // ���� źâ�� ź�� ��
    [Networked]
    public int RemainingAmmo { get; set; } // ���� ź�� ��

    [Networked]
    private int _fireCount { get; set; } // �߻� Ƚ��
    [Networked]
    private TickTimer _fireCooldown { get; set; } // �߻� ��ٿ� Ÿ�̸�
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> _projectileData { get; } // ������Ÿ�� ������ �迭

    private int _fireTicks; // �߻� ƽ
    private int _visibleFireCount; // �ð�ȭ�� �߻� Ƚ��
    private bool _reloadingVisible; // ������ �ð�ȭ ����
    private GameObject _muzzleEffectInstance; // �߻� ȿ�� �ν��Ͻ�

    //private SceneObjects _sceneObjects; // SceneObjects Ŭ����

    // ���� �߻��ϴ� �޼ҵ�
    public void Fire(Vector3 firePosition, Vector3 fireDirection, bool justPressed)
    {
        // ���� ȹ����� �ʾҰų� �ڵ� �߻簡 �ƴϰų� ������ ���̰ų� �߻� ��ٿ��� ���������� ����
        if (IsCollected == false || (justPressed == false && !IsAutomatic) || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        // źâ�� ź���� ���� ��� �� źâ ���� ��� �� ����
        //if (ClipAmmo <= 0)
        //{
        //    PlayEmptyClipSound(justPressed);
        //    return;
        //}

        // ����ü �߻�
        for (int i = 0; i < ProjectilesPerShot; i++)
        {
            var projectileDirection = fireDirection;

            if (Dispersion > 0f)
            {
                // �л��� ����� �߻� ���� ���
                var dispersionRotation = Quaternion.Euler(Random.insideUnitSphere * Dispersion);
                projectileDirection = dispersionRotation * fireDirection;
            }

            FireProjectile(firePosition, projectileDirection);
        }

        // �߻� ��ٿ� ���� �� ź�� ����
        _fireCooldown = TickTimer.CreateFromTicks(Runner, _fireTicks);
        ClipAmmo--;
    }

    // ������ �޼ҵ�
    public void Reload()
    {
        // ���� ȹ����� �ʾҰų� źâ�� ���� á�ų� ���� ź���� ���ų� ������ ���̰ų� �߻� ��ٿ��� ���������� ����
        if (IsCollected == false || ClipAmmo >= MaxClipAmmo || RemainingAmmo <= 0 || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        // ������ ������ �����ϰ� ������ ��ٿ� ����
        IsReloading = true;
        _fireCooldown = TickTimer.CreateFromSeconds(Runner, ReloadTime);
    }

    // ź�� �߰� �޼ҵ�
    public void AddAmmo(int amount)
    {
        RemainingAmmo += amount;
    }


    // ������ ���� ���¸� �������� �޼ҵ�
    public float GetReloadProgress()
    {
        // ������ ���� �ƴϸ� 1�� ��ȯ
        if (!IsReloading)
            return 1f;

        // ������ ���� ��� ���� ������ �ð��� ���� ���� ��ȯ
        return 1f - _fireCooldown.RemainingTime(Runner).GetValueOrDefault() / ReloadTime;
    }

    // ������Ʈ�� ������ �� ȣ��Ǵ� �޼ҵ�
    public override void Spawned()
    {
        // ���� ������ �ִ� ��쿡�� �ʱ�ȭ �ڵ� ����
        if (HasStateAuthority)
        {
            ClipAmmo = Mathf.Clamp(StartAmmo, 0, MaxClipAmmo);
            RemainingAmmo = StartAmmo - ClipAmmo;
        }
        
        _visibleFireCount = _fireCount;

        float fireTime = 60f / FireRate;
        _fireTicks = Mathf.CeilToInt(fireTime / Runner.DeltaTime);

        // �߻� ȿ�� �ν��Ͻ� ���� �� ��Ȱ��ȭ
       // _muzzleEffectInstance = Instantiate(MuzzleEffectPrefab, MuzzleTransform);
        _muzzleEffectInstance.SetActive(false);

        // SceneObjects Ŭ���� ����
        //_sceneObjects = Runner.GetSingleton<SceneObjects>();
    }

    // ��Ʈ��ũ ������Ʈ �޼ҵ�
    public override void FixedUpdateNetwork()
    {
        // ���� ȹ����� �ʾҴٸ� ����
        if (IsCollected == false)
            return;

        // źâ�� ��������� �ڵ����� ������ �õ�
        if (ClipAmmo == 0)
            Reload();

        // ������ ���̸� ������ ��ٿ��� ������ ��
        if (IsReloading && _fireCooldown.ExpiredOrNotRunning(Runner))
        {
            // ������ �Ϸ�
            IsReloading = false;

            // �������� �� �ִ� �ִ� ź�� �� ���
            int reloadAmmo = MaxClipAmmo - ClipAmmo;
            reloadAmmo = Mathf.Min(reloadAmmo, RemainingAmmo);

            // ź�� �߰� �� ���� ź�� ����
            ClipAmmo += reloadAmmo;
            RemainingAmmo -= reloadAmmo;

            // ������ �� �غ� �ð� �߰�
            _fireCooldown = TickTimer.CreateFromSeconds(Runner, 0.25f);
        }
    }

    // ������ �޼ҵ�
    public override void Render()
    {
        // �߻� Ƚ���� ����Ǿ����� �߻� ȿ�� ���
        //if (_visibleFireCount < _fireCount)
        //{
        //    PlayFireEffect();
        //}

        // ���� ǥ�õ��� ���� ��� ������Ÿ�Ͽ� ���� �ð����� ó��
        //for (int i = _visibleFireCount; i < _fireCount; i++)
        //{
        //    var data = _projectileData[i % _projectileData.Length];
        //    var muzzleTransform = MuzzleTransform;

        //    var projectileVisual = Instantiate(ProjectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
        //    projectileVisual.SetHit(data.HitPosition, data.HitNormal, data.ShowHitEffect);
        //}

        _visibleFireCount = _fireCount;

        if (_reloadingVisible != IsReloading)
        {
            // ���ο� ������ ���¿� ���� �ִϸ��̼� �� �Ҹ��� ó���մϴ�.
            //Animator.SetBool("IsReloading", IsReloading);

            if (IsReloading)
            {
                // ������ �߿��� ���ε� ���带 ����մϴ�.
               // ReloadingSound.Play();
            }

            _reloadingVisible = IsReloading;
        }
    }

    private void FireProjectile(Vector3 firePosition, Vector3 fireDirection)
    {
        var projectileData = new ProjectileData();

        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        // ��ü �߻�ü ��� �� ȿ���� ��� ó���մϴ�(��Ʈ��ĵ �߻�ü).
        if (Runner.LagCompensation.Raycast(firePosition, fireDirection, MaxHitDistance,
                Object.InputAuthority, out var hit, HitMask, hitOptions))
        {
            projectileData.HitPosition = hit.Point;
            projectileData.HitNormal = hit.Normal;

            if (hit.Hitbox != null)
            {
                // ��Ʈ�ڽ��� �ִ� ��� �������� �����մϴ�.
                ApplyDamage(hit.Hitbox, hit.Point, fireDirection);
            }
            else
            {
                // �÷��̾ �ܴ��� ��ü�� �浹���� ���� ��Ʈ ȿ���� ǥ���մϴ�.
                projectileData.ShowHitEffect = true;
            }
        }

        _projectileData.Set(_fireCount % _projectileData.Length, projectileData);
        _fireCount++;
    }

    //private void PlayFireEffect()
    //{
    //    if (FireSound != null)
    //    {
    //        // �߻� ���带 �� �� ����մϴ�.
    //        FireSound.PlayOneShot(FireSound.clip);
    //    }

    //    // �ѱ� ����Ʈ ���ü��� �缳���մϴ�.
    //    _muzzleEffectInstance.SetActive(false);
    //    _muzzleEffectInstance.SetActive(true);

    //    // �߻� �ִϸ��̼��� ����մϴ�.
    //    Animator.SetTrigger("Fire");

    //    // �θ� Player�� �߻� ����Ʈ�� ����մϴ�.
    //    GetComponentInParent<CharacterMovementHandler>().PlayFireEffect();
    //}

    private void ApplyDamage(Hitbox enemyHitbox, Vector3 position, Vector3 direction)
    {
        //var enemyHealth = enemyHitbox.Root.GetComponent<Health>();
        //if (enemyHealth == null || enemyHealth.IsAlive == false)
        //    return;

        //float damageMultiplier = enemyHitbox is BodyHitbox bodyHitbox ? bodyHitbox.DamageMultiplier : 1f;
        //bool isCriticalHit = damageMultiplier > 1f;

        //float damage = Damage * damageMultiplier;
        //if (_sceneObjects.Gameplay.DoubleDamageActive)
        //{
        //    // ���� ������ Ȱ��ȭ ���̸� �������� �� ��� ������ŵ�ϴ�.
        //    damage *= 2f;
        //}

        //if (enemyHealth.ApplyDamage(Object.InputAuthority, damage, position, direction, Type, isCriticalHit) == false)
        //    return;

        //if (HasInputAuthority && Runner.IsForward)
        //{
        //    // ���� �÷��̾�� UI ��Ʈ ����Ʈ�� ǥ���մϴ�.
        //   // _sceneObjects.GameUI.PlayerView.Crosshair.ShowHit(enemyHealth.IsAlive == false, isCriticalHit);
        //}
    }

    //private void PlayEmptyClipSound(bool fireJustPressed)
    //{
    //    // �ڵ� ������ ��� ������ �߻� �Ŀ� �� źâ ���带 �� �� ����Ϸ��� �մϴ�.
    //    bool firstEmptyShot = _fireCooldown.TargetTick.GetValueOrDefault() == Runner.Tick - 1;

    //    if (fireJustPressed == false && firstEmptyShot == false)
    //        return;

    //    if (EmptyClipSound == null || EmptyClipSound.isPlaying)
    //        return;

    //    if (Runner.IsForward && HasInputAuthority)
    //    {
    //        // �� źâ ���带 ����մϴ�.
    //        EmptyClipSound.Play();
    //    }
    //}

    /// <summary>
    /// ���� �߻�ü �߻縦 ��Ÿ���� ����ü�Դϴ�.
    /// </summary>
    private struct ProjectileData : INetworkStruct
    {
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public NetworkBool ShowHitEffect;
    }
}

