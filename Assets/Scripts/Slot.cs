using UnityEngine;

[CreateAssetMenu(fileName = "NewSlot", menuName = "ScriptableObjects/Slot", order = 1)]
public class Slot : ScriptableObject
{
    public int SlotId;
    public Sprite Icon;
    public int Multiplier;
    public bool IsBomb;
    public float Possibility;
}