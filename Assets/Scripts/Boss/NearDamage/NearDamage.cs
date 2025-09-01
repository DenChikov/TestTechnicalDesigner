using UnityEngine;

public class NearDamage : HandDamage
{
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Health>(out Health health);
            health.TakeDamage(damage);
        }
    }
}
