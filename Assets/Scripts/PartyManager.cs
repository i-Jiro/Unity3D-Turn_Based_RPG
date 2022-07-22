using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;
    public Inventory Inventory;

    public PartyManager()
    {
        Inventory = new Inventory();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    void Start()
    {
        PreloadItems();
    }

    //Temporary method to test loading items into inventory.
    private void PreloadItems()
    {
        foreach (var itemData in Inventory.preloadItems)
        {
            if(itemData == null) continue;
            Inventory.AddItem(itemData);
        }
    }
}
