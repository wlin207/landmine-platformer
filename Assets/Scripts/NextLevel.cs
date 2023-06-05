using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public GameManager gameManager;

    public void StartNextLevel()
    {
        gameManager.NextLevel();
    }
}
