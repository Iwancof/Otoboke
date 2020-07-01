using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMngr : MonoBehaviour
{
    enum SceneType {
        Title,
        GameMain,
        GameOver,
        GameClear,
    };
    SceneType currentScene;
    string currentSceneString;
    static AsyncOperation asyncOperation, clearOperation;
    // Start is called before the first frame update
    void Start()
    {
        currentSceneString = SceneManager.GetActiveScene().name;
        switch(currentSceneString) {
            case "GameOver": currentScene = SceneType.GameOver; break;
            case "Title": currentScene = SceneType.Title; break;
            case "GameMain": currentScene = SceneType.GameMain; break;
            case "GameClear": currentScene = SceneType.GameClear; break;
            default: currentScene = SceneType.Title; break;
        }
        switch(currentScene) {
            case SceneType.GameOver: {
                asyncOperation = SceneManager.LoadSceneAsync("Title");
                asyncOperation.allowSceneActivation = false;
                break;
            }
            case SceneType.Title: {
                break;
            }
            case SceneType.GameMain: {
                asyncOperation = SceneManager.LoadSceneAsync("GameOver");
                clearOperation = SceneManager.LoadSceneAsync("GameClear");
                asyncOperation.allowSceneActivation = false;
                clearOperation.allowSceneActivation = false;
                break;
            }
            case SceneType.GameClear: {
                asyncOperation = SceneManager.LoadSceneAsync("Title");
                asyncOperation.allowSceneActivation = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentScene) {
            case SceneType.Title: {
                if(Title.canStartGame) {
                    SceneManager.LoadScene("GameMain");
                }
                break;
            }
            case SceneType.GameOver: {
                if(Input.GetKeyDown(KeyCode.Return)) asyncOperation.allowSceneActivation = true;
                break;
            }
            case SceneType.GameMain: {
                if(PointManager.defeat) asyncOperation.allowSceneActivation = true;
                if (PointManager.clear) clearOperation.allowSceneActivation = true;
                break;
            }
            case SceneType.GameClear: {
                if (Input.GetKeyDown(KeyCode.Return)) asyncOperation.allowSceneActivation = true;
                break;
            }
        }
    }

}
