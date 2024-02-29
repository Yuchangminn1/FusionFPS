using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Setup")]
    public float Speed = 80f;
    public float MaxDistance = 100f;
    public float LifeTimeAfterHit = 2f;

    [Header("Impact Setup")]
    public GameObject ProjectileObject;
    public GameObject HitEffectPrefab;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private Vector3 _hitNormal;

    private bool _showHitEffect;
    private GameObject _hitEffectPrefab;

    private float _startTime;
    private float _duration;

    /// <summary>
    /// Set where the projectile visual should land.                                ź�� ���־��� ��ź�� ��ġ�� �����մϴ�.
    /// </summary>
    /// <param name="hitPosition">Position where projectile hit geometry</param>    ź���� ����ü�� ���� ��ġ
    /// <param name="hitNormal">Normal of the hit surface</param>                   ���� ǥ���� ���� ����
    /// <param name="showHitEffect">Whether projectile impact should be displayed   ź�� �浹 ȿ���� ǥ������ ����
    /// (e.g. we don't want static impact effect displayed on other player's body)</param>
    public void SetHit(Vector3 hitPosition, Vector3 hitNormal, bool showHitEffect)
    {
        _targetPosition = hitPosition;
        _showHitEffect = showHitEffect;
        _hitNormal = hitNormal;
    }

    private void Start()
    {
        _startPosition = transform.position;

        if (_targetPosition == Vector3.zero)
        {
            _targetPosition = _startPosition + transform.forward * MaxDistance;
        }

        _duration = Vector3.Distance(_startPosition, _targetPosition) / Speed;
        _startTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        float time = Time.timeSinceLevelLoad - _startTime;

        if (time < _duration)
        {
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, time / _duration);
        }
        else
        {
            transform.position = _targetPosition;
            FinishProjectile();
        }
    }

    private void FinishProjectile()
    {
        if (_showHitEffect == false)
        {
            // No hit effect, destroy immediately.
            Destroy(gameObject);
            return;
        }

        // Stop updating projectile visual.
        enabled = false;

        if (ProjectileObject != null)
        {
            ProjectileObject.SetActive(false);
        }

        if (HitEffectPrefab != null)
        {
            Instantiate(HitEffectPrefab, _targetPosition, Quaternion.LookRotation(_hitNormal), transform);
        }

        Destroy(gameObject, LifeTimeAfterHit);
    }
}

