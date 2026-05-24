using UnityEngine;

public class WaveComponent : MonoBehaviour
{
    [Header("WaveAttack")]
    public GameObject waveGo;
    [HideInInspector] public Vector3 waveGoPos;
    public Vector3 scaleAdd;
    public float timeLife;
    public float time;
    [HideInInspector] public Vector3 oldScale;
    [HideInInspector] public bool waveActivate;
    [Header("RadiusWave")]
    [Tooltip("Начальный внешний радиус кольца")]
    public float outsideRadius;
    [Tooltip("Начальный внутренний радиус кольца (толщина = outside - inner)")]
    public float innerRadius;

    // Текущие радиусы кольца (растут вместе с визуалом)
    public float currentInnerRadius;
    public float currentOutsideRadius;
}
