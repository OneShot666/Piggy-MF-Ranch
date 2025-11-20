using UnityEngine;

public class PigRunnerUI : MonoBehaviour
{
    public float speed = 5f;
    public Transform finishLine;

    private bool finished = false;

    void Update()
    {
        if (finished || finishLine == null)
            return;

        // Avance vers la ligne d'arrivée
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Vérifie si la ligne d'arrivée est atteinte
        if (transform.position.x >= finishLine.position.x)
        {
            Vector3 pos = transform.position;
            pos.x = finishLine.position.x;
            transform.position = pos;
            finished = true;
        }
    }

    public bool IsFinished()
    {
        return finished;
    }
}