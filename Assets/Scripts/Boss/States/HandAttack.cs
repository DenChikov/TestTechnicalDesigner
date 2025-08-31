using UnityEngine;

public class HandAttack : IState
{
    private Enemy enemy;
    private GameObject player;
    public HandAttack(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        enemy.StartHand();
        Debug.Log("HandAttackEnter");
    }

    public void Exit()
    {
    }

    public void Update()
    {
        
        Debug.Log("HandAttackUpdate");
    }
}
