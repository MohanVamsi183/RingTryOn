using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
public void ManoScene()
{
    SceneManager.LoadScene("ManoRing");
}

public void ExitApp()
{
    Application.Quit();
    Debug.Log("application exited");
}

public void ReturnMenu()
{
    SceneManager.LoadScene("MainMenu");
}

public void VufoScene()
{
    SceneManager.LoadScene("VuforiaScene");
}

public void ViewSSscene()
{
    SceneManager.LoadScene("SSscene");
}
}
 