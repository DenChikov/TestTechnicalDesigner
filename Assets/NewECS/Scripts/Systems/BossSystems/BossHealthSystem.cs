using UnityEngine;

public class BossHealthSystem : MonoBehaviour, ISystem
{

    private BossHealthComponent bossHealthComponent;
    public void Init()
    {
        bossHealthComponent = FindAnyObjectByType<BossHealthComponent>(FindObjectsInactive.Include);
        bossHealthComponent.health = bossHealthComponent.maxHealth;
    }
    public void Execute()
    {
        CheckHp();
    }
    private void CheckHp()
    {
        bossHealthComponent.hpImage.fillAmount = bossHealthComponent.health / bossHealthComponent.maxHealth;
        if (bossHealthComponent.health <= 0)
        {
            bossHealthComponent.gameObject.SetActive(false);
        }
    }
}
