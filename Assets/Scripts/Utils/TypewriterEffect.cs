using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour {
    [SerializeField] public float typewriterSpeed = 50f;
    
    public Coroutine DisplayDialogue(string text, TMP_Text textField) {
        return StartCoroutine(TypeText(text, textField));
    }
    
    private IEnumerator TypeText(string textToType, TMP_Text textField) {
        float t = 0;
        int charIndex = 0;
        
        while(charIndex < textToType.Length) {
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);

            textField.text = textToType.Substring(0, charIndex);
            yield return null;
        }
    }
}
