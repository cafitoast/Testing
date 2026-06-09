using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform player;          
    public GameObject bulletPrefab;   
    public Transform firePoint;       
    
    public float attackRange = 15f;  
    public float fireRate = 1f;      
    public float bulletSpeed = 20f;  
    public float timeOfBullet = 2.0f;
    public LayerMask obstacleMask;  
    public AudioSource audioSource; 

    public AudioClip soundEffectClip;

    private float nextFireTime;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && HasLineOfSight())
        {
            Vector3 targetDirection = player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(targetDirection);

            if (Time.time >= nextFireTime)
            {
                ShootProjectile();
                nextFireTime = Time.time + fireRate;
                if (audioSource != null && soundEffectClip != null)
                {
                audioSource.PlayOneShot(soundEffectClip);
                }
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Destroy(bullet, timeOfBullet);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}