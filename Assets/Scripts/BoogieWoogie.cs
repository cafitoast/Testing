using UnityEngine;

public class BoogieWoogie : MonoBehaviour
{
    [Header("Ability Status")]
    [SerializeField] private bool hasAbility = false;
    // Assign these in the Unity Inspector
    [Header("Raycast Settings")]
    [SerializeField] private Transform playerCamera; 
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Input")]
    [SerializeField] private KeyCode swapKey = KeyCode.Mouse2;

    void Update()
    {
        // Check for the swap input key
        if (Input.GetKeyDown(swapKey) && hasAbility)
        {
            TrySwapPosition();
        }
    }

    private void TrySwapPosition()
    {
        {
            playerCamera = Camera.main.transform;
        }

        RaycastHit hit;
        
        // Cast a ray from the center of the camera forward
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, interactableLayer))
        {
            GameObject targetObject = hit.collider.gameObject;
            
            Vector3 playerCurrentPosition = transform.position;
            transform.position = targetObject.transform.position;
            targetObject.transform.position = playerCurrentPosition;
            
            Debug.Log($"Swapped positions with {targetObject.name}!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AbilityItem"))
        {
            hasAbility = true;
            Debug.Log("Picked up swap ability!");

            Destroy(other.gameObject); 
        }
    }
}