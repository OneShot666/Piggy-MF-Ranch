using UnityEngine.EventSystems;
using System.Collections;
using Managers;
using UnityEngine.UI;
using UnityEngine;
using Scenes;

// LL Add screen for each icon on main menu
namespace Island {
    public class IslandManager : MonoBehaviour, IPointerClickHandler {
        [Header("References")]
        [SerializeField] private Button openButton;
        [SerializeField] private CanvasGroup mainMenuPanel;
        [SerializeField] private RectTransform gameImage;
        [SerializeField] private RectTransform leftIcons;
        [SerializeField] private RectTransform rightIcons;

        [Header("Animation settings")]
        [SerializeField] private float animationSpeed = 5f;
        [SerializeField] private float exitOffset = 200f;

        private Coroutine _activeAnim;
        private Vector2 _logoOpenPos, _group1OpenPos, _group2OpenPos;
        private Vector2 _logoClosedPos, _group1ClosedPos, _group2ClosedPos;
        private bool _isMenuClosed;

        private void Start() {
            if (openButton) openButton.gameObject.SetActive(_isMenuClosed);
            CollectPositions();
            InitTeleporters();
            EnableMenu();                                                       // Activated by default
            
            if (GameManager.Instance)
                if (GameManager.Instance.isFirstLaunch) GameManager.Instance.isFirstLaunch = false;
                else SetMenuClosedInstant();
        }

        private void CollectPositions() {
            _logoOpenPos = gameImage.anchoredPosition;                           // Initial positions
            _group1OpenPos = leftIcons.anchoredPosition;
            _group2OpenPos = rightIcons.anchoredPosition;

            _logoClosedPos = _logoOpenPos + new Vector2(0, gameImage.rect.height + exitOffset); // Closed position
            _group1ClosedPos = _group1OpenPos - new Vector2(exitOffset, leftIcons.rect.height + exitOffset);
            _group2ClosedPos = _group2OpenPos - new Vector2(-exitOffset, rightIcons.rect.height + exitOffset);
        }

        private void InitTeleporters() {
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

        public void OnPointerClick(PointerEventData eventData) {
            if (!_isMenuClosed) CloseMenu();                                    // Close menu by clicking on it
        }

        private void EnableMenu() {
            if (mainMenuPanel) {
                mainMenuPanel.interactable = true;
                mainMenuPanel.blocksRaycasts = true;
                mainMenuPanel.alpha = 1;
            }
        }

        private void DisableMenu() {
            if (mainMenuPanel) {
                mainMenuPanel.interactable = false;
                mainMenuPanel.blocksRaycasts = false;
                mainMenuPanel.alpha = 1;
            }
        }

        [ContextMenu("Open Menu")]
        public void OpenMenu() {
            _isMenuClosed = false;
            StopActiveAnimation();
            if (openButton) openButton.gameObject.SetActive(_isMenuClosed);
            _activeAnim = StartCoroutine(AnimateTo(_logoOpenPos, _group1OpenPos, _group2OpenPos));
        }

        [ContextMenu("Close Menu")]
        public void CloseMenu() {
            _isMenuClosed = true;
            StopActiveAnimation();
            _activeAnim = StartCoroutine(AnimateTo(_logoClosedPos, _group1ClosedPos, _group2ClosedPos));
            if (openButton) openButton.gameObject.SetActive(_isMenuClosed);
        }

        private void SetMenuClosedInstant() {
            StopActiveAnimation();
            _isMenuClosed = true;

            gameImage.anchoredPosition = _logoClosedPos;                        // Place UI objects to close positions
            leftIcons.anchoredPosition = _group1ClosedPos;
            rightIcons.anchoredPosition = _group2ClosedPos;

            DisableMenu();
        }

        private IEnumerator AnimateTo(Vector2 logoTarget, Vector2 g1Target, Vector2 g2Target) {
            if (_isMenuClosed) DisableMenu(); else EnableMenu();

            while (Vector2.Distance(gameImage.anchoredPosition, logoTarget) > 0.5f) {
                float speed = animationSpeed * Time.deltaTime;
                gameImage.anchoredPosition = Vector2.Lerp(gameImage.anchoredPosition, logoTarget, speed);
                leftIcons.anchoredPosition = Vector2.Lerp(leftIcons.anchoredPosition, g1Target, speed);
                rightIcons.anchoredPosition = Vector2.Lerp(rightIcons.anchoredPosition, g2Target, speed);
                yield return null;
            }

            gameImage.anchoredPosition = logoTarget;
            leftIcons.anchoredPosition = g1Target;
            rightIcons.anchoredPosition = g2Target;
        }

        private void StopActiveAnimation() {
            if (_activeAnim != null) StopCoroutine(_activeAnim);
        }
    }
}
