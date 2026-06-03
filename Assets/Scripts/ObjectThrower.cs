using Unity.VisualScripting;
using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    
   [Header("References")]
    public Transform throwPoint;      
    public GameObject throwablePrefab; 
    [SerializeField] private bool hasAbility = false;
    [Header("Settings")]
    public KeyCode throwKey = KeyCode.Mouse0; 
    public float throwForce = 15f;          
    public float throwUpwardForce = 3f;      
    public float nextFireTime = 15f; 
    public float fireRate = 5;
    public float timer = 2;
    void Update()
    {
        
        if (Input.GetKeyDown(throwKey) && hasAbility)
        {
            if (Time.time >= nextFireTime)
            {
                ThrowObject();
                nextFireTime = Time.time + fireRate;
            }
            
        }
        
    }

    void ThrowObject()
    {
        GameObject projectItem = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);

        Rigidbody rb = projectItem.GetComponent<Rigidbody>();
        Destroy(projectItem.gameObject, timer);
        Vector3 forceDirection = throwPoint.forward;

        Vector3 finalForce = (forceDirection * throwForce) + (throwPoint.up * throwUpwardForce);

        rb.AddForce(finalForce, ForceMode.Impulse);
    }
       private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            hasAbility = true;
            Debug.Log("Picked up rock ability!");

            Destroy(other.gameObject); 
        }
    }
}