using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ShieldTrigger : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject shieldPrefab;// Prefab with 2 particle systems (the shield)
    [SerializeField] private ParticleSystem baseCirclePrefab;// Prefab: summon circle at player's feet
    [SerializeField] private GameObject risingEffectPrefab;// Prefab: rising effect from the ground

    [Header("Timing")]
    [SerializeField] private float shieldDuration = 10f;

    private Collider triggerCollider;
    private bool isActive = true;
    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.TryGetComponent<PlayerMovement>(out var player))
        {
            StartCoroutine(ActivateShieldSequence(player));
        }
    }
    private IEnumerator ActivateShieldSequence(PlayerMovement player)
    {
        isActive = false;
        triggerCollider.enabled = false;
        //Instantiate shield and make it follow the player
        GameObject shieldInstance = Instantiate(shieldPrefab, player.transform.position, Quaternion.identity, player.transform);
        ParticleSystem baseCircle = Instantiate(baseCirclePrefab, player.transform.position, Quaternion.identity, player.transform);
        GameObject risingEffect = Instantiate(risingEffectPrefab, player.transform.position, Quaternion.identity, player.transform);

        shieldInstance.SetActive(true);
        baseCircle.Play();

        yield return new WaitForSeconds(shieldDuration);

        baseCircle.Stop();
        shieldInstance.SetActive(false);

        Destroy(shieldInstance);
        Destroy(baseCircle.gameObject, 1f);
        Destroy(risingEffect.gameObject, 1f);

        triggerCollider.enabled = true;
        isActive = true;
    }
}
