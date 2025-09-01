using UnityEngine;

public class Idle : IState
{
    private Enemy enemy;

    public Idle(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        Debug.Log("IdleEnter");
    }

    public void Exit()
    {
       
    }

    public void Update()
    {
        Debug.Log("IdleUpdate");
    }
}
