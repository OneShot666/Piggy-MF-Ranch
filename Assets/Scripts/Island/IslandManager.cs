using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Scenes;

namespace Island {
    public class IslandManager : MonoBehaviour {
        private void Start() {
            SceneTeleporter[] teleporters = GetComponentsInChildren<SceneTeleporter>();

            foreach (var tp in teleporters) {
                Button btn = tp.GetComponent<Button>();
                if (!btn) continue;

                CanvasGroup group = btn.GetComponent<CanvasGroup>();
                if (!group) group = btn.gameObject.AddComponent<CanvasGroup>(); // To hide image and text at the same time

                group.alpha = 0f;                                               // Hide by default
                group.interactable = true;
                group.blocksRaycasts = true;

                AddHoverEvents(btn, group);                                     // Detect mouse
                btn.onClick.AddListener(tp.Teleport);                           // Go to target scene on click
            }
        }

        private void AddHoverEvents(Button btn, CanvasGroup group) {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (!trigger) trigger = btn.gameObject.AddComponent<EventTrigger>();

            // Event : PointerEnter
            EventTrigger.Entry entryHover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryHover.callback.AddListener(_ => { group.alpha = 1; });
            trigger.triggers.Add(entryHover);

            // Event : PointerExit
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener(_ => { group.alpha = 0f; });
            trigger.triggers.Add(entryExit);
        }
    }
}
