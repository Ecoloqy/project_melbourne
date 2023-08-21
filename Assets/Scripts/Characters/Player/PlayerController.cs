using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public float inputHorizontal = 0f;
    public float inputVertical = -1f;
    public ContactFilter2D movementFilter;
    public LayerMask interactionLayer;

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    private float _raycastLength = 0.1f;

    private bool _isMovable = true;

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
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
        
        if (inputHorizontal != 0 || inputVertical != 0) {
            _animator.SetFloat("xFacing", inputHorizontal);
            _animator.SetFloat("yFacing", inputVertical);
        }
    }

    void OnMove(InputValue movementValue) {
        if (_isMovable) {
            _movementInput = movementValue.Get<Vector2>();
        }
    }

    void OnFire() {
        _movementInput = new Vector2(0, 0);
        
        _isMovable = false;
        _animator.SetTrigger("pick");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(_animator.GetFloat("xFacing"), _animator.GetFloat("yFacing")), _raycastLength, interactionLayer);
        Debug.DrawRay(transform.position, new Vector2(GetRaycastLength(_animator.GetFloat("xFacing")), GetRaycastLength(_animator.GetFloat("yFacing"))), Color.red, 10f);
        Debug.Log(hit.collider);
        
        if (hit.collider != null)
        {
            ITriggerInteraction triggerInteractionObject = hit.collider.GetComponent<ITriggerInteraction>();
            Debug.Log(triggerInteractionObject);
            if (triggerInteractionObject != null) {
                triggerInteractionObject.TriggerInteraction(gameObject);
            }
        }

        StartCoroutine(UnlockPlayer());
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

    private float GetRaycastLength(float value) {
        return value * _raycastLength;
    }

    private IEnumerator UnlockPlayer() {
        yield return new WaitForSeconds(0.5f);
        _isMovable = true;
    }
}
