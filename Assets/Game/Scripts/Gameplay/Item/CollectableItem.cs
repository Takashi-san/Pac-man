using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] CollectableItemType _itemType = null;
    
    void Start() {
        _itemType.Setup(gameObject);
    }
    
    void GotItem() {
        _itemType.ItemEffect(gameObject);
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D p_other) {
        PacMan pacman = p_other.GetComponent<PacMan>();
        if (pacman != null) {
            GotItem();
        }
    }
}
