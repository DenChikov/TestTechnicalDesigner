using System.Collections;
using UnityEngine;

public class PlayerShootSystem : MonoBehaviour, ISystem
{
    private PlayerShootComponent playerShootComponents;
    public void Init()
    {
        playerShootComponents = FindAnyObjectByType<PlayerShootComponent>();
    }
    public void Execute()
    {
        playerShootComponents.patrons.fillAmount = 1f - playerShootComponents.timer / playerShootComponents.timeBetweenShoot;
        if (playerShootComponents.timer > 0) { playerShootComponents.timer -= Time.deltaTime; }

        if (playerShootComponents.timer <= 0 && Input.GetMouseButtonDown(0))
        {
            playerShootComponents.timer = playerShootComponents.timeBetweenShoot;
            StartCoroutine(HitMarker());
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
                Mathf.Infinity, playerShootComponents.enemyDamageLayer))
            {
                if (hit.collider.GetComponentInParent<IHealthBoss>(true) is IHealthBoss hp) hp.health -= playerShootComponents.damage;
            }
        }
    }
     private IEnumerator HitMarker()
 {
     playerShootComponents.markerHit.enabled = true;
     playerShootComponents.particle.Play();
     playerShootComponents.animator.SetBool("Shoot", true);
     yield return new WaitForSeconds(0.5f);
     playerShootComponents.animator.SetBool("Shoot", false);
     playerShootComponents.markerHit.enabled = false;
 }
}
