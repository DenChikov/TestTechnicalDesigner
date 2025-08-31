using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private LayerMask enemyDamageLayer;
    [SerializeField] private float timeBetweenShoot;
    [SerializeField] private Image markerHit;
    [SerializeField]private Image patrons;
    private float timer;
    private Animator animator;
    private ParticleSystem particle;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        particle = GetComponentInChildren<ParticleSystem>();
    }
    private void Update()
    {
        patrons.fillAmount = 1f - timer /timeBetweenShoot;
        if (timer > 0) { timer -= Time.deltaTime; }

        if (timer <= 0 && Input.GetMouseButtonDown(0))
        {
            timer = timeBetweenShoot;
            StartCoroutine(HitMarker());
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
                Mathf.Infinity, enemyDamageLayer))
            {
                hit.collider.transform.root.TryGetComponent<IHealth>(out IHealth health);
                health.TakeDamage(damage);
                
            }
        }
    }
    IEnumerator HitMarker()
    {
        markerHit.enabled = true;
        particle.Play();
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Shoot", false);
        markerHit.enabled = false;
    }
}
