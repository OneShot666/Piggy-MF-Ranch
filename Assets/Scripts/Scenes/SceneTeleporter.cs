using Managers;
using UnityEngine;

namespace Scenes {
    public class SceneTeleporter : MonoBehaviour {
        public SceneField targetScene;

        public void Teleport() {
            if (GameManager.Instance) GameManager.Instance.LoadScene(targetScene);
        }
    }
}
