using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private float hp;
    [SerializeField] private Image hpImage;
    private float maxHp;
    private void Awake()
    {
        maxHp = hp;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
    public void TakeDamage(float damage)
    {
        hp -= damage;
        hpImage.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
