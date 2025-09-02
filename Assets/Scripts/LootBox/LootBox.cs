using System.IO;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [SerializeField] private float baseChance = 15f;     // базовый шанс
    [SerializeField] private float increment = 5f;    // прирост за неудачу
    [SerializeField] private float maxChance = 50f;     // верхний предел шанса
    [SerializeField] private GameObject rareDrop;
    [SerializeField] private GameObject trashDrop;
    private Transform textTake; // подсказка для открытия сундука
    private GameObject player;
    private string filePath;
    public float currentChance;

    private int streakFail;
    private int streakSuccess;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        textTake = gameObject.transform.GetChild(0);
        filePath = Path.Combine(Application.persistentDataPath, "data.txt");

        currentChance = float.Parse(LoadData());
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && 
            Vector3.Distance(player.transform.position, gameObject.transform.position) < 2)
        {
            TryDrop();
        }
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 2)
        {
            textTake.gameObject.SetActive(true);
        }
        else { textTake.gameObject.SetActive(false); }
    }
    public void TryDrop()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= currentChance)
        {
            Debug.Log("Редкий предмет получен!");
            currentChance = baseChance; // сброс
            GameObject loot = Instantiate(rareDrop);
            loot.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            streakSuccess += 1;
            if(streakSuccess >= 1)
            {
                streakSuccess = 0;
                currentChance = 10;
            }
        }
        else
        {
            Debug.Log("Не повезло...");
            currentChance = Mathf.Min(currentChance + increment, maxChance);
            GameObject loot = Instantiate(trashDrop);
            loot.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            streakFail += 1;
            if(streakFail >= 8)
            {
                streakFail = 0;
                currentChance = 80;
            }
        }

        Debug.Log($"Текущий шанс на следующий дроп: {currentChance}%");
    }
    private void OnDisable()
    {
        SaveData(currentChance.ToString());
    }
    public void SaveData(string chance)
    {
        File.WriteAllText(filePath, chance);
        Debug.Log($"Данные записаны в {filePath}");
    }
    public string LoadData()
    {
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            Debug.Log("Данные загружены: " + data);
            return data;
        }
        else
        {
            Debug.LogWarning("Файл не найден");
            string data;
            data = baseChance.ToString();
            return data;
        }

    }
}
