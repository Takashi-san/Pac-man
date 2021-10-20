using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableItemType : ScriptableObject
{
    public virtual void Setup(GameObject p_owner) {}
    public abstract void ItemEffect(GameObject p_owner);
}
