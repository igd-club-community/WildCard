using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour {


    public GameController gameController;

    private bool m_ShotAxis = false;

    private Animator animator;

    public int Health;

    public enum PlayerState
    {
        NoDamaged,
        Shooting,
    }

    public PlayerState playerState;
    public int _maxHealth = 10;

    public bool IsAlive
    {
        get
        {
            return Health > 0;
        }

        set
        {

        }
    }
    // Use this for initialization
    void Start () {
                
        animator = GetComponent<Animator>();
        animator.SetTrigger("NoDamaged");

    }




    // Update is called once per frame
    void Update () {
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

        //Gamepad Shot
        if (Input.GetAxisRaw("Shot") != 0)
        {
            if (m_ShotAxis == false)
            {
                
                
                Debug.Log("RT");


                m_ShotAxis = true;
            }
        }

        if (Input.GetAxisRaw("Shot") == 0)
        {
            m_ShotAxis = false;
        }

        if (Input.GetButtonDown("Start"))
        {
            Debug.Log("Start");
        }
    }

   

   

}


