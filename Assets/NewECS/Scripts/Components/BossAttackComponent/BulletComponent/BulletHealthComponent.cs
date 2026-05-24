using UnityEngine;

public class BulletHealthComponent : MonoBehaviour, IHealthBoss
{
    private float bulletHp = 1.0f;

    [HideInInspector]public float health { get => bulletHp; set => bulletHp = value; }
}
