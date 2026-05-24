using UnityEngine;

public class MainSystem : MonoBehaviour
{
    [SerializeField] private ISystem[] systems;

    private void Awake()
    {
        systems = GetComponents<ISystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Init();
        }

        var configuration = GetComponent<ConfigurationSystem>();
        if (configuration != null)
            configuration.ApplyRuntimeFromConfig();
    }

    private void Update()
    {
        //debugging
        BossCombatDataComponent data = FindAnyObjectByType<BossCombatDataComponent>();
        Debug.Log(data.phaseData.Value);
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Execute();
        }
    }
}
