using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;

    void Start()
    {
        director.Play();
    }
}




// using UnityEngine;
// using UnityEngine.Playables;

// public class CutsceneManager : MonoBehaviour
// {
//     public PlayableDirector director;
//     public PlayerController playerController;

//     void Start()
//     {
//         playerController.enabled = false;
//         director.Play();
//         director.stopped += OnCutsceneEnd;
//     }

//     void OnCutsceneEnd(PlayableDirector d)
//     {
//         playerController.enabled = true;
//     }
// }