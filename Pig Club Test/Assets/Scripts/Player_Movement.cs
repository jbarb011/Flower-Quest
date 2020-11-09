using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    private Rigidbody2D myRigidBody = null;
    private Dialogue_Manager dialogue_manager = null;

    private Animator myAnimator = null;

    public float slow_butt;
    public float fast_ass;

    float moveSpeed;

    bool facingRight;

    void Move(float Horizontal, float Vertical)
    {
        //if ur running
        if (Input.GetButton("Run"))
        {
            moveSpeed = fast_ass;
        }
        //if ur not
        else
        {
            moveSpeed = slow_butt;
        }


        //flip
        if ((facingRight && Horizontal < 0) || (!facingRight && Horizontal  > 0))
        {
            facingRight = !facingRight;

            Vector3 scale_hold = transform.localScale;
            scale_hold.x *= -1;
            transform.localScale = scale_hold;
        }
        
        if(Horizontal != 0)
        {
            myAnimator.SetBool("Down", false);
            myAnimator.SetBool("Side", true);
            myAnimator.SetBool("Up", false);

        }
        else if(Vertical < 0)
        {
            myAnimator.SetBool("Down", true);
            myAnimator.SetBool("Side", false);
            myAnimator.SetBool("Up", false);
        }
        else if (Vertical > 0)
        {
            myAnimator.SetBool("Down", false);
            myAnimator.SetBool("Side", false);
            myAnimator.SetBool("Up", true);
        }
        else
        {
            myAnimator.SetBool("Down", false);
            myAnimator.SetBool("Side", false);
            myAnimator.SetBool("Up", false);
        }


        //movement
        myRigidBody.velocity = new Vector2(Horizontal * moveSpeed, Vertical * moveSpeed);
    }




    // Start is called before the first frame update
    void Start()
    { 
        //get rigid body component from player
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();

        dialogue_manager = FindObjectOfType<Dialogue_Manager>();

        // set facing right
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // check if dialogue is on
        if (!dialogue_manager.isRunning)
        {
            //Move character
            Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            myAnimator.SetBool("Down", false);
            myAnimator.SetBool("Side", false);
        }

    }
}
