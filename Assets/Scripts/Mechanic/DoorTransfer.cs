using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransfer : MonoBehaviour, ITriggerInteraction {
    public string sceneName;
    public Vector2 enterCords;
    
    private Animator _animator;
    private bool _isOpenedState;

    void Start() {
        _animator = GetComponent<Animator>();
    }

    public void TriggerInteraction(GameObject eGameObject) {
        StartCoroutine(Interact(gameObject));
    }

    private IEnumerator Interact(GameObject eGameObject) {
        Debug.Log("Hello Props");
        
        if (_isOpenedState) {
            _animator.SetTrigger("close");
        } else {
            _animator.SetTrigger("open");
        }
        _isOpenedState = !_isOpenedState;

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        SceneManager.GetSceneByName("Village");
        // eGameObject.transform.position = new Vector3(enterCords.x, enterCords.y, 0f);

        if (sceneName != null) {
            yield return new WaitForSeconds(1f);
            _animator.Play("door_closed_state");
        }
    }
}
