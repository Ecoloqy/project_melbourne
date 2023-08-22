using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapTransition : MonoBehaviour, ITouchInteraction {
    public string inputSceneName;
    public string outputSceneName;
    public List<Direction> collideDirection = new List<Direction>();
    public bool loadNewScene = false;

    public void TouchInteraction(GameObject eGameObject, Direction direction) {
        StartCoroutine(Interact(gameObject, direction));
    }

    private IEnumerator Interact(GameObject eGameObject, Direction direction) {
        if (collideDirection.Contains(direction)) {
            yield return new WaitForSeconds(0.5f);

            var activeSceneObjects = SceneManager.GetSceneByName(inputSceneName).GetRootGameObjects();
            activeSceneObjects[activeSceneObjects.Length - 1].SetActive(false);

            if (loadNewScene) {
                SceneManager.LoadScene(outputSceneName, LoadSceneMode.Additive);
                loadNewScene = false;
            } else {
                var scene = SceneManager.GetSceneByName(outputSceneName);
                SceneManager.SetActiveScene(scene);
            }
            var newSceneObjects = SceneManager.GetSceneByName(outputSceneName).GetRootGameObjects();
            if (newSceneObjects.Length > 0) {
                var sceneGrid = newSceneObjects[newSceneObjects.Length - 1];
                if (sceneGrid != null) {
                    newSceneObjects[newSceneObjects.Length - 1].SetActive(true);
                }
            }
        }
    }
}
