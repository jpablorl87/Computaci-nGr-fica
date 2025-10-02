using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    [Tooltip("Prefab o modelo del arma que se debe mostrar al equipar")]
    public GameObject weaponModel;

    private void Awake()
    {
        if (weaponModel == null)
            weaponModel = this.gameObject;
    }
}