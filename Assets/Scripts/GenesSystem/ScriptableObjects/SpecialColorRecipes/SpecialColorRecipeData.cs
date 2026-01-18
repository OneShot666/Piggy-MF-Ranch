using UnityEngine;

[CreateAssetMenu(fileName = "SpecialColorRecipeData", menuName = "Scriptable Objects/SpecialColorRecipeData")]
public class SpecialColorRecipeData : ScriptableObject
{
    public PigColor parentColorA;
    public PigColor parentColorB;
    public PigColor result;
}
