using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yarn.Unity;

public class DialogueTrigger : InteractBase
{
    [SerializeField] private NPC npcInfo;
    [SerializeField] private GameObject InteractIcon_Keyboard;
    private void Update()
    {
        CastOverlapCircle();

      

        if (hasObjectsEntered)
        {
            if (InteractIcon_Keyboard != null && !InteractIcon_Keyboard.gameObject.activeSelf) InteractIcon_Keyboard.SetActive(true);

            if (Player.Instance.dialogueTrigger == null || Player.Instance.dialogueTrigger != GetComponent<DialogueTrigger>()) 
            {
                Player.Instance.dialogueTrigger = GetComponent<DialogueTrigger>();
            }
        }
        else
        {
            if (InteractIcon_Keyboard != null && InteractIcon_Keyboard.gameObject.activeSelf) InteractIcon_Keyboard.SetActive(false);
        }
        
    }
  
    public void InteractWithNPC()
    {
        if (hasObjectsEntered)
        {
            FindObjectOfType<DialogueRunner>().StartDialogue(npcInfo.npcNodeName);
        }
        
    }

}
