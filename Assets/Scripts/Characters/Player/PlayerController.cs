using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private float _inputHorizontal = 0f;
    private float _inputVertical = -1f;
    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update() {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        if (_movementInput != Vector2.zero) {
            bool success = TryMove(_movementInput);
            if (!success) {
                success = TryMove(new Vector2(_movementInput.x, 0));
                if (!success) {
                    TryMove(new Vector2(0, _movementInput.y));
                }
            }
            
            _animator.SetBool("isMoving", success);
        }
        else {
            _animator.SetBool("isMoving", false);
        }
        
        if (_inputHorizontal != 0 || _inputVertical != 0) {
            _animator.SetFloat("xFacing", _inputHorizontal);
            _animator.SetFloat("yFacing", _inputVertical);
        }
    }

    void OnMove(InputValue movementValue) {
        _movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        _animator.SetTrigger("pick");
    }

    private bool TryMove(Vector2 direction) {
        int count = _rigidbody.Cast(
            direction,
            movementFilter,
            _castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset
        );

        if (count == 0) {
            float moveTimeOffset = moveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(direction * moveTimeOffset + _rigidbody.position);
            return true;
        }

        return false;
    }
}
