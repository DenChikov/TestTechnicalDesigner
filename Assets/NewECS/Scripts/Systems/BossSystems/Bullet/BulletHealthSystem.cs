using UnityEngine;

public class BulletHealthSystem : MonoBehaviour, ISystem
{
    private BulletHealthComponent[] bulletHealthComponents;
    public void Init()
    {
        bulletHealthComponents = FindObjectsByType<BulletHealthComponent>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
    }
    public void Execute()
    {
        foreach (var bulletHealthComponent in bulletHealthComponents)
        {
            if (bulletHealthComponent.health <= 0)
            {
                bulletHealthComponent.gameObject.SetActive(false);
                bulletHealthComponent.health = 1;
            }
        }
    }

}
