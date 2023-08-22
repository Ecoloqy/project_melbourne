using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveDialogueObject : MonoBehaviour, ITriggerInteraction  {
    [SerializeField] private DialogueScriptableObject _dialogueObject;
    [SerializeField] private List<Direction> _collideDirection = new List<Direction>();

    private DialogueUI _dialogueUI;

    public void Start() {
        _dialogueUI = FindObjectOfType<DialogueUI>();
    }

    public void TriggerInteraction(GameObject eGameObject, Direction direction) {
        if (_collideDirection.Contains(direction) && !_dialogueUI.IsDisplayingDialogue()) {
            _dialogueUI.DisplayDialogue(_dialogueObject);
        }
    }
}
