
using UnityEngine;

public class BallOfDoomContact : MonoBehaviour
{
    public AudioSource audioSource; 

    public AudioClip soundEffectClip;
   
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var playerScript = other.GetComponent<PlayerController>();
                playerScript.currentHealth -= playerScript.currentHealth;
                Destroy(gameObject);
        } 
        else if (other.gameObject.CompareTag("Enemy"))
        Destroy(gameObject);
        {
            Destroy(gameObject);
        }
    }
}
