using UnityEngine;

namespace Scenes {
    [System.Serializable]
    public class SceneField {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string sceneName = "";

        public string SceneName => sceneName;

        public static implicit operator string(SceneField sceneField) {         // Implicit -> can be use as string
            return sceneField.SceneName;
        }
    }
}
