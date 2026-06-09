using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI Text Fields")]
    public TMP_Text firstPlaceText;
    public TMP_Text secondPlaceText;
    public TMP_Text thirdPlaceText;

    private void Start()
    {
        DisplayLeaderboard(); 
    }

    public void DisplayLeaderboard()
    {
        Format(firstPlaceText, "1st", "Leaderboard_1");
        Format(secondPlaceText, "2nd", "Leaderboard_2");
        Format(thirdPlaceText, "3rd", "Leaderboard_3");
    }

    private void Format(TMP_Text text, string label, string key)
    {
        float time = PlayerPrefs.GetFloat(key, 999999f);

        if (time >= 999999f)
        {
            text.text = $"{label}: No Record";
            return;
        }

        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        float seconds = time % 60f;

        text.text = $"{label}: {hours}h {minutes:00}m {seconds:00.00}s";
    }
}
   