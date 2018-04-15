using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Animator animator;

    public int health;

    public PlayerState playerState;
    public int _maxHealth = 10;

    public bool IsAlive
    {
        get
        {
            return health > 0;
        }
        
        set{}
    }



    public void SetState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Bleeding:
                animator.SetTrigger("WasShot");
                break;
            case PlayerState.Idle:
                animator.SetTrigger("Stalled");
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
