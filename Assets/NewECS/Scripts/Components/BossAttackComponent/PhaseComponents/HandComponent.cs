using UnityEngine;

public class HandComponent : MonoBehaviour
{
    [Header("HandAttack")]
    public float windUpTime;
    [Tooltip("Когда у врага пропадают точки получения урона")]
    public float damageZoneTime;
    [Min(0.01f)] public float punchTime;
    [Min(0.01f)] public float returnTime;
    public GameObject goMove;
    public GameObject effectHit;
    public GameObject damageEnemy;
    [HideInInspector] public bool flagHand;
}

