using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour, IHealth
{
    [SerializeField] private float hp;
    [SerializeField] private Image hpImage;
    [SerializeField] private GameObject spawnLootBox;
    [SerializeField] private GameObject restartFight;
    [SerializeField] private GameObject[] damageZone;
    private int damageZoneCount;
    private float maxHp;

    private void Awake()
    {
        damageZoneCount = damageZone.Length - 1;
        maxHp = hp;
    }
    public void TakeDamage(float damage)
    {
        hp -= damage;
        damageZone[damageZoneCount].SetActive(false);
        damageZoneCount -= 1;
        hpImage.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            GameObject loot = Instantiate(spawnLootBox);
            loot.transform.position = gameObject.transform.position;
            restartFight.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
