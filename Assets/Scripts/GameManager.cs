using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;
using Scenes;
using Items;

// ... Making crop field dry when empty
// . Add close button function to Seed Selector
// ! Add use() function in items based on their type
// ! Make save (money, inventory, crop fields, markets...)
// ? Add player
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
public class GameManager : MonoBehaviour {
    [Header("Persistence")]
    [Tooltip("Objects that stay between scenes (inventory is found automatically)")]
    [SerializeField] private List<GameObject> persistentObjects = new();

    [Header("Scene list")]
    [SerializeField] private SceneField islandSceneName;
    [SerializeField] private SceneField houseSceneName;
    [SerializeField] private SceneField farmSceneName;
    [SerializeField] private SceneField marketSceneName;
    [SerializeField] private SceneField raceSceneName;

    private InventoryManager _inventory;
    
    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(gameObject);                                      // Stay between scene

            foreach (GameObject obj in persistentObjects) if (obj) DontDestroyOnLoad(obj);
        } else Destroy(gameObject);
    }

    private void Update() {
        if (!_inventory) _inventory = InventoryManager.Instance;                // Try to find inventory in scene
        if (_inventory) DontDestroyOnLoad(_inventory.gameObject);

        HandleGlobalInputs();
    }

    private void HandleGlobalInputs() {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.tabKey.wasPressedThisFrame) ToggleInventory();                   // Open/close inventory with Tab

        // Scene shortcuts
        if (kb.f1Key.wasPressedThisFrame || kb.digit1Key.wasPressedThisFrame) LoadScene(islandSceneName);
        if (kb.f2Key.wasPressedThisFrame || kb.digit2Key.wasPressedThisFrame) LoadScene(houseSceneName);
        if (kb.f3Key.wasPressedThisFrame || kb.digit3Key.wasPressedThisFrame) LoadScene(farmSceneName);
        if (kb.f4Key.wasPressedThisFrame || kb.digit4Key.wasPressedThisFrame) LoadScene(marketSceneName);
        if (kb.f5Key.wasPressedThisFrame || kb.digit5Key.wasPressedThisFrame) LoadScene(raceSceneName);

        if (kb.escapeKey.wasPressedThisFrame) HandleEscape();                   // Check which UI to close
    }

    private void ToggleInventory() {
        if (_inventory) _inventory.ToggleOpening();
    }

    private void HandleEscape() {
        if (_inventory && _inventory.IsOpened) _inventory.ToggleOpening();      // Close inventory first
        else LoadScene(islandSceneName);
    }

    public void LoadScene(SceneField scene) {
        if (scene == null || string.IsNullOrEmpty(scene.SceneName)) return;
        if (SceneManager.GetActiveScene().name == scene.SceneName) return;      // Don't load current scene

        SceneManager.LoadScene(scene.SceneName);
    }
}
