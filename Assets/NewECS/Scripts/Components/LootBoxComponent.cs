using UnityEngine;

public class LootBoxComponent : MonoBehaviour
{
    [Header("Config — applied by ConfigurationSystem")]
    [Range(0f, 1f)] public float baseChance = 0.2f;
    [Range(0f, 1f)] public float chanceIncrement = 0.06f;
    [Min(1)] public int hardPityThreshold = 8;

    [Header("Runtime state")]
    public float currentChance;
    public float baseChanceNow;
    [Min(1)] public int hardPityThresholdNow;
}
