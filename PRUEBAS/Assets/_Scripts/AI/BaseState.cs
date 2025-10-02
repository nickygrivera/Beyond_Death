using UnityEngine;

[RequireComponent(typeof(AIStateMotor))]
public abstract class BaseState : MonoBehaviour
{
    protected AIStateMotor StateMotor;

    private void Awake()
    {
        StateMotor = GetComponent<AIStateMotor>();
    }

    public virtual void Construct() {}

    public virtual void Destruct() {}

    public virtual void UpdateState() {}
    
    public virtual void Transition() {}
}
