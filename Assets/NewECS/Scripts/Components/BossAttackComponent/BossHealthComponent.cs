using UnityEngine;
using UnityEngine.UI;

public class BossHealthComponent : MonoBehaviour, IHealthBoss
{
    public float maxHealth;
    private float bossHp;
    [HideInInspector]public float health { get => bossHp; set => bossHp = value; }

    public Image hpImage;


}
