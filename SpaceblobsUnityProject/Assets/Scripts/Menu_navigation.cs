using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_navigation : MonoBehaviour
{
    public void OpenMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void PlaySingleplayer()
    {
        SceneManager.LoadScene(1);
        GameObject.Find("GameMenuUI").GetComponent<Canvas>().gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
