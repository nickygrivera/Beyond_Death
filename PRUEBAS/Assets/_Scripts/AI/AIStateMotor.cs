using System;
using Unity.VisualScripting;
using UnityEngine;

public class AIStateMotor : MonoBehaviour
{
    public Animator anim;
    public Transform target;
    public AIBehaviour aiBehaviour;

    public bool idleIsDone, playerOnSight;

    private BaseState _currentState;

    private void Awake()
    {
        aiBehaviour = GetComponent<AIBehaviour>();
        _currentState = GetComponent<PatrolState>();
    }

    private void Start()
    {
        _currentState.Construct();
    }

    private void Update()
    {
        _currentState.Transition();
        _currentState.UpdateState();
    }

    public void ChangeState(BaseState newState)
    {
        _currentState.Destruct();
        _currentState = newState;
        _currentState.Construct();
    }

    private void OnTriggerEnter(Collider other)
    {
        target = other.GetComponent<ITargeteable>()?.GetTransform(out playerOnSight);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<ITargeteable>() != null)
            playerOnSight = false;
    }
}
