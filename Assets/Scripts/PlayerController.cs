using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour {

    public GameController gameController;

    private bool m_ShotAxisPressed = false;

    private Animator animator;

    public int health;

    public PlayerState playerState;
    public int _maxHealth = 7;

    public bool isAlive
    {
        get
        {
            return health > 0;
        }

        set {}
    }



    public void SetState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Bleeding:
                animator.SetTrigger("WasShot");
                break;
            case PlayerState.Idle:
                animator.SetTrigger("RoundStarted");
                break;
            case PlayerState.Healing:
                animator.SetTrigger("WasHealed");
                break;
            case PlayerState.Dead:
                animator.SetTrigger("Died");
                break;
            case PlayerState.Shooting:
                animator.SetTrigger("AttemptShoot");
                break;
            case PlayerState.Dodge:
                animator.SetTrigger("AttemptDodge");
                break;
        }
    }



    // Use this for initialization
    void Start () 
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("RoundStarted");
    }



    // Update is called once per frame
    void Update () 
    {
        if(!gameController.amIAnimate)
            GetPlayerInput();
      
    }
    

    private void GetPlayerInput()
    {

        if (Input.GetButtonDown("Socket1"))
        {
            gameController.SetSelectedCard(0);
        }

        if (Input.GetButtonDown("Socket2"))
        {
            gameController.SetSelectedCard(1);
        }

        if (Input.GetButtonDown("Socket3"))
        {
            gameController.SetSelectedCard(2);
        }

        if (Input.GetButtonDown("Socket4"))
        {
            
            gameController.SetSelectedCard(3);
        }

        if (Input.GetButtonDown("Start") && gameController.serverState == GameState.Start)
        {
            gameController.Cmd_SetReady(true);
        }

        

            //Gamepad Shot
            if (Input.GetAxisRaw("Shot") != 0 || Input.GetButtonDown("Shot"))
            {
                if (m_ShotAxisPressed == false)
                {
                    Debug.Log("Gamepad");
                    gameController.Cmd_FinishRound();
                    m_ShotAxisPressed = true;
                }
            }

            if (Input.GetAxisRaw("Shot") == 0)
            {

                m_ShotAxisPressed = false;
            }
        

       
       
       

       
    }

}


