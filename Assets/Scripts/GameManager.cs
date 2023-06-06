using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;

    public GameObject completeLevelUI;

    public void EndGame() {
        if (!gameHasEnded) {
            gameHasEnded = true;
            // gameover
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("level complete called");
        completeLevelUI.SetActive(true);
    }

    public void NextLevel() {
        Debug.Log("getting next level");
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
