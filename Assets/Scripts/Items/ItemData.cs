using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string Name;
    public string Description;
    
    public abstract Item Initialize(Inventory inventory);
}
