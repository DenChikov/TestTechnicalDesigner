using UnityEngine;
using static PhaseEnum;

public class ShootSystem : MonoBehaviour, ISystem
{
    private BossCombatDataComponent data;
    private ShootComponent shoot;
    private HandComponent hand;
    private BulletComponent[] bull;
    private PlayerHealthDataComponent playerHealth;

    public void Init()
    {
        data = FindAnyObjectByType<BossCombatDataComponent>();
        shoot = FindAnyObjectByType<ShootComponent>();
        hand = FindAnyObjectByType<HandComponent>();
        playerHealth = FindAnyObjectByType<PlayerHealthDataComponent>();
        CreatePool();
    }
    public void Execute()
    {
        MoveBullet();
        if (data == null) return;

        if (data.phaseData.Value == Phase.Idle)
        {
            TryEnterCombatIfPlayerInAggroSphere();
        }

        if (data.phaseData.Value == Phase.Shooting)
        {
            Shoot();
        }
    }

    //aggro boss
    private void TryEnterCombatIfPlayerInAggroSphere()
    {
        if (data == null || data.aggroRadius <= 0f)
            return;

        if (Physics.CheckSphere(data.transform.position, data.aggroRadius, data.playerLayerMask,
            QueryTriggerInteraction.Collide))
            data.phaseData.Value = Phase.Shooting;
    }
    private void OnDrawGizmos()
    {

        BossCombatDataComponent d = data;
        if (d == null)
            d = FindAnyObjectByType<BossCombatDataComponent>();

        if (d == null || d.aggroRadius <= 0f)
            return;

        Gizmos.color = d.phaseData.Value == Phase.Idle
            ? new Color(0f, 0.85f, 1f, 1f)
            : new Color(1f, 0.55f, 0f, 1f);
        Gizmos.DrawWireSphere(d.transform.position, d.aggroRadius);
    }

    //create pool bullet

    private void CreatePool()//create pool bullet
    {
        if (shoot == null)
            return;

        bull = new BulletComponent[shoot.poolSize];

        for (int i = 0; i < shoot.poolSize; i++)
        {
            GameObject bullet = Instantiate(shoot.bulletPrefab);
            bullet.SetActive(false);
            shoot.bulletPool.Add(bullet);

            // Сохраняем компонент пули в наш массив
            bull[i] = bullet.GetComponent<BulletComponent>();
        }
    }
    private GameObject GetPool() //get bullet
    {
        if (shoot == null)
            return null;

        foreach (var bullet in shoot.bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }

    //shoot logic


    private void Shoot() //spawn bullet
    {
        if (shoot == null)
            return;


        data.transform.LookAt(data.player.transform, Vector3.up);// look for player

        shoot.time += Time.deltaTime;
        if (shoot.time > shoot.timeBetweenShoot)
        {
            GameObject bullet = GetPool();
            if (bullet != null)
            {
                bullet.transform.position = shoot.bulletSpawn.transform.position;
                if (bullet.TryGetComponent<BulletComponent>(out var bc))
                {
                    bc.currentLifeTime = bc.bulletLifeTime;
                }
                bullet.SetActive(true);
            }
            shoot.time = 0;
            shoot.valueShoot += 1;
        }
        if (shoot.valueShoot >= shoot.valueShootMax)
        {
            shoot.valueShoot = 0;
            data.phaseData.Value = Phase.Hand;
        }

    }

    //move bullet
    private void MoveBullet()
    {
        if (bull == null) return;

        for (int i = 0; i < bull.Length; i++)
        {
            BulletComponent bulletComp = bull[i];
            if (!bulletComp.gameObject.activeInHierarchy) continue; 
            if (bulletComp != null && bulletComp.gameObject.activeInHierarchy)
            {
                bulletComp.currentLifeTime -= Time.deltaTime;
                if (bulletComp.currentLifeTime <= 0)
                {
                    bulletComp.gameObject.SetActive(false);
                    continue;
                }
                // Поворот к игроку
                Vector3 direction = ((data.player.transform.position + Vector3.up) -
                    bulletComp.gameObject.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                bulletComp.gameObject.transform.rotation =
                    Quaternion.RotateTowards(bulletComp.gameObject.transform.rotation, targetRotation,
                    bulletComp.bulletRotationSpeed * Time.deltaTime);

                // Движение вперёд
                bulletComp.gameObject.transform.position +=
                    bulletComp.gameObject.transform.forward * bulletComp.bulletSpeed * Time.deltaTime;

                // OverlapSphere даёт реальные коллайдеры — применяем урон напрямую в данные,
                // не завися от OnTriggerEnter (который не срабатывает при ручном движении transform)
                Collider[] hits = Physics.OverlapSphere(
                    bulletComp.gameObject.transform.position,
                    bulletComp.sphereCollider.radius * bulletComp.gameObject.transform.lossyScale.x,
                    bulletComp.bulletCollisionMask);

                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        if (hit.TryGetComponent<PlayerHealthDataComponent>(out var health))
                        {
                            if (bulletComp.TryGetComponent<DamageComponent>(out var dmg))
                                health.pendingDamage += dmg.damage;
                        }
                    }
                    bulletComp.gameObject.SetActive(false);
                }
            }
        }
    }
}
