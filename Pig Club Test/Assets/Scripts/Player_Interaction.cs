using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    private Dialogue_Manager dialogue_manager = null;
    private Flower_Manager flower_manager = null;


    public GameObject currentNPC = null;
    public NPC_Interaction currentNPCScript = null;



    void Start()
    {
        dialogue_manager = FindObjectOfType<Dialogue_Manager>();
        flower_manager = FindObjectOfType<Flower_Manager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && currentNPC)
        {
            //If Object Talks
            if (currentNPCScript.npc_type == NPC_Interaction.NPC_Type.Talks && !dialogue_manager.isRunning)
            {
                dialogue_manager.StartDialogue(currentNPCScript.Dialogue_Path, currentNPCScript.icon);
            }

            if (currentNPCScript.npc_type == NPC_Interaction.NPC_Type.Collectable && !dialogue_manager.isRunning)
            {
                flower_manager.Update_Count(currentNPCScript.amount);
                Destroy(currentNPC.gameObject);
                currentNPCScript = null;

            }
        }
    }


    //When object enters Player Collider
    void OnTriggerEnter2D(Collider2D obj)
    {
        //if it's interactable make it the current interactable object
        if (obj.CompareTag("Object_Interactable"))
        {
            currentNPC = obj.gameObject;
            currentNPCScript = currentNPC.GetComponent <NPC_Interaction>();
        }
    }

    //When object leaves Player collider
    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.CompareTag("Object_Interactable"))
        {
            //if current interactable object has left, empty currentNPC
            if(obj.gameObject == currentNPC)
            {
                currentNPC = null;
                currentNPCScript = null;
            }
        }
    }
}
