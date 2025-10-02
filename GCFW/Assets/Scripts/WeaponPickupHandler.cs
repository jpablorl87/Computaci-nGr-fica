using UnityEngine;

public class WeaponPickupHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform weaponHolder;

    [Header("Settings")]
    [SerializeField] private float pickupRange = 2f;

    private GameObject equippedWeapon;

    private void Update()
    {
        if (inputHandler.IsInteractPressed)
        {
            TryPickupWeapon();
        }
    }

    private void TryPickupWeapon()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var hitCollider in hitColliders)
        {
            var pickup = hitCollider.GetComponent<PickupWeapon>();
            if (pickup != null && pickup.weaponModel != null)
            {
                EquipWeapon(pickup.weaponModel, hitCollider.gameObject);
                break;
            }
        }
    }

    private void EquipWeapon(GameObject weapon, GameObject originalObject)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon);

        // Reposicionar el arma en el holder
        originalObject.transform.SetParent(weaponHolder);
        originalObject.transform.localPosition = Vector3.zero;
        originalObject.transform.localRotation = Quaternion.identity;

        // Desactivar física y collider
        if (originalObject.TryGetComponent(out Collider col))
            col.enabled = false;
        if (originalObject.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        equippedWeapon = originalObject;
    }
}