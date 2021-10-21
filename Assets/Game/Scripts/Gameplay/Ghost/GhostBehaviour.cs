using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName="Ghost Behaviour/Default")]
public abstract class GhostBehaviour : ScriptableObject
{
    public Character Ghost;
    public float Speed;
    public float RespawnTime;

    public abstract Vector3Int GetChasePosition(GameObject p_owner);
}
