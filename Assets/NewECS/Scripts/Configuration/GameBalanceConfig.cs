using System;
using UnityEngine;

/// <summary>
/// Единый источник правды по геймплейному балансу.
/// Создаётся через Assets → Create → NewECS → Game Balance Config.
/// Раскидывается по компонентам в ConfigurationSystem на старте сцены.
/// </summary>
[CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "NewECS/Game Balance Config", order = 0)]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Player")]
    public PlayerSection player = new PlayerSection();

    [Header("Boss")]
    public BossSection boss = new BossSection();

    // ---------------- Player ----------------

    [Serializable]
    public class PlayerSection
    {
        [Header("Health")]
        public PlayerHealthSection health = new PlayerHealthSection();

        [Header("Shoot")]
        public PlayerShootSection shoot = new PlayerShootSection();

        [Serializable]
        public class PlayerHealthSection
        {
            [Min(1f), Tooltip("Максимум HP игрока на старте.")]
            public float healthMax = 100f;
        }

        [Serializable]
        public class PlayerShootSection
        {
            [Min(0f), Tooltip("Урон одного выстрела игрока по боссу.")]
            public float damage = 25f;

            [Min(0.01f), Tooltip("Кулдаун между выстрелами, сек.")]
            public float timeBetweenShoot = 0.5f;
        }
    }

    // ---------------- Boss ----------------

    [Serializable]
    public class BossSection
    {
        [Header("Health")]
        public BossHealthSection health = new BossHealthSection();

        [Header("Combat")]
        public BossCombatSection combat = new BossCombatSection();

        [Header("Hand attack")]
        public BossHandSection hand = new BossHandSection();

        [Header("Wave attack")]
        public BossWaveSection wave = new BossWaveSection();

        [Header("Shoot")]
        public BossShootSection shoot = new BossShootSection();

        [Header("Bullet")]
        public BossBulletSection bullet = new BossBulletSection();

        [Serializable]
        public class BossHealthSection
        {
            [Min(1f)] public float maxHealth = 1000f;
        }

        [Serializable]
        public class BossCombatSection
        {
            [Min(0f), Tooltip("Радиус сферы аггра босса. При входе игрока в радиус начинается бой.")]
            public float aggroRadius = 12f;
        }

        [Serializable]
        public class BossHandSection
        {
            [Min(0f), Tooltip("Время замаха перед ударом рукой.")]
            public float windUpTime = 1f;

            [Min(0f), Tooltip("Сколько секунд активна зона урона удара руки.")]
            public float damageZoneTime = 0.3f;

            [Min(0.01f), Tooltip("Длительность фазы удара (опускание руки).")]
            public float punchTime = 0.3f;

            [Min(0.01f), Tooltip("Длительность возврата руки в исходное положение.")]
            public float returnTime = 0.5f;

            [Min(0f), Tooltip("Урон удара рукой (DamageType.Hand).")]
            public float damage = 30f;
        }

        [Serializable]
        public class BossWaveSection
        {
            [Tooltip("Прирост размера визуала волны за кадр.")]
            public Vector3 scaleAdd = new Vector3(0.1f, 0f, 0.1f);

            [Min(0.01f), Tooltip("Сколько секунд живёт волна после удара.")]
            public float timeLife = 1f;

            [Min(0f), Tooltip("Стартовый внешний радиус кольца урона.")]
            public float outsideRadius = 3f;

            [Min(0f), Tooltip("Стартовый внутренний радиус кольца урона.")]
            public float innerRadius = 1f;

            [Min(0f), Tooltip("Урон волны (DamageType.Wave).")]
            public float damage = 20f;
        }

        [Serializable]
        public class BossShootSection
        {
            [Min(1), Tooltip("Размер пула пуль босса.")]
            public int poolSize = 5;

            [Min(0.01f), Tooltip("Кулдаун между выстрелами босса, сек.")]
            public float timeBetweenShoot = 0.5f;

            [Min(1), Tooltip("Сколько выстрелов сделать до перехода в фазу Hand.")]
            public int valueShootMax = 3;
        }

        [Serializable]
        public class BossBulletSection
        {
            [Min(0f)] public float bulletSpeed = 9f;
            [Min(0f)] public float bulletRotationSpeed = 200f;
            [Min(0.01f)] public float bulletLifeTime = 10f;

            [Min(0f), Tooltip("Урон пули босса (DamageType.Bullet).")]
            public float damage = 10f;
        }
    }
}
