using System.Collections.Generic;
using UnityEngine;

public class GameObjectTogglerPlatform : MonoBehaviour {
    [Header("References")]
    [SerializeField] 
    private List<GameObject> objectsToEnable;
    [SerializeField]
    private List<GameObject> objectsToDisable;
    
    [Header("Config")]
    [SerializeField] 
    private LayerMask playerLayer;
    
    private void HandlePlayerCollision() {
        if (objectsToDisable.Count > 0) {
            foreach (var obj in objectsToDisable) {
                obj.SetActive(false);
            }
        }

        if (objectsToEnable.Count <= 0) return; // Damn Rider is really obsessed with inverted ifs
        foreach (var obj in objectsToEnable) {
            obj.SetActive(true);
        }
    }
    
    private void OnCollisionEnter(Collision collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & playerLayer) == 0) return;
        HandlePlayerCollision();
    }

    private void OnValidate() {
        if (playerLayer != 0) return;
        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1) {
            playerLayer = 1 << playerLayerIndex;
        }
    }
}
