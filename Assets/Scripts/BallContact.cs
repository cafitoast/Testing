
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
        if (playerScript != null)
            playerScript.currentHealth -= 1;

        Destroy(gameObject);
    }
    else if (other.gameObject.CompareTag("Enemy"))
    {
    return;
    }
    else
    {
        Destroy(gameObject); 
    }
}
}