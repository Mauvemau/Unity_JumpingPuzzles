using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementNode {
    [Tooltip("Position to move the platform towards")]
    public Transform transform;
    [Tooltip("Amount of time the platform should wait on this node")]
    public float haltTime;
}

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour {
    [Header("Config")]
    [SerializeField] private List<MovementNode> movementNodes = new List<MovementNode>();
    [Tooltip("Speed in which the platform will move between the nodes")]
    [SerializeField] private float speed = 5f;
    
    private Rigidbody _rb;
    private int _currentNode;
    private bool _movingForward;
    private float _nextMoveTime;
    private float _remainingTime; // Used for game pause
    private bool _shouldMove;

    private void HandleGamePause(bool paused) {
        _shouldMove = !paused;

        if (paused) {
            _remainingTime = Mathf.Max(0, _nextMoveTime - Time.time);
        } else if (_remainingTime > 0) {
            _nextMoveTime = Time.time + _remainingTime;
        }
    }
    
    private void HandleMovement() {
        if (movementNodes.Count < 2) return;
        var targetNode = movementNodes[_currentNode];
        if (!targetNode.transform) return;
        
        if (Vector3.Distance(transform.position, targetNode.transform.position) < .1f) {
            _rb.MovePosition(targetNode.transform.position);
            _nextMoveTime = Time.time + targetNode.haltTime;
            
            if (_currentNode == movementNodes.Count - 1) {
                _movingForward = false;
            } else if (_currentNode == 0) {
                _movingForward = true;
            }
            
            _currentNode += _movingForward ? 1 : -1;
        }
        
        if (Time.time > _nextMoveTime) {
            _rb.MovePosition(Vector3.MoveTowards(transform.position, targetNode.transform.position, speed * Time.deltaTime));
        }
    }
    
    private void FixedUpdate() {
        if (!_shouldMove) return;
        HandleMovement();
    }
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
    }
    
    private void OnEnable() {
        _currentNode = 0;
        _nextMoveTime = 0;
        _movingForward = true;
        _shouldMove = true;
        GameManager.OnGamePaused += HandleGamePause;
    }

    private void OnDisable() {
        GameManager.OnGamePaused -= HandleGamePause;
    }
    
}
