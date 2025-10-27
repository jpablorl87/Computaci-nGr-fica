using UnityEngine;

public class ShieldReaction : MonoBehaviour
{
    [SerializeField] private GameObject shieldVFX;
    [SerializeField] private Material material;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            var reaction = Instantiate(shieldVFX, transform) as GameObject;
            var psr = reaction.GetComponent<ParticleSystemRenderer>();
            material = psr.material;
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            material.SetVector("_SphereCenter", contactPoint);
            Destroy(reaction,2);
        }
    }/*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var reaction = Instantiate(shieldVFX, transform) as GameObject;
            var psr = reaction.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            material = psr.material;
            material.SetVector("SphereCenter", collision.contacts[0].point);
            Destroy(reaction, 2);
        }
    }*/
}
