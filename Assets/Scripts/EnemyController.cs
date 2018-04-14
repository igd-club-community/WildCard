using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    

    

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



    public void SetState(ServerBehaviour.PlayerState state)
    {
        switch (state)
        {
            case ServerBehaviour.PlayerState.Damaged:
                animator.SetTrigger("Damaged");
                break;
            case ServerBehaviour.PlayerState.NoDamaged:
                animator.SetTrigger("NoDamaged");
                break;
            case ServerBehaviour.PlayerState.Healed:
                animator.SetTrigger("Healed");
                break;
            case ServerBehaviour.PlayerState.Dead:
                animator.SetTrigger("Dead");
                break;
        }
    }


    // Use this for initialization
    void Start()
    {

        animator = GetComponent<Animator>();
        animator.SetTrigger("NoDamaged");

    }




    // Update is called once per frame
    void Update()
    {
       
    }


   




}
