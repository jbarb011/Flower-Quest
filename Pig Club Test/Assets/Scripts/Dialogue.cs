using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Option
{
    public string choice;
    public int destination;
}

public class Dialogue
{
    public enum Dialogue_Type
    {
        text,
        options,
        icons
    }
    public Dialogue_Type dialogue_type;
    public string name;
    public string message;
    public List<Option> Options;
    public string Icon_path;

    public Dialogue(string n, string m, Dialogue_Type x)
    {
        name = n;
        message = m;
        dialogue_type = x;
        Options = new List<Option>();
    }

    public Dialogue()
    {
        name = null;
        message = null;
        dialogue_type = Dialogue_Type.text;
        Options = new List<Option>();
    }
}

public class Conversation
{
    public int ID;
    public Queue<Dialogue> messages;

    public Conversation()
    {
        int ID = 0;
        messages = new Queue<Dialogue>();
    }
}
