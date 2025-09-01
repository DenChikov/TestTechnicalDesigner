using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotation;
    [SerializeField] private float lifeTime;
    [SerializeField] private float damage;
    private float oldLifeTime;
    private GameObject player;
    private Rigidbody rb;
    private void Awake()
    {
        oldLifeTime = lifeTime;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        lifeTime = oldLifeTime;
    }
    private void FixedUpdate()
    {
        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime < 0)
        {
            gameObject.SetActive(false);
            return;
        }
        Vector3 direction = ((player.transform.position + new Vector3(0, 1f,0)) - rb.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, lookRotation, rotation));
        rb.linearVelocity = transform.forward * speed;

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Health>(out Health health);
            health.TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }

}
