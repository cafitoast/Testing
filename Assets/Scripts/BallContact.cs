
using UnityEngine;

public class BallContact : MonoBehaviour

{   
    void Start()
    {
        
    }

 void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerScript = other.GetComponent<PlayerController>();
            playerScript.currentHealth -= 1;
        }
        
        Destroy(gameObject);
    }
}
