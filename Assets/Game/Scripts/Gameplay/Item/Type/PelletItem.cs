using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Item/Pellet")]
public class PelletItem : CollectableItemType
{
    [SerializeField] int _score = 1;
    
    public override void Setup(GameObject p_owner) {
        GameplayManager.Instance.AddPellet(p_owner);
    }

    public override void ItemEffect(GameObject p_owner) {
        GameplayManager.Instance.RemovePellet(p_owner);
        GameplayManager.Instance.AddScore(_score);
    }
}
