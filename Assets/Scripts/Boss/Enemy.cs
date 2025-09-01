using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    private StateMachine stateMachine;
    private Idle idleState;
    private ShootAttack shootState;
    private HandAttack handState;


    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public int poolSize = 5;
    [SerializeField]private float timeBetweenShoot;
    private float time;
    [SerializeField]private int valueShootMax; //количество раз для выстрела
    private int valueShoot;
    [SerializeField]private Transform bulletSpawn;

    private List<GameObject> bulletPool = new List<GameObject>();



    [Header("HandAttack")]
    [SerializeField] private float windUpTime;
    [SerializeField,Tooltip("Когда у врага пропадают точки получения урона")] 
    private float damageZoneTime;

    [SerializeField] private float damageHand;
    [SerializeField, Min(0.01f)] private float punchTime;
    [SerializeField, Min(0.01f)] private float returnTime;
    [SerializeField] private GameObject goMove;
    [SerializeField] private GameObject effectHit;
    [SerializeField] private GameObject damageEnemy;

    [Header("WaveAttack")]
    [SerializeField]private GameObject damageWave;
    [SerializeField] private Vector3 scaleAdd;
    [SerializeField] private float damageWaveAttack;
    [SerializeField] private float innerWaveAttack;

    [SerializeField] private float timeLife;


    private void Awake()
    {
        
    }
    private void Start()
    {
        WaveDamage wave = damageWave.GetComponent<WaveDamage>();
        wave.damage = damageWaveAttack;        
        wave.innerRadius = innerWaveAttack;
        wave.timeLife = timeLife;
        wave.scaleAdd = scaleAdd;

        HandDamage hg = GetComponentInChildren<HandDamage>();
        hg.damage = damageHand;


        player = GameObject.FindGameObjectWithTag("Player");

        CreatePool();

        stateMachine = new StateMachine();
        idleState = new Idle(this);
        shootState = new ShootAttack(this);
        handState = new HandAttack(this);
        stateMachine.ChangeState(idleState);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
        stateMachine.Update();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stateMachine.ChangeState(shootState);
        }  
    }


    //shoot logic

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }
    private GameObject GetPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }
    public void Shoot()
    {
        time += Time.deltaTime;
        if (time > timeBetweenShoot)
        {
            GameObject bullet = GetPool();
            bullet.transform.position = bulletSpawn.transform.position;
            bullet.SetActive(true);
            time = 0;
            valueShoot += 1;
        }
        if(valueShoot >= valueShootMax)
        {
            valueShoot = 0;
            stateMachine.ChangeState(handState);
        }
        transform.LookAt(player.transform, Vector3.up);
    }

    //hand attack logic
    public void StartHand()
    {
        StartCoroutine(Hand(player.transform.position));
    }
    private IEnumerator Hand(Vector3 pos)
    {
        pos.y = 0;
        transform.LookAt(player.transform, Vector3.up);
        float elapsed = 0;
        Vector3 oldPosition = goMove.transform.position;


        Debug.Log(oldPosition);
        effectHit.transform.position = pos;
        effectHit.SetActive(true);


        yield return new WaitForSeconds(windUpTime);

        effectHit.SetActive(false);


        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime / punchTime;
            goMove.transform.position = Vector3.Slerp(oldPosition,
                pos, elapsed);
            yield return null;
        }
        damageWave.transform.position = pos;
        damageWave.SetActive(true);

        damageEnemy.SetActive(true);
        yield return new WaitForSeconds(damageZoneTime);
        damageEnemy.SetActive(false);
        
        elapsed = 0;
        Debug.Log(oldPosition);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime / returnTime;
            goMove.transform.position = Vector3.Slerp(pos,
                oldPosition, elapsed);
            yield return null;
        }
        transform.LookAt(player.transform, Vector3.up);
       
        
       
        stateMachine.ChangeState(shootState);
    }
}
