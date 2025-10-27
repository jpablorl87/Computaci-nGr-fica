using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject portalPrefab;//Prefab with portal particles
    [SerializeField] private ParticleSystem summonCirclePrefab;//Prefab: summon circle
    [SerializeField] private GameObject risingEffectPrefab;//Prefab: rising effect
    [SerializeField] private Transform teleportTarget;//Destination point
    [SerializeField] private Vector3 portalOffset = new Vector3(0, 0, 2f);//Offset from trigger

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
            StartCoroutine(ActivatePortalSequence(player));
        }
    }
    private IEnumerator ActivatePortalSequence(PlayerMovement player)
    {
        isActive = false;
        triggerCollider.enabled = false;
        // Instantiate portal and effects
        GameObject portalInstance = Instantiate(portalPrefab, transform.position + portalOffset, Quaternion.identity);
        ParticleSystem summonCircle = Instantiate(summonCirclePrefab, player.transform.position, Quaternion.identity, player.transform);
        GameObject risingEffect = Instantiate(risingEffectPrefab, player.transform.position, Quaternion.identity, player.transform);

        summonCircle.Play();
        // Wait until player touches the portal (any side)
        bool playerTeleported = false;
        while (!playerTeleported)
        {
            float distance = Vector3.Distance(player.transform.position, portalInstance.transform.position);
            if (distance < 1.2f) // radius of activation
            {
                playerTeleported = true;
            }
            yield return null;
        }
        // Stop effects and teleport
        summonCircle.Stop();

        if (teleportTarget != null)
            player.transform.position = teleportTarget.position;
        // Clean up
        Destroy(portalInstance);
        Destroy(summonCircle.gameObject, 1f);
        Destroy(risingEffect.gameObject, 1f);

        triggerCollider.enabled = true;
        isActive = true;
    }
}
