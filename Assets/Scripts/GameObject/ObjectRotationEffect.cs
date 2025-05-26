using System;
using UnityEngine;

public class ObjectRotationEffect : MonoBehaviour {
    [SerializeField] 
    private Vector3 rotationSpeed = Vector3.zero;
    
    private void Update() {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
