using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public static bool battleOver;
    public int team;
    public CharacterController controller;

    float horizontalMove = 0;
    public float runSpeed = 40f;
    bool jump = false;

    [SerializeField] GameObject groundCheck;
  
    void Start()
    {
        battleOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!battleOver)
        {
            if (team == 0)
            {
                horizontalMove = Input.GetAxis("Horizontal White");

                if (Input.GetButtonDown("Jump White"))
                {
                    jump = true;
                }
     
           
            }
            else if (team == 1)
            {
                horizontalMove = Input.GetAxis("Horizontal Black");
             
                if (Input.GetButtonDown("Jump Black"))
                {
                    jump = true;
                }


                   
                
            }
        }
    }
    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime * runSpeed, false, jump);
        jump = false;
    }
}
