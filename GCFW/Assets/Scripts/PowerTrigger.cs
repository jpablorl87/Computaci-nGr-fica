using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class PowerTrigger : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ParticleSystem summonCirclePrefab;//Prefab: summon circle
    [SerializeField] private GameObject risingEffectPrefab;//Prefab: rising effect
    [SerializeField] private GameObject projectilePrefab;//Prefab: projectile
    [SerializeField] private Transform firePoint;//Projectile spawn point
    [SerializeField] private Camera playerCamera;//Camera for aim direction
    [Header("Timing")]
    [SerializeField] private float powerDuration = 10f;
    [SerializeField] private float projectileSpeed = 25f;

    private Collider triggerCollider;
    private bool canShoot = false;
    private bool isActive = true;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }
    private void Update()
    {
        if (canShoot && Input.GetButtonDown("Fire1"))
        {
            ShootProjectile();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.TryGetComponent<PlayerMovement>(out var player))
        {
            StartCoroutine(ActivatePowerSequence(player));
        }
    }
    private IEnumerator ActivatePowerSequence(PlayerMovement player)
    {
        isActive = false;
        triggerCollider.enabled = false;

        canShoot = true;
        // Instantiate summon and rising effects at player's position and make them children
        ParticleSystem summonCircle = Instantiate(summonCirclePrefab, player.transform.position, Quaternion.identity, player.transform);
        GameObject risingEffect = Instantiate(risingEffectPrefab, player.transform.position, Quaternion.identity, player.transform);

        summonCircle.Play();

        yield return new WaitForSeconds(powerDuration);

        canShoot = false;

        summonCircle.Stop();

        Destroy(summonCircle.gameObject, 1f);
        Destroy(risingEffect.gameObject, 1f);

        triggerCollider.enabled = true;
        isActive = true;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || playerCamera == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = playerCamera.transform.forward * projectileSpeed;
        }
    }
}
