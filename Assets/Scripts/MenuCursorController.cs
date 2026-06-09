using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuCursorController : MonoBehaviour
{

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MyEyesFeelHeavy")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}