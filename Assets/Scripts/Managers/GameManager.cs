using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using Fields;
using Scenes;
using Items;
using Save;

// L Increase timer of seeds (grow too fast)
// ? Add watering can
// L [Enclosure/Breeding scenes] Multiply speed of pigs byb their real speed to move in areas
// L Make shortcuts menu (UI) for places (scenes) in island scene
// L Upgrade save system to save pigs
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
namespace Managers {
    public class GameManager : MonoBehaviour {
        [Header("Persistence")]
        [Tooltip("Objects that stay between scenes (inventory is found automatically)")]
        [SerializeField] private List<GameObject> persistentObjects = new();

        [Header("Scene list")]
        [SerializeField] private SceneField islandSceneName;
        [SerializeField] private SceneField houseSceneName;
        [SerializeField] private SceneField farmSceneName;
        [SerializeField] private SceneField breedSceneName;
        [SerializeField] private SceneField marketSceneName;
        [SerializeField] private SceneField raceSceneName;
        [SerializeField] private SceneField portSceneName;

        [Header("Save System")]
        public List<ItemData> allPossibleItems;                                     // ScriptableObjects of items

        private GlobalSaveData _tempSave;                                           // Save current state when switch scene
        private string SavePath => Application.persistentDataPath + "/savegame.json";

        private InventoryManager _inventory;
    
        public static GameManager Instance { get; private set; }

        private void Awake() {
            if (!Instance) {
                Instance = this;
                DontDestroyOnLoad(gameObject);                                      // Stay between scene

                foreach (GameObject obj in persistentObjects) if (obj) DontDestroyOnLoad(obj);

                allPossibleItems = new List<ItemData>(Resources.FindObjectsOfTypeAll<ItemData>());  // Auto-find items
            } else Destroy(gameObject);
        }

        private void Start() {                                                      // Get save data
            StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine() {                                         // Wait clearing time before loading save
            yield return new WaitForEndOfFrame();
            LoadFullGame();
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
            if (kb.f6Key.wasPressedThisFrame || kb.digit6Key.wasPressedThisFrame) LoadScene(portSceneName);
            if (kb.f7Key.wasPressedThisFrame || kb.digit7Key.wasPressedThisFrame) LoadScene(breedSceneName);

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

        private void SaveFullGame() {
            if (!_inventory) return;

            GlobalSaveData data = new GlobalSaveData { money = _inventory.Money }; // Save money and inventory content

            foreach (var item in _inventory.items)
                data.inventory.Add(new ItemSaveData { itemName = item.data.name, quantity = item.quantity });

            var fieldManager = FindFirstObjectByType<FieldManager>(); // Save fields status
            _tempSave = new GlobalSaveData();
            if (fieldManager) {
                var allPlots = fieldManager.GetAllPlots();
                if (allPlots != null && _tempSave is { fields: not null })
                    foreach (var plot in allPlots) if (plot) _tempSave.fields.Add(plot.GetPlotSaveData());
            }

            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(SavePath, json);
            Debug.Log("Game save in : " + SavePath);
        }

        private void LoadFullGame() {
            if (!_inventory) return;

            if (!System.IO.File.Exists(SavePath)) return;

            string json = System.IO.File.ReadAllText(SavePath);
            GlobalSaveData data = JsonUtility.FromJson<GlobalSaveData>(json);

            _inventory.SetMoney(data.money);                                        // Restore inventory
            _inventory.items.Clear();
            foreach (var itemSave in data.inventory) {
                ItemData dataRef = allPossibleItems.Find(i => i.name == itemSave.itemName);
                if (dataRef) _inventory.items.Add(new ItemInstance(dataRef, itemSave.quantity));
            }
            _inventory.UpdateMoneyUI();
            _inventory.RefreshUI();

            // L Restore crop fields (use FieldManager)
        }
    
        private void OnApplicationQuit() {
            SaveFullGame();
        }
    }
}
