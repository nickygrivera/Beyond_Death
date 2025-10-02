using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    
    [SerializeField] private float patrolSpeed, 
        chaseSpeed, 
        breakingDistance, 
        detectionRange;
    
    [SerializeField] private CapsuleCollider detectionRangeCollider;
    
    private Transform _target;
    private NavMeshAgent _agent;
    
    private int _currentWaypointIndex;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        
        detectionRangeCollider.center = new Vector3(
            detectionRangeCollider.center.x, 
            detectionRangeCollider.center.y, 
            detectionRange/ 2);
        detectionRangeCollider.height = detectionRange;
        InitializeAgent();
    }

    private void InitializeAgent()
    {
        SetAgentPatrolSpeed();
        SetStoppingDistance(breakingDistance);
    }

    public void ResetAgentPath()
    {
        _agent.ResetPath();
    }

    public void SetStoppingDistance(float stoppingDistance)
    {
        _agent.stoppingDistance = stoppingDistance;
    }

    public void SetAgentPatrolSpeed()
    {
        SetAgentSpeed(patrolSpeed);
    }

    public void SetAgentChaseSpeed()
    {
        SetAgentSpeed(chaseSpeed);
    }

    private void SetAgentSpeed(float speed)
    {
        _agent.speed = speed;
    }

    public void SetAgentTarget(Transform target)
    {
        _target = target;
    }

    public void SetNextPatrolPoint()
    {
        if(waypoints.Length == 0) return;

        if (_currentWaypointIndex >= waypoints.Length)
            _currentWaypointIndex = 0;
        
        SetAgentTarget(waypoints[_currentWaypointIndex++]);
    }

    public void SetMovingDestination()
    {
        if(!_target) return;
        _agent.SetDestination(_target.position);
    }

    public void GoToNextWaypoint()
    {
        SetNextPatrolPoint();
        _agent.SetDestination(_target.position);
    }

    public bool CheckNextPoint()
    {
        return !_agent.pathPending && _agent.remainingDistance <= breakingDistance;
    }
}
