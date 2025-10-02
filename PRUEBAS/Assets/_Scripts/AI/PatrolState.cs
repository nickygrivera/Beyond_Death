

using UnityEngine;

public class PatrolState : BaseState
{
    public override void Construct()
    {
        StateMotor.aiBehaviour.SetAgentPatrolSpeed();
        StateMotor.aiBehaviour.GoToNextWaypoint();
    }

    public override void Transition()
    {
        if(StateMotor.playerOnSight)
            StateMotor.ChangeState(GetComponent<ChaseState>());
        if (StateMotor.aiBehaviour.CheckNextPoint())
        {
            StateMotor.ChangeState(GetComponent<IdleState>());
        }
    }

    public override void UpdateState()
    {
        if(!StateMotor.aiBehaviour.CheckNextPoint()) return;
        StateMotor.aiBehaviour.GoToNextWaypoint();
    }

    public override void Destruct()
    {
        Debug.Log("Destruct patrol state");
    }
}
