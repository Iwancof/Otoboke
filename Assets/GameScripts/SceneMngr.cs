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
        /*
        switch(currentScene) {
            case SceneType.GameOver: {
                SceneManager.UnloadSceneAsync("GameClear");
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
                SceneManager.UnloadSceneAsync("GameOver");
                asyncOperation = SceneManager.LoadSceneAsync("Title");
                asyncOperation.allowSceneActivation = false;
                break;
            }
        }
        */
        Debug.LogWarning(currentSceneString);
    }

    // Update is called once per frame
    Vector2 swipeDir = Vector2.zero;

    void Update()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            swipeDir = Swipe.SwipeDirection();
        }
        switch(currentScene) {
            case SceneType.Title: {
                if(Title.canStartGame) {
                    SceneManager.LoadScene("GameMain");
                }
                break;
            }
            case SceneType.GameOver: {
                goto case SceneType.GameClear;
            }
            case SceneType.GameClear: {
                if(Input.GetKeyDown(KeyCode.Return) || swipeDir == Vector2.right) SceneManager.LoadSceneAsync("Title");
                CheckBackToTitle();
                break;
            }
            case SceneType.GameMain: {
                if(PointManager.clear) SceneManager.LoadSceneAsync("GameClear");
                if(PointManager.defeat) SceneManager.LoadSceneAsync("GameOver");
                CheckBackToTitle();
                break;
            }
        }
    }

    void CheckBackToTitle() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Title");
        }
    }
}
