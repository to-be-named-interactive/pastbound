using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    [Header("PlayerStats")]
    public int coins;
    public float maxHealth = 5;

    [Space(20)]

    [Header("Inventroy")]
    public List<InventoryItemInfo> inventoryItems = new List<InventoryItemInfo>();

    [Space(20)]

    [Header("UI Items")]
    [SerializeField] private TextMeshProUGUI coinsUIDisplayer;


    //Private Varibales
    //the old coins amount must me -1 in the begining so we can display coins value even when it's at 0 
    private int oldCoinsAmount = -1;
    private bool displayCoins;

    #region Instance
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    #endregion


    private void Update()
    {
        //Listen if the coins variable changes display the new value on UI 
        displayCoins = (coins != oldCoinsAmount) ?  true :  false;
        
        //if we have a UI element to display to
        if (coinsUIDisplayer != null)
        {
            //Display the changed amount
            if (displayCoins)
            {
                coinsUIDisplayer.text = coins.ToString();
                oldCoinsAmount = coins;
            }
           
            
        }
        
    }


    //Collect coins skatered throughout the world
    public void CollectCoin()
    {
        coins += 1;
    }

   



}
