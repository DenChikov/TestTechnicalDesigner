using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public DamageType damageType;
    public float damage;
}

public enum DamageType
{
    Bullet,
    Hand,
    Wave
}
