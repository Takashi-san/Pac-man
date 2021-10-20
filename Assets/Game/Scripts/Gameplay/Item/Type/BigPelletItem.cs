using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Item/BigPellet")]
public class BigPelletItem : PelletItem
{
    public override void ItemEffect(GameObject p_owner) {
        base.ItemEffect(p_owner);
        GameplayManager.Instance.GotBigPellet();
    }
}
