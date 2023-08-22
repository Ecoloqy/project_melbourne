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
            foreach (var sceneObject in activeSceneObjects) {
                Debug.Log(sceneObject.name);
                if (sceneObject.name == "Grid") {
                    sceneObject.SetActive(false);
                }
            }

            if (loadNewScene) {
                SceneManager.LoadScene(outputSceneName, LoadSceneMode.Additive);
                loadNewScene = false;
            } else {
                var scene = SceneManager.GetSceneByName(outputSceneName);
                SceneManager.SetActiveScene(scene);
            }
            var newSceneObjects = SceneManager.GetSceneByName(outputSceneName).GetRootGameObjects();
            if (newSceneObjects.Length > 0) {
                foreach (var newSceneObject in newSceneObjects) {
                    Debug.Log(newSceneObject.name);
                    if (newSceneObject.name == "Grid") {
                        newSceneObject.SetActive(true);
                    }
                }
            }
        }
    }
}
