using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public GameObject tutorial;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Tutorial(bool on)
    {
        tutorial.SetActive(on);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
