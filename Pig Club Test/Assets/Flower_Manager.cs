using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Flower_Manager : MonoBehaviour
{
    private int Count;
    private Text Count_Output = null;

    // Start is called before the first frame update
    void Start()
    {
        Count = 0;
        Count_Output = GameObject.Find("/UI/Count Holder/Count").GetComponent<Text>();
        Output_Count();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Update_Count(int x)
    {
        Count += x;
        Output_Count();
    }

    private void Output_Count()
    {
        Count_Output.text = Count.ToString();
    }
    
}
