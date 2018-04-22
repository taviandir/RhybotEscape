using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null)
        {
            GameObject.Find("ResultText").GetComponent<Text>().text =
                "You made it to floor " + GameManager.instance.level + ".";
        }
    }

    public void OnTryAgain()
    {
        if (GameManager.instance != null)
        {
            // Setting a new parent, that is not set to "dont destroy on load", will destroy the object on scene load/change
            // ref : https://answers.unity.com/questions/18217/undoing-dontdestroyonload-without-immediately-dest.html
            GameObject newObj = new GameObject("GameManager_Destroyer");
            GameManager.instance.transform.parent = newObj.transform;
            SceneManager.LoadScene("mainMenuScene");
        }
    }
}
