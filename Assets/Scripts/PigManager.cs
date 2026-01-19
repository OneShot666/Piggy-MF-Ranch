using System.Collections.Generic;
using UnityEngine;

public class PigManager : MonoBehaviour
{
    public List<Pig> pigs = new List<Pig>();

    public void AddPig(Pig pig)
    {
        pigs.Add(pig);
    }

    public Pig GetPig(int index)
    {
        if (index >= 0 && index < pigs.Count)
            return pigs[index];
        return null;
    }

    public List<Pig> GetRarePigs(PigRarity minRarity) {
        List<Pig> result = new List<Pig>();
        foreach (var pig in pigs) {
            if (pig.Rarity >= minRarity) result.Add(pig);
        }
        return result;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Exemple d'initialisation d'un cochon pour le POC
        /*AddPig(new Pig("Rose", PigRarity.Common, 80f, 60f, PigPower.None));*/
    }

    // Update is called once per frame
    void Update()
    {
        // Ici, on pourrait décrémenter le bien-être de chaque cochon
        foreach (var pig in pigs)
        {
            pig.WellBeing.Decrement(Time.deltaTime);
        }
    }
}
