using UnityEngine;

public class BoogieWoogie : MonoBehaviour
{
    [Header("Ability Status")]
    [SerializeField] private bool hasAbility = false;

    [Header("Raycast Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Input")]
    [SerializeField] private KeyCode swapKey = KeyCode.Mouse2;

    private Rigidbody playerRb;

    public AudioSource audioSource; 

    public AudioClip soundEffectClip; 
    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(swapKey) && hasAbility)
        {
            TrySwapPosition();
        if (audioSource != null && soundEffectClip != null)
        {
        audioSource.PlayOneShot(soundEffectClip);
        }
        }
    }

    private void TrySwapPosition()
    {
        // Find camera if not assigned
        if (playerCamera == null)
        {
            Camera cam = Camera.main;

            if (cam == null)
            {
                Debug.LogError("No Main Camera found!");
                return;
            }

            playerCamera = cam.transform;
        }

        RaycastHit hit;

        if (Physics.Raycast(
                playerCamera.position,
                playerCamera.forward,
                out hit,
                maxDistance,
                interactableLayer))
        {
            GameObject targetObject = hit.collider.gameObject;

            // Don't swap with yourself
            if (targetObject == gameObject)
                return;

            Rigidbody targetRb = targetObject.GetComponent<Rigidbody>();

            // Stop movement before swapping
            if (playerRb != null)
                playerRb.linearVelocity = Vector3.zero;

            if (targetRb != null)
                targetRb.linearVelocity = Vector3.zero;

            // Store positions
            Vector3 playerPosition = transform.position;
            Vector3 targetPosition = targetObject.transform.position;

            // Swap positions
            if (playerRb != null)
                playerRb.MovePosition(targetPosition);
            else
                transform.position = targetPosition;

            if (targetRb != null)
                targetRb.MovePosition(playerPosition);
            else
                targetObject.transform.position = playerPosition;

            Debug.Log($"Swapped with {targetObject.name}");
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