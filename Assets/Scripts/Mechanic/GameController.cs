using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject worldMap;

    public void HideWorldMap() {
        worldMap.SetActive(false);
    }

    public void ShowWorldMap() {
        worldMap.SetActive(true);
    }
}
