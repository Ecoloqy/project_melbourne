using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    
    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody;
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate() {
        if (_movementInput != Vector2.zero) {
            int count = _rigidbody.Cast(
                _movementInput,
                movementFilter,
                _castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
            );

            if (count == 0) {
                float moveTimeOffset = moveSpeed * Time.fixedDeltaTime;
                _rigidbody.MovePosition(_movementInput * moveTimeOffset + _rigidbody.position);
            }
        }
    }
    
    void OnMove(InputValue movementValue) {
        _movementInput = movementValue.Get<Vector2>();
    }
}
