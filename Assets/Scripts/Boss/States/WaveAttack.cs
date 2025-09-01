using UnityEngine;

public class WaveAttack : IState
{
    private Enemy enemy;
    public WaveAttack(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        Debug.Log("WaveAttackEnter");
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
