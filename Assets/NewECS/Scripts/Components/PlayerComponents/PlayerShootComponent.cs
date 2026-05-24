using UnityEngine;
using UnityEngine.UI;

public class PlayerShootComponent : MonoBehaviour
{
    public float damage = 25f;
    public LayerMask enemyDamageLayer;
    public float timeBetweenShoot;
    public Image markerHit;
    public Image patrons;
    public float timer;
    public Animator animator;
    public ParticleSystem particle;
}
