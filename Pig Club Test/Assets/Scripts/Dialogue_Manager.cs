using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour {

    private List<Conversation> conversations;               //Queue containing messages that will appear on screen
    private int curr_conv;                                  //Current position in the conversations Array
    
    private Animator Icon;                                   //The Animator for the Icon
    private Animator Bubble;                                 //The Animator for the Bubble
    private Animator Arrow;

    private Text dialogue_message = null;                    //Text inside the Bubble
    private Text dialogue_name = null;                       //Text for the Name

    private GameObject option_bubble;                        //Bubble for options
    private Text option1 = null;                             //Text for the first option
    private Text option2 = null;                             //Text for second option
    private int curr_opt;                                    //Iterator through list of options

    public bool isRunning;                                  //Bool to tell other scripts if dialogue is running

    private Dialogue curr_message;                           //current message that will be printed to screen

    private Image Icon_Sprite;                      //current icon on screen


    public float timePerCharacter;                          //Time it takes for a character to appear
    private float timer;                                    //Keeps track of time between characters
    private int characterIndex;                             //Current position in current message

    public enum FSM_Dialogue                                //FINITE STATE MACHINE  
    {
        Idle,
        Entrance,
        Text_Crawl,
        Text_Full,
        Options_Entrance,
        Options,
        Proceed,
        Exit,
        Reset
    }
    FSM_Dialogue fsm_dialogue = FSM_Dialogue.Idle;          //Further information see below
    
	void Start ()
    {

        //Initialize messages Queue as empty
        conversations = new List<Conversation>();

        //isRunning is set to false automatically
        isRunning = false;

        //No Current Message Available
        curr_message = null;

        //Get Components for Animators
        Icon = GameObject.Find("/UI/Icon Holder").GetComponent<Animator>();
        Bubble = GameObject.Find("/UI/Text Holder").GetComponent<Animator>();
        Arrow = GameObject.Find("/UI/Text Holder/Option Holder/Arrow").GetComponent<Animator>();
        
        //Get Component for Icon
        Icon_Sprite = GameObject.Find("/UI/Icon Holder/Icon").GetComponent<Image>();

        //Get Components for Option Bubble and hide it
        option_bubble = GameObject.Find("/UI/Text Holder/Option Holder");

        option1 = GameObject.Find("/UI/Text Holder/Option Holder/Option1").GetComponent<Text>();
        option2 = GameObject.Find("/UI/Text Holder/Option Holder/Option2").GetComponent<Text>();
        option_bubble.SetActive(false);

        //Get Components for Text

        dialogue_message = GameObject.Find("/UI/Text Holder/Dialogue Bubble/Text").GetComponent<Text>();
        dialogue_name = GameObject.Find("/UI/Text Holder/Name Bubble/Name").GetComponent<Text>();
    

        //set the current conversation to top of List
        curr_conv = 0;
        curr_opt = 0;
        Icon_Sprite.enabled = false;

    }

    void Update()
    {
        switch (fsm_dialogue)
        {
            /* IDLE
             * 
             * Resting State. Do Nothing.
             *
             *
            */

            case FSM_Dialogue.Idle:
                break;

            /*
             * ENTRANCE
             * 
             * Transition State. When Player interacts with NPC, they will call Start_Dialogue().
             * This state itself does nothing at all, but act as a buffer between the button check
             * on the Player_Interaction Script and the button press check on the 
             * Dialogue_Manager Script.
             * 
            */


            case FSM_Dialogue.Entrance:
                dialogue_name.text = curr_message.name;
                Icon_Sprite.enabled = true;
                fsm_dialogue = FSM_Dialogue.Text_Crawl;
                break;

            /*
             * TEXT CRAWL 
             * 
             * Display characters of message one by one. If you press interact or the message is done
             * go to Text Full.
             * 
            */

            case FSM_Dialogue.Text_Crawl:

                if (Input.GetButtonDown("Interact") || curr_message.message.Length <= characterIndex)
                {
                    DisplayFullMessage();
                    characterIndex = 0;
                    fsm_dialogue = FSM_Dialogue.Text_Full;
                }
                else
                {
                    DisplayTextCrawl();
                }

                break;

            case FSM_Dialogue.Options_Entrance:
                {
                    option_bubble.SetActive(true);
                    curr_opt = 0;
                    option1.text = curr_message.Options[0].choice;
                    option2.text = curr_message.Options[1].choice;
                    fsm_dialogue = FSM_Dialogue.Options;
                }
                break;
            

            case FSM_Dialogue.Options:
                //Scroll through list
                if (Input.GetButtonDown("Vertical"))
                {
                    //if they press Down
                    if (Input.GetAxis("Vertical") < 0 && curr_opt < curr_message.Options.Count - 1)
                    {
                        //Iterate through list
                        curr_opt++;
                        //Check if we have to move arrow
                        if (!Arrow.GetCurrentAnimatorStateInfo(0).IsName("Option 2"))
                        {
                            Arrow.SetTrigger("Down");
                        }

                        //Change options on list
                        option1.text = curr_message.Options[curr_opt - 1].choice;
                        option2.text = curr_message.Options[curr_opt].choice;

                    }

                    //Same as previous but with Up
                    else if (Input.GetAxis("Vertical") > 0 && curr_opt > 0)
                    {
                        curr_opt--;
                        if (!Arrow.GetCurrentAnimatorStateInfo(0).IsName("Option 1"))
                        {
                            Arrow.SetTrigger("Up");
                        }

                        option1.text = curr_message.Options[curr_opt].choice;
                        option2.text = curr_message.Options[curr_opt + 1].choice;
                    }

                }

                if (Input.GetButtonDown("Interact"))
                {
                    for (int i = 0; i < conversations.Count; i++)
                    {
                        if(conversations[i].ID == curr_message.Options[curr_opt].destination)
                        {
                            curr_conv = i;
                        }
                    }
                    option_bubble.SetActive(false);
                    fsm_dialogue = FSM_Dialogue.Proceed;
                }
                break;
            
            /*
             * TEXT FULL
             * 
             * Print full message on screen. Wait for player input to display next message. If no more
             * messages in queue left, leave dialogue manager.
             * 
            */

            case FSM_Dialogue.Text_Full:
                if(Input.GetButtonDown("Interact"))
                {
                    fsm_dialogue = FSM_Dialogue.Proceed;
                }
                break;



            case FSM_Dialogue.Proceed:
                if (conversations[curr_conv].messages.Count > 0)
                {
                    curr_message = conversations[curr_conv].messages.Dequeue();
                    if (curr_message.dialogue_type == Dialogue.Dialogue_Type.options)
                    {
                        fsm_dialogue = FSM_Dialogue.Options_Entrance;
                    }
                    else if (curr_message.dialogue_type == Dialogue.Dialogue_Type.icons)
                    {
                        Debug.Log(curr_message.Icon_path);
                        Icon_Sprite.sprite = Resources.Load<Sprite>(curr_message.Icon_path);
                        break;
                    }
                    else
                    {
                        dialogue_name.text = curr_message.name;
                        fsm_dialogue = FSM_Dialogue.Text_Crawl;
                    }

                }
                else
                {
                    fsm_dialogue = FSM_Dialogue.Exit;
                }

                break;


            /*
             * EXIT
             * 
             * Transition State. Play Dialogue and Icon exit animation. Acts as another buffer 
             * between the Exit animation and Resetting all information in Dialogue Manager.
             * 
            */ 

            case FSM_Dialogue.Exit:
                EndDialogue();
                fsm_dialogue = FSM_Dialogue.Reset;
                break;


            /*
             * RESET
             * 
             * Another Transition State. Waits for Text Bubble to be hidden before resetting
             * all information. Mostly a cosmetic thing. I do not want the text to disappear before
             * it hides.
             */

            case FSM_Dialogue.Reset:
                if (Bubble.GetCurrentAnimatorStateInfo(0).IsName("Hiding"))
                {
                    isRunning = false;
                    dialogue_message.text = null;
                    dialogue_name.text = null;
                    Icon_Sprite.enabled = false;
                    fsm_dialogue = FSM_Dialogue.Idle;
                }
                break;

        }

    }

    public void StartDialogue (string script_path, Sprite icon_sprite)
    {
        Debug.Log("Starting conversation");
        isRunning = true;
        characterIndex = 0;

        Icon_Sprite.sprite = icon_sprite; 

        //clear previous messages
        conversations.Clear();
        curr_conv = 0;

        Icon.SetTrigger("Show");
        Bubble.SetTrigger("Show");

        //Get Xml Document
        XmlDocument document = new XmlDocument();
        document.Load(Application.dataPath + script_path);
        //Get Root Child
        XmlElement rootEle = document.LastChild as XmlElement;
        
        //Look through ever element of the root child
        foreach (XmlElement ele in rootEle.ChildNodes)
        {
            //Check through each conversation
            if(ele.Name == "Conversation")
            {
                //Make a new conversation variable
                Conversation con = new Conversation();
                con.ID = XmlConvert.ToInt16(ele.Attributes["ID"].Value);

                //Check through each element in Conversation Module
                foreach(XmlElement dia_ele in ele.ChildNodes)
                {
                    //Make new Dialogue variable which will be added to Conversation Variable
                    Dialogue new_dialogue = new Dialogue();


                    //if it is an Dialogue keep the name and message
                    if (dia_ele.Name == "Dialogue")
                    {
                        new_dialogue.dialogue_type = Dialogue.Dialogue_Type.text;
                        new_dialogue.name = dia_ele.ChildNodes[0].InnerText;
                        new_dialogue.message = dia_ele.ChildNodes[1].InnerText;

                    }

                    //if it is an Option save it as an Option
                    else if (dia_ele.Name == "Option")
                    {
                        new_dialogue.dialogue_type = Dialogue.Dialogue_Type.options;
                        foreach(XmlElement opt_ele in dia_ele.ChildNodes)
                        {
                            Option new_option = new Option();
                            new_option.choice = opt_ele.InnerText;
                            new_option.destination = XmlConvert.ToInt16(opt_ele.Attributes["ID"].Value);
                            new_dialogue.Options.Add(new_option);
                        }

                    }

                    //if it is Icon, change Icon to new Icon
                    else if(dia_ele.Name == "Icon")
                    {
                        new_dialogue.dialogue_type = Dialogue.Dialogue_Type.icons;
                        new_dialogue.Icon_path = dia_ele.ChildNodes[0].InnerText;
                    }

                    //Add Dialogue to Messages Queue in our current Conversation
                    con.messages.Enqueue(new_dialogue);
                }
                //push new conversation variable onto Conversation List
                conversations.Add(con);
            }

        }

        //display the next message
        curr_message = conversations[curr_conv].messages.Dequeue();

        fsm_dialogue = FSM_Dialogue.Entrance;
    }

    public void DisplayTextCrawl()
    {
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            timer += timePerCharacter;
            characterIndex++;
            dialogue_message.text = curr_message.message.Substring(0, characterIndex);
        }
    }

    public void DisplayFullMessage ()
    {
        //display new dialogue
        dialogue_message.text = curr_message.message;
        Debug.Log(curr_message.message);
    }

    void EndDialogue ()
    {
        //close windows
        Debug.Log("End of Conversation");

        Icon.SetTrigger("Hide");
        Bubble.SetTrigger("Hide");
    }
	
}
