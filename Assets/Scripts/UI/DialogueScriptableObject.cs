using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueScriptableObject : ScriptableObject {
    [SerializeField] [TextArea] private string[] dialogue;

    public string[] Dialogue => dialogue;
}
