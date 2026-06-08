using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    // This function runs when our button is clicked
    public void ClickedTheButton()
    {
            SceneManager.LoadScene("Spawn");
        }


}