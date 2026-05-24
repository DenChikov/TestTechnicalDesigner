using System.Collections.Generic;
using UnityEngine;

public class BossCombatDataComponent : MonoBehaviour
{
    public GameObject player;

    [Header("Aggro")]
    [Tooltip("Сфера от позиции босса: если коллайдер игрока попадает внутрь, начинается бой.")]
    public float aggroRadius = 12f;
    [Tooltip("Cлой игрока. CheckSphere учитывает только этот слой.")]
    public LayerMask playerLayerMask;
    
    [Header("Current State")]
    public PhaseEnum phaseData = new PhaseEnum { Value = PhaseEnum.Phase.Idle };
}
