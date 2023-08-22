using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour {
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private float _waitBeforeWrite = 0.3f;

    private TypewriterEffect _typewriterEffect;
    private bool _isDisplaying;

    public void Start() {
        _typewriterEffect = GetComponent<TypewriterEffect>();
    }

    public bool IsDisplayingDialogue() {
        return _isDisplaying;
    }
    
    public void DisplayDialogue(DialogueScriptableObject dialogueObject) {
        Debug.Log(dialogueObject);
        if (dialogueObject != null) {
            _isDisplaying = true;
            transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(StepThroughDialogue(dialogueObject));
        }
    }

    public void OnFire() {
        Debug.Log("Fire");
    }

    private IEnumerator StepThroughDialogue(DialogueScriptableObject dialogueObject) {
        yield return new WaitForSeconds(_waitBeforeWrite);

        foreach (var dialogue in dialogueObject.Dialogue) {
            yield return _typewriterEffect.DisplayDialogue(dialogue, _textField);
            yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));
        }

        _isDisplaying = false;
        _textField.text = String.Empty;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
