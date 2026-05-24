using UnityEngine;
using UnityEngine.UI;

// Компонент - только данные.
public class PlayerHealthDataComponent : MonoBehaviour
{
    public float healthMax = 100f;
    public LayerMask damageSourceLayer;
    public Image hpImage;
    
    [HideInInspector] public float health;
    
    // Временные данные (буфер): сколько урона нужно нанести в текущем кадре
    [HideInInspector] public float pendingDamage; 

    // Транслируем физическое событие Unity в данные для нашей ECS
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer.value) != 0)
        {
            if (other.TryGetComponent<DamageComponent>(out var damageComponent))
            {
                // Накапливаем урон (на случай если в один кадр попало несколько снарядов)
                pendingDamage += damageComponent.damage;
            }
        }
    }
}
