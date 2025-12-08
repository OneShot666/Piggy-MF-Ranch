using UnityEngine;

// . Remove item overlay when buy it
// ! Add overlay of item in inventory
// ! Add sell feature from inventory
// ! Use fields
// L Add use() function in items based on their type
// ? Add player
// L Add map & travels
public class CameraManager : MonoBehaviour {                                    // [UNUSED in this project -> 2D]
    [Header("Camera settings")]
    [Tooltip("Speed of camera movement")]
    [SerializeField] private float speed = 10f;
    [Tooltip("Speed of camera rotation")]
    [SerializeField] private float rotation = 100f;

    [Header("Target settings")]
    [Tooltip("[Optional] Target to follow (free camera mode if target is null)")]
    [SerializeField] private Transform target;
    [Tooltip("Distance from the target")]
    [SerializeField] private float offSet = 50f;                                // Distance from target if any
    
    private bool _isPaused;

    void Start() {
        if (target) transform.LookAt(target);                                   // Look at target if any
    }

    void Update() {
        if (target) {
            transform.position = Vector3.Lerp(transform.position, 
                target.position + target.forward * offSet, speed * Time.unscaledDeltaTime);
            transform.LookAt(target);
        } else {                                                                // Free camera mode
            MoveCamera();
            RotateCamera();
        }
    }

    private void MoveCamera() {
        if (Input.GetKeyDown(KeyCode.Space)) {                                  // Un/pause
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        Vector3 dir = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) dir += transform.forward;                  // Forward
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;                  // Backward
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;                    // Left
        if (Input.GetKey(KeyCode.D)) dir += transform.right;                    // Right
        if (Input.GetKey(KeyCode.Q)) dir += Vector3.up;                         // Up
        if (Input.GetKey(KeyCode.E)) dir -= Vector3.up;                         // Down

        transform.position += dir.normalized * (speed * Time.unscaledDeltaTime);  // Move camera
    }

    private void RotateCamera() {
        float yaw = 0f;                                                         // Horizontal rotation
        float pitch = 0f;                                                       // Vertical rotation

        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f; 
        if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f; 
        if (Input.GetKey(KeyCode.UpArrow)) if (target) offSet--; else pitch = -1f; 
        if (Input.GetKey(KeyCode.DownArrow)) if (target) offSet++; else pitch = 1f;

        if (target) offSet = Mathf.Max(1f, offSet);

        transform.Rotate(Vector3.up, yaw * rotation * Time.unscaledDeltaTime, Space.World); 
        transform.Rotate(Vector3.right, pitch * rotation * Time.unscaledDeltaTime, Space.Self);
    }
}
