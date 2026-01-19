using UnityEngine;

[CreateAssetMenu(fileName = "ColorPowerAffinityData", menuName = "Scriptable Objects/ColorPowerAffinityData")]
public class ColorPowerAffinityData : ScriptableObject
{
    public PigColor color;
    public PigSpecialPower favoredPower;
    public float powerBoostMultiplier = 5f;
}
