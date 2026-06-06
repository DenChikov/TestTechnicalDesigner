using UnityEngine;

public class ConfigurationSystem : MonoBehaviour, ISystem
{
    [Tooltip("Единый ScriptableObject с балансом игры. Создать через Create → NewECS → Game Balance Config.")]
    [SerializeField] private GameBalanceConfig config;

    public void Init()
    {
        if (config == null)
        {
            Debug.LogError("ConfigurationSystem: GameBalanceConfig не назначен!", this);
            return;
        }

        ApplyToSceneComponents();
    }
    public void ApplyRuntimeFromConfig()
    {
        if (config == null) return;

        ApplyToRuntimeEntities();
    }

    public void Execute() { }

    // ============== Scene-level ==============

    private void ApplyToSceneComponents()
    {
        ApplyPlayerHealth();
        ApplyPlayerShoot();
        ApplyBossHealth();
        ApplyBossCombat();
        ApplyHand();
        ApplyWave();
        ApplyShoot();
        ApplySceneDamageComponents();
        ApplyLootBoxes();
    }

    private void ApplyPlayerHealth()
    {
        var components = FindObjectsByType<PlayerHealthDataComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.healthMax = config.player.health.healthMax;
        }
    }

    private void ApplyPlayerShoot()
    {
        var components = FindObjectsByType<PlayerShootComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.damage = config.player.shoot.damage;
            c.timeBetweenShoot = config.player.shoot.timeBetweenShoot;
            c.shootRayRadius = config.player.shoot.shootRayRadius;
        }
    }

    private void ApplyBossHealth()
    {
        var components = FindObjectsByType<BossHealthComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.maxHealth = config.boss.health.maxHealth;
        }
    }

    private void ApplyBossCombat()
    {
        var components = FindObjectsByType<BossCombatDataComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.aggroRadius = config.boss.combat.aggroRadius;
        }
    }

    private void ApplyHand()
    {
        var components = FindObjectsByType<HandComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.windUpTime = config.boss.hand.windUpTime;
            c.damageZoneTime = config.boss.hand.damageZoneTime;
            c.punchTime = config.boss.hand.punchTime;
            c.returnTime = config.boss.hand.returnTime;
        }
    }

    private void ApplyWave()
    {
        var components = FindObjectsByType<WaveComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.scaleAdd = config.boss.wave.scaleAdd;
            c.timeLife = config.boss.wave.timeLife;
            c.outsideRadius = config.boss.wave.outsideRadius;
            c.innerRadius = config.boss.wave.innerRadius;
        }
    }

    private void ApplyShoot()
    {
        var components = FindObjectsByType<ShootComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.poolSize = config.boss.shoot.poolSize;
            c.timeBetweenShoot = config.boss.shoot.timeBetweenShoot;
            c.valueShootMax = config.boss.shoot.valueShootMax;
        }
    }

    private void ApplySceneDamageComponents()
    {
        var components = FindObjectsByType<DamageComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.damage = GetDamageByType(c.damageType);
        }
    }

    private void ApplyLootBoxes()
    {
        var components = FindObjectsByType<LootBoxComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var c in components)
        {
            c.baseChance        = config.loot.baseChance;
            c.chanceIncrement   = config.loot.chanceIncrement;
            c.hardPityThreshold = config.loot.hardPityThreshold;
        }
    }

    // ============== Runtime entities ==============

    private void ApplyToRuntimeEntities()
    {
        var shoots = FindObjectsByType<ShootComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var shoot in shoots)
        {
            if (shoot.bulletPool == null) continue;

            foreach (var bullet in shoot.bulletPool)
            {
                if (bullet == null) continue;

                if (bullet.TryGetComponent<BulletComponent>(out var b))
                {
                    b.bulletSpeed = config.boss.bullet.bulletSpeed;
                    b.bulletRotationSpeed = config.boss.bullet.bulletRotationSpeed;
                    b.bulletLifeTime = config.boss.bullet.bulletLifeTime;
                }

                if (bullet.TryGetComponent<DamageComponent>(out var d))
                {
                    d.damage = GetDamageByType(d.damageType);
                }
            }
        }
    }

    private float GetDamageByType(DamageType type)
    {
        switch (type)
        {
            case DamageType.Bullet: return config.boss.bullet.damage;
            case DamageType.Hand:   return config.boss.hand.damage;
            case DamageType.Wave:   return config.boss.wave.damage;
            default:                return 0f;
        }
    }
}
