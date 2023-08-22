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
    private DialogueUI _dialogueUI;

    private bool _isMovable = true;

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _dialogueUI = FindObjectOfType<DialogueUI>();
    }

    void Update() {
        if (!_dialogueUI.IsDisplayingDialogue()) {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");

            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                new Vector2(_animator.GetFloat("xFacing"), _animator.GetFloat("yFacing")), _raycastLength,
                interactionLayer);
            Debug.DrawLine(transform.position,
                new Vector2(transform.position.x + inputHorizontal, transform.position.y + inputVertical), Color.red,
                1f);
            if (hit.collider != null) {
                ITouchInteraction touchInteractionObject = hit.collider.GetComponent<ITouchInteraction>();
                if (touchInteractionObject != null) {
                    touchInteractionObject.TouchInteraction(gameObject, GetPlayerStandingDirection());
                }
            }
        }
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
        if (_isMovable && !_dialogueUI.IsDisplayingDialogue()) {
            _movementInput = movementValue.Get<Vector2>();
        }
    }

    void OnFire() {
        if (!_dialogueUI.IsDisplayingDialogue()) {
            _movementInput = new Vector2(0, 0);
        
            _isMovable = false;
            _animator.SetTrigger("pick");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(_animator.GetFloat("xFacing"), _animator.GetFloat("yFacing")), _raycastLength, interactionLayer);

            if (hit.collider != null)
            {
                ITriggerInteraction triggerInteractionObject = hit.collider.GetComponent<ITriggerInteraction>();
                if (triggerInteractionObject != null) {
                    triggerInteractionObject.TriggerInteraction(gameObject, GetPlayerStandingDirection());
                }
            }

            StartCoroutine(UnlockPlayer());
        }
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

    private Direction GetPlayerStandingDirection() {
        var facingXPos = _animator.GetFloat("xFacing");
        var facingYPos = _animator.GetFloat("yFacing");
        if (facingXPos > 0 && facingYPos == 0) {
            return Direction.EAST;
        }
        if (facingXPos < 0 && facingYPos == 0) {
            return Direction.WEST;
        }
        if (facingXPos == 0 && facingYPos < 0) {
            return Direction.SOUTH;
        }
        return Direction.NORTH;
    }

    private IEnumerator UnlockPlayer() {
        yield return new WaitForSeconds(0.5f);
        _isMovable = true;
    }
}
