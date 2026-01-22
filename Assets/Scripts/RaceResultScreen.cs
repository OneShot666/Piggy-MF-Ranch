using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceResultsScreen : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject endScreenRoot;

    [Header("Victory / Defeat")]
    [SerializeField] private GameObject victoryObject;
    [SerializeField] private GameObject defeatObject;

    [Header("Spots (1..6) - Pigs placeholders")]
    [Tooltip("6 transforms (cochons placeholders) dans l'ordre 1st..6th")]
    [SerializeField] private Transform[] spotPigs = new Transform[6];

    [Header("FX")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private ParticleSystem rain;

    [Header("Anim - Jump")]
    [SerializeField] private float jumpOffsetY = 100f;
    [SerializeField] private float jumpUpTime = 0.08f;
    [SerializeField] private float jumpDownTime = 0.08f;

    [Header("Anim - Timing")]
    [SerializeField] private float top3LoopPause = 0.25f;
    [SerializeField] private float lookIntervalMin = 0.6f;
    [SerializeField] private float lookIntervalMax = 1.2f;

    private Coroutine playerAnimCoroutine;
    private Transform currentPlayerPig;
    private Vector3 currentPlayerBaseLocalPos;

    void Awake()
    {
        if (endScreenRoot != null) endScreenRoot.SetActive(false);
        if (victoryObject != null) victoryObject.SetActive(false);
        if (defeatObject != null) defeatObject.SetActive(false);

        SetFx(confetti, false);
        SetFx(rain, false);
    }

    void OnDisable()
    {
        StopPlayerAnim();
        SetFx(confetti, false);
        SetFx(rain, false);
    }

    public void Hide()
    {
        StopPlayerAnim();
        SetFx(confetti, false);
        SetFx(rain, false);

        if (endScreenRoot != null) endScreenRoot.SetActive(false);
    }

    public void Show(List<Sprite> rankedSprites, int playerRank) // playerRank: 1..6
    {
        if (endScreenRoot != null) endScreenRoot.SetActive(true);

        // Place sprites into spot pigs (1..6)
        for (int i = 0; i < spotPigs.Length; i++)
        {
            Transform spot = spotPigs[i];
            if (spot == null) continue;

            var sr = spot.GetComponentInChildren<SpriteRenderer>(true);
            if (sr == null) continue;

            Sprite s = (rankedSprites != null && i < rankedSprites.Count) ? rankedSprites[i] : null;
            sr.sprite = s;
            sr.enabled = (s != null);

            // Reset rotation only
            Vector3 e = spot.localEulerAngles;
            e.y = 0f;
            spot.localEulerAngles = e;
        }

        bool isTop3 = playerRank >= 1 && playerRank <= 3;

        if (victoryObject != null) victoryObject.SetActive(isTop3);
        if (defeatObject != null) defeatObject.SetActive(!isTop3);

        // FX: top3 => confetti, sinon => rain
        SetFx(confetti, isTop3);
        SetFx(rain, !isTop3);

        StopPlayerAnim();

        int idx = playerRank - 1;
        if (idx < 0 || idx >= spotPigs.Length) return;

        currentPlayerPig = spotPigs[idx];
        if (currentPlayerPig == null) return;

        currentPlayerBaseLocalPos = currentPlayerPig.localPosition;

        playerAnimCoroutine = isTop3
            ? StartCoroutine(Top3AnimLoop(currentPlayerPig))
            : StartCoroutine(LoseIdleLookLoop(currentPlayerPig));
    }

    private void SetFx(ParticleSystem ps, bool enable)
    {
        if (ps == null) return;

        if (enable)
        {
            ps.gameObject.SetActive(true);
            ps.Play(true);
        }
        else
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.gameObject.SetActive(false);
        }
    }

    private void StopPlayerAnim()
    {
        if (playerAnimCoroutine != null)
        {
            StopCoroutine(playerAnimCoroutine);
            playerAnimCoroutine = null;
        }

        if (currentPlayerPig != null)
        {
            currentPlayerPig.localPosition = currentPlayerBaseLocalPos;
            Vector3 e = currentPlayerPig.localEulerAngles;
            e.y = 0f;
            currentPlayerPig.localEulerAngles = e;
        }

        currentPlayerPig = null;
    }

    private IEnumerator Top3AnimLoop(Transform pig)
    {
        while (endScreenRoot == null || endScreenRoot.activeInHierarchy)
        {
            ToggleLook(pig);
            yield return JumpOnce(pig);
            yield return JumpOnce(pig);

            if (top3LoopPause > 0f)
                yield return new WaitForSeconds(top3LoopPause);
            else
                yield return null;
        }
    }

    private IEnumerator LoseIdleLookLoop(Transform pig)
    {
        while (endScreenRoot == null || endScreenRoot.activeInHierarchy)
        {
            float wait = Random.Range(lookIntervalMin, lookIntervalMax);
            yield return new WaitForSeconds(wait);
            ToggleLook(pig);
        }
    }

    private void ToggleLook(Transform pig)
    {
        if (pig == null) return;

        Vector3 e = pig.localEulerAngles;
        e.y = (Mathf.Abs(e.y - 180f) < 0.1f) ? 0f : 180f;
        pig.localEulerAngles = e;
    }

    private IEnumerator JumpOnce(Transform pig)
    {
        if (pig == null) yield break;

        Vector3 start = currentPlayerBaseLocalPos;
        Vector3 up = start + new Vector3(0f, jumpOffsetY, 0f);

        float t = 0f;
        while (t < jumpUpTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / jumpUpTime);
            pig.localPosition = Vector3.Lerp(start, up, a);
            yield return null;
        }

        t = 0f;
        while (t < jumpDownTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / jumpDownTime);
            pig.localPosition = Vector3.Lerp(up, start, a);
            yield return null;
        }

        pig.localPosition = start;
    }
}
