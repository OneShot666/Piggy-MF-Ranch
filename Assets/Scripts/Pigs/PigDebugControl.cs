using UnityEngine;

public class PigDebugControl : MonoBehaviour
{
    PigWellBeing pig;

    void Start()
    {
        pig = GetComponent<PigWellBeing>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            pig.FeedThePig(20);

        if (Input.GetKeyDown(KeyCode.H))
            pig.PetThePig(20);

        if (Input.GetKeyDown(KeyCode.C))
            pig.CleanThePig(25);
    }
}