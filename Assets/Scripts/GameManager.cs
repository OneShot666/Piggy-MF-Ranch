using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;
using Scenes;
using Items;

// ... Polishing fields generation
// . Making switch between scenes work
// ! Add water fields, plant seeds and harvest functions
// L Add use() function in items based on their type
// L Add map & travels
// ? Add player
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
public class GameManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private SceneField farmSceneName;
    [SerializeField] private SceneField marketSceneName;

    private InventoryManager _inventory;

    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(gameObject);                                      // Stay between scene
        } else Destroy(gameObject);
    }

    private void Start() {
        _inventory = FindFirstObjectByType<InventoryManager>();                 // Try to find inventory in scene
    }

    private void Update() {
        HandleGlobalInputs();
    }

    private void HandleGlobalInputs() {
        if (Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame) ToggleInventory();     // Open/close inventory with Tab

        if (Keyboard.current.f1Key.wasPressedThisFrame) LoadScene(farmSceneName);   // Scene shortcuts
        if (Keyboard.current.f2Key.wasPressedThisFrame) LoadScene(marketSceneName);

        if (Keyboard.current.escapeKey.wasPressedThisFrame) HandleEscape();     // Check which UI to close
    }

    private void ToggleInventory() {
        if (_inventory) _inventory.ToggleOpening();
    }

    private void HandleEscape() {
        if (_inventory && _inventory.IsOpened) _inventory.ToggleOpening();      // Close inventory first
        else Debug.Log("Opening main menu...");                                 // L Close Main Menu
    }

    private void LoadScene(SceneField scene) {
        if (scene != null && !string.IsNullOrEmpty(scene.SceneName)) SceneManager.LoadScene(scene.SceneName);
    }
}
