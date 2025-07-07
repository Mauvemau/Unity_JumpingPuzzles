using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A platform that enables or/and disables objects on collision
/// </summary>
public class GameObjectTogglerPlatform : CollisionInteractable {
    [Header("References")]
    [SerializeField] private List<GameObject> objectsToEnable;
    [SerializeField] private List<GameObject> objectsToDisable;

    protected override void HandleCollision(GameObject other) {
        if (objectsToDisable.Count > 0) {
            foreach (var obj in objectsToDisable) {
                if (!obj) {
                    Debug.LogWarning($"{name}: Trying to disable null object!");
                    continue;
                }
                obj.SetActive(false);
            }
        }

        if (objectsToEnable.Count <= 0) return;
        foreach (var obj in objectsToEnable) {
            if (!obj) {
                Debug.LogWarning($"{name}: Trying to enable null object!");
                continue;
            }
            obj.SetActive(true);
        }
    }

    protected override void HandleTrigger(GameObject other) { }
}
