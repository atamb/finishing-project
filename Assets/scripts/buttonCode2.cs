using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonCode2 : MonoBehaviour
{
    public GameObject toDestroy;
    public GameObject toCreate;
    // Start is called before the first frame update
    public void MainMenu()
    {
        toCreate.SetActive(true);
        toDestroy.SetActive(false);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(1);
    }
    public void NextScene2()
    {
        SceneManager.LoadScene(2);
    }
    public void MainMenu1()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
