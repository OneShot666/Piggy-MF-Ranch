using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using Scenes;
using Items;
using Pigs;
using Save;

// . [SaveSystem] Items dont show at marketplace, so are seeds in cropfields (same pb with build)
// ! [Enclosure/Breeding scenes] Multiply speed of pigs by their real speed to move in areas
// ! Make shortcuts menu (UI) for places (scenes) in island scene
// L After race update, update SaveData.cs to save new data
// ? Add watering can -> allow player to water crop fields
// D Increase timer of seeds (grow too fast)
// LL Add mine (use Pickhammer item)
// LL Add rainbow visual effect on font for rainbow pigs

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

        private InventoryManager _inventory;
        private string SavePath => Application.persistentDataPath + "/savegame.json";

        public static GameManager Instance { get; private set; }
        public GlobalSaveData currentSave = new();                              // Save game's data
        public List<ItemData> allPossibleItems;                                 // ScriptableObjects of items
        public List<PigData> allPossiblePigs;                                   // ScriptableObjects of pigs
        public bool isFirstLaunch = true;

        private void Awake() {
            if (!Instance) {
                Instance = this;
                DontDestroyOnLoad(gameObject);                                  // Self stay between scene

                foreach (GameObject obj in persistentObjects) if (obj) DontDestroyOnLoad(obj);

                allPossibleItems = new List<ItemData>(Resources.FindObjectsOfTypeAll<ItemData>());  // Auto-find items
                allPossiblePigs = new List<PigData>(Resources.FindObjectsOfTypeAll<PigData>());
            } else Destroy(gameObject);                                         // If is a clone
        }

        private void Start() {
            StartCoroutine(LoadRoutine());                                      // Get save data
        }

        private IEnumerator LoadRoutine() {                                     // Wait clearing time before loading save
            yield return new WaitForEndOfFrame();
            LoadFullGame();
        }

        private void Update() {
            if (!_inventory) _inventory = InventoryManager.Instance;            // Try to find inventory in scene
            if (_inventory) DontDestroyOnLoad(_inventory.gameObject);

            HandleGlobalInputs();
        }

        private void HandleGlobalInputs() {
            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            if (kb.tabKey.wasPressedThisFrame) ToggleInventory();               // Open/close inventory with Tab

            // Scene shortcuts
            if (kb.f1Key.wasPressedThisFrame || kb.digit1Key.wasPressedThisFrame) LoadScene(islandSceneName);
            if (kb.f2Key.wasPressedThisFrame || kb.digit2Key.wasPressedThisFrame) LoadScene(houseSceneName);
            if (kb.f3Key.wasPressedThisFrame || kb.digit3Key.wasPressedThisFrame) LoadScene(farmSceneName);
            if (kb.f4Key.wasPressedThisFrame || kb.digit4Key.wasPressedThisFrame) LoadScene(marketSceneName);
            if (kb.f5Key.wasPressedThisFrame || kb.digit5Key.wasPressedThisFrame) LoadScene(raceSceneName);
            if (kb.f6Key.wasPressedThisFrame || kb.digit6Key.wasPressedThisFrame) LoadScene(portSceneName);
            if (kb.f7Key.wasPressedThisFrame || kb.digit7Key.wasPressedThisFrame) LoadScene(breedSceneName);

            if (kb.escapeKey.wasPressedThisFrame) HandleEscape();               // Check which UI to close
        }

        private void ToggleInventory() {
            if (_inventory) _inventory.ToggleOpening();
        }

        private void HandleEscape() {
            if (_inventory && _inventory.IsOpened) _inventory.ToggleOpening();  // Close inventory first
            else LoadScene(islandSceneName);
        }

        public void LoadScene(SceneField scene) {
            if (scene == null || string.IsNullOrEmpty(scene.SceneName)) return;
            if (SceneManager.GetActiveScene().name == scene.SceneName) return;  // Don't load current scene

            SceneManager.LoadScene(scene.SceneName);
        }

        private void LoadFullGame() {
            if (!System.IO.File.Exists(SavePath)) return;

            string json = System.IO.File.ReadAllText(SavePath);
            currentSave = JsonUtility.FromJson<GlobalSaveData>(json);

            if (_inventory) _inventory.LoadData(currentSave, allPossibleItems);
            // L Restore races + the rest
        }
    
        private void SaveFullGame() {
            if (_inventory) SyncInventory(_inventory.Money, _inventory.items);

            string json = JsonUtility.ToJson(currentSave, true);
            System.IO.File.WriteAllText(SavePath, json);
            Debug.Log("Game save in : " + SavePath);                            // !!
        }

        public void SyncInventory(int money, List<ItemInstance> items) {        // Called by InventoryManager
            currentSave.money = money;
            currentSave.inventory.Clear();
            foreach(var item in items)
                currentSave.inventory.Add(new ItemSaveData { itemName = item.data.itemName, quantity = item.quantity });
        }

        public void SyncPigs(List<PigSaveData> pigData) {                       // Called by PigManager
            currentSave.herd = pigData;
        }

        public void SyncFields(List<FieldSaveData> fieldData) {                 // Called by FieldManager
            currentSave.cropfield = fieldData;
        }

        public void SyncMarketplace(List<MarketSaveData> marketplace) {         // Called by MarketManager
            currentSave.marketplace = marketplace;
        }

        private void OnApplicationQuit() {
            SaveFullGame();
        }
    }
}
