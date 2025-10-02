
using UnityEngine;

public interface ITargeteable
{
    public Transform GetTransform(out bool playerOnSight);
}
