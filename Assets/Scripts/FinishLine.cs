using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    private bool finished = false;
    private bool active = false;
    public PlayerController playerScript;
    private void Start()
    {
        Invoke(nameof(Activate), 1f);
    }

    private void Activate()
    {
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (finished) return;

        if (other.CompareTag("Player"))
        {
            finished = true;
            float newTime =
                (playerScript.hourCount * 3600f) +
                (playerScript.minuteCount * 60f) +
                playerScript.secondsCount;

            SaveTime(newTime);

            SceneManager.LoadScene("MyEyesFeelHeavy");
        }
    }

    private void SaveTime(float newTime)
    {
        Debug.Log("New time: " + newTime);
        
        float score1 = PlayerPrefs.GetFloat("Leaderboard_1", 999999f);
        float score2 = PlayerPrefs.GetFloat("Leaderboard_2", 999999f);
        float score3 = PlayerPrefs.GetFloat("Leaderboard_3", 999999f);
        
        Debug.Log($"Existing scores: {score1}, {score2}, {score3}");

        if (newTime < score1)
        {
            PlayerPrefs.SetFloat("Leaderboard_3", score2);
            PlayerPrefs.SetFloat("Leaderboard_2", score1);
            PlayerPrefs.SetFloat("Leaderboard_1", newTime);
            Debug.Log("Saved as 1st");
        }
        else if (newTime < score2)
        {
            PlayerPrefs.SetFloat("Leaderboard_3", score2);
            PlayerPrefs.SetFloat("Leaderboard_2", newTime);
            Debug.Log("Saved as 2nd");
        }
        else if (newTime < score3)
        {
            PlayerPrefs.SetFloat("Leaderboard_3", newTime);
            Debug.Log("Saved as 3rd");
        }
        else
        {
            Debug.Log("Time not fast enough to place");
        }
        PlayerPrefs.Save();
    }
}