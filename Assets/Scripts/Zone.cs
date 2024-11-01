using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZone", menuName = "ScriptableObjects/Zone", order = 1)]
public class Zone : ScriptableObject
{
    public int ZoneId;
    public List<Slot> Slots;
}