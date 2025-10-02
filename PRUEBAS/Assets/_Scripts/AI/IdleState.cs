using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    [SerializeField] private float idleCooldown;
    
    private Coroutine _coroutine;

    public override void Construct()
    {
        StateMotor.aiBehaviour.ResetAgentPath();
        StateMotor.idleIsDone = false;
        _coroutine = StartCoroutine(IdleCooldown());
    }

    public override void Transition()
    {
        if(StateMotor.playerOnSight)
            StateMotor.ChangeState(GetComponent<ChaseState>());
        if(StateMotor.idleIsDone)
            StateMotor.ChangeState(GetComponent<PatrolState>());
    }

    public override void Destruct()
    {
        StateMotor.idleIsDone = true;
        StopCoroutine(_coroutine);
    }

    private IEnumerator IdleCooldown()
    {
        yield return new WaitForSeconds(idleCooldown);
        StateMotor.idleIsDone = true;
    }
}
