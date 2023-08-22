using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSimpleController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveTime = 1f;
    public float moveBreak = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public LayerMask interactionLayer;
    public List<Vector2> moveCoordinates = new List<Vector2>();

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    private float _raycastLength = 0.1f;
    private int _moveIndex = 0;
    private bool _moveAvailable = true;
    private bool _isMovable = true;

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            new Vector2(_animator.GetFloat("xFacing"), _animator.GetFloat("yFacing")), _raycastLength,
            interactionLayer);
        if (hit.collider != null) {
            ITouchInteraction touchInteractionObject = hit.collider.GetComponent<ITouchInteraction>();
            if (touchInteractionObject != null) {
                touchInteractionObject.TouchInteraction(gameObject, GetPlayerStandingDirection());
            }
        }
    }

    void FixedUpdate() {
        if (_moveIndex + 1 <= moveCoordinates.Count && _moveAvailable) {
            StartCoroutine(MoveCharacter(_moveIndex));
        }
        
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

    private IEnumerator MoveCharacter(int moveIndex) {
        if (_isMovable) {
            _moveAvailable = false;
            _movementInput = moveCoordinates[moveIndex];
            if (_movementInput.x != 0 || _movementInput.y != 0) {
                _animator.SetFloat("xFacing", _movementInput.x);
                _animator.SetFloat("yFacing", _movementInput.y);
            }
            yield return new WaitForSeconds(moveTime);
            
            _moveIndex++;
            if (_moveIndex == moveCoordinates.Count) {
                _moveIndex = 0;
            }

            _movementInput = Vector2.zero;
            yield return new WaitForSeconds(moveBreak);
            _moveAvailable = true;
        }
    }
}
