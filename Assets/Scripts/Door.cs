using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            LoadNewScene();
        }
    }

    private void LoadNewScene()
    {
        if (!string.IsNullOrEmpty("FinalLevel"))
        {
            SceneManager.LoadScene("FinalLevel");
        }
        else
        {
            Debug.LogWarning("Scene name is empty on ");
        }
    }
}
