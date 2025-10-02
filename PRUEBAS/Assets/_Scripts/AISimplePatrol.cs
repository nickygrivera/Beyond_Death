using System;
using UnityEngine;
using UnityEngine.AI;

public class AISimplePatrol : MonoBehaviour
{
    [SerializeField] private Transform[] wayPoints;
    
    private NavMeshAgent _agent;
    private int _currentWayPointIndex;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        GoToNextPoint();
    }

    private void Update()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
        {
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        if(_currentWayPointIndex >= wayPoints.Length) _currentWayPointIndex = 0;
        
        _agent.SetDestination(wayPoints[_currentWayPointIndex++].position);
    }
}
