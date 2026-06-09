using UnityEngine;

public class EnemyShootingTier2 : MonoBehaviour
{
    public Transform player;          
    public GameObject bullet2Prefab;   
    public Transform firePoint;       

    
    public float attackRange = 15f;  
    public float fireRate = 1f;      
    public float bulletSpeed = 20f;  
    public float timeOfBullet = 2.0f;
    private float nextFireTime;
      public LayerMask obstacleMask; 
          public AudioSource audioSource; 

    public AudioClip soundEffectClip;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
   
            Vector3 targetDirection = player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(targetDirection);

            if (Time.time >= nextFireTime)
            {
                ShootProjectile();
                if (audioSource != null && soundEffectClip != null)
                {
                audioSource.PlayOneShot(soundEffectClip);
                }
                nextFireTime = Time.time + fireRate;
                
            }
        }
    }
    bool HasLineOfSight()
    {
        Vector3 directionToPlayer = player.position - firePoint.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (Physics.Raycast(firePoint.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer, obstacleMask))
        {
            // something is blocking the path
            return false;
        }

        return true;
    }
    void ShootProjectile()
    {
      
        GameObject bullet2 = Instantiate(bullet2Prefab, firePoint.position, firePoint.rotation);
        
    
        Destroy(bullet2, timeOfBullet);
        
        Rigidbody rb = bullet2.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
        
    }
}
