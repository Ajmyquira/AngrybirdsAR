using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Exit()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
