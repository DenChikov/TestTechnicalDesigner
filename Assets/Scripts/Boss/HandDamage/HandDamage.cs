using UnityEngine;

public class HandDamage : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Health>(out Health health);
            health.TakeDamage(damage);
        }
    }
}
