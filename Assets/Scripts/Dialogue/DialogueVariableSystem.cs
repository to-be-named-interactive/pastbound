using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class DialogueVariableSystem : DialogueViewBase
{
    [SerializeField]
    private GameObject dialogueBoxBackground;
    
    // variables
    private static bool isPaid;

    // scripts
    DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        
        dialogueRunner.AddFunction<bool>("are_the_coins_paid", AreTheCoinsPaid);
        dialogueRunner.AddFunction<int>("player_coins", PlayerCoins);
        dialogueRunner.AddCommandHandler<bool>("has_been_paid", HasBeenPaid);
        dialogueRunner.AddCommandHandler<int>("remove_player_coins", RemovePlayerCoins);
    }

    private void Start()
    {
        // dialogueRunner Event subscription
        dialogueRunner.onDialogueStart.AddListener(dialogueRunner_OnDialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(dialogueRunner_OnDialogueComplete);
    }
    
    private void dialogueRunner_OnDialogueStart()
    {
        // Set dialogueBoxBackground ACTIVE
        dialogueBoxBackground.SetActive(true);

        // Freeze the player when in dialogue
        Player.Instance.FreezePlayer();
    }
    
    private void dialogueRunner_OnDialogueComplete()
    {
        // Set dialogueBoxBackground UNACTIVE
        dialogueBoxBackground.SetActive(false);

        // Unfreeze player after completing a dialogue 
        Player.Instance.UnFreezePlayer();
    }

    // Functions
    private static int PlayerCoins()
    {
        return GameManager.Instance.coins;
    }
    
    private static bool AreTheCoinsPaid()
    {
        if (isPaid)
        {
            return true;
        }

        return false;
    }
    
    // Commands
    private void HasBeenPaid(bool isTrue)
    {
        isPaid = isTrue;
    }

    private void RemovePlayerCoins(int removeValue)
    {
        GameManager.Instance.coins -= removeValue;
    }
}