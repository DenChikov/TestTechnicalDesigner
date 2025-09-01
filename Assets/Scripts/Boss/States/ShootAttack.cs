using UnityEngine;

public class ShootAttack : IState
{
    private Enemy enemy;
    public ShootAttack(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        Debug.Log("ShootAttackEnter");
    }

    public void Exit()
    {
    }

    public void Update()
    {
        enemy.Shoot();
        Debug.Log("ShootAttackUpdate");
    }
}
