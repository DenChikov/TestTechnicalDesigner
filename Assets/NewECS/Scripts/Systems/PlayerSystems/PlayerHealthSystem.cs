using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour, ISystem
{
    // Система хранит массив всех сущностей, которые она должна обрабатывать
    private PlayerHealthDataComponent[] healthComponents;

    public void Init()
    {
        // Находим все нужные компоненты (для простоты так, в будущем можно передавать их через менеджер/регистр)
        healthComponents = FindObjectsByType<PlayerHealthDataComponent>(FindObjectsSortMode.None);
        
        // Система инициализирует данные
        foreach (var data in healthComponents)
        {
            data.health = data.healthMax;
            UpdateUI(data);
        }
    }

    public void Execute()
    {
        // Итерируемся по всем сущностям, у которых есть компонент здоровья
        foreach (var data in healthComponents)
        {
            // Если есть нерешенный урон
            if (data.pendingDamage > 0)
            {
                data.health -= data.pendingDamage;
                
                // ВАЖНО: Система "потребила" урон и очищает буфер
                data.pendingDamage = 0; 

                UpdateUI(data); // Обновляем полоску только когда здоровье реально изменилось

                if (data.health <= 0)
                {
                    Die();
                }
            }
        }
    }

    private void UpdateUI(PlayerHealthDataComponent data)
    {
        if (data.hpImage != null)
        {
            data.hpImage.fillAmount = data.health / data.healthMax;
        }
    }

    private void Die()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}