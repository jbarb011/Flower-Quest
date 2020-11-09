using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]



public class NPC_Interaction : MonoBehaviour
{
    public enum NPC_Type
    {
        Talks,
        Collectable
    }
    public NPC_Type npc_type;

    //Talks Type
    public Sprite icon;
    public string Dialogue_Path;

    //Collectable Type
    public int amount = 1;
}
