using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : BaseHealth
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private NavMeshAgent _agent;
    private Material _material;

    protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    protected override void Die()
    {
        _agent.enabled = false;
        _material.color = Color.red;
        Destroy(gameObject, 3f);
    }
}
