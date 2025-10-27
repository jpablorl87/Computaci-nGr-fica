using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}

