using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Chức năng điều khiển của nút bấm: Di chuyển giữa các scene & thoát phần mềm

public class ButtonManager : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void OpenScene()
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Moved to " + sceneName);
    }
    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Exit App");
    }
}
