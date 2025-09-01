using UnityEngine;

public class WaveDamage : MonoBehaviour
{
    public float damage;
    public Vector3 scaleAdd;
    public float timeLife;
    public float innerRadius;
    private Vector3 oldScale;
    private float oldInnerRadius;
    private float time;

    private void OnEnable()
    {
        oldScale = gameObject.transform.localScale;
        oldInnerRadius = innerRadius;
        time = timeLife;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool isTargetInner = Vector3.Distance(other.transform.position, gameObject.transform.position)
                > innerRadius;
            if (isTargetInner)
            {
                other.gameObject.TryGetComponent<Health>(out Health health);
                health.TakeDamage(damage);
            }
        }
    }


    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            gameObject.transform.localScale += scaleAdd;
            innerRadius += scaleAdd.x;
        }
        else
        {
            gameObject.SetActive(false);
        }


    }

    private void OnDisable()
    {
        gameObject.transform.localScale = oldScale;
        innerRadius = oldInnerRadius;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

}
