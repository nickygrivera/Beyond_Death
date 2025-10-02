using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public override void Construct()
    {
        StateMotor.aiBehaviour.SetAgentTarget(StateMotor.target);
        StateMotor.aiBehaviour.SetAgentChaseSpeed();
    }

    public override void Transition()
    {
        if (StateMotor.playerOnSight) return;
        
        StateMotor.ChangeState(GetComponent<PatrolState>());
    }

    public override void UpdateState()
    {
        StateMotor.aiBehaviour.SetMovingDestination();
    }
}
