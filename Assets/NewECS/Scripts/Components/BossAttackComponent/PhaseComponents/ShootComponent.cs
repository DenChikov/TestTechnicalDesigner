using System.Collections.Generic;
using UnityEngine;

public class ShootComponent : MonoBehaviour
{
    [Header("Bullet")]
    [HideInInspector] public float time;
    [HideInInspector] public int valueShoot;
    [HideInInspector] public List<GameObject> bulletPool = new List<GameObject>();

    public GameObject bulletPrefab;
    [Tooltip("Префабы пуль; для пула можно задать один или несколько типов.")]
    public GameObject[] bulletPrefabs;
    public int poolSize = 5;
    public float timeBetweenShoot;
    public int valueShootMax;
    public Transform bulletSpawn;


}
