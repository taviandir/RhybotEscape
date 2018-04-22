using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;
    public GameObject gameWinMenu;

    void Start()
    {
        // UGLY HACK TO END GAME, part 2
        if (GameManager.instance.level == GameManager.instance.levelEnd)
        {
            // display win menu
            gameWinMenu.SetActive(true);

            GameObject.Find("ResultText").GetComponent<Text>().text =
                "You made it thru all " + (GameManager.instance.level - 1) + " floors! Thanks for playing!";
        }
        else
        {
            // display game over menu
            gameOverMenu.SetActive(true);

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
