using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {


    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;
    public CardDesk cardDesk;
    public int[] AvailableCards;
    public GameObject[] cardSockets;

    public int playerID;

    public string ip = "localhost";
    public GameObject canvasUI;


    public GameStates currentState;
    public GameObject tutorialSprite;



    private PlayerController player;


    //for server interaction:
    [SyncVar]
    public bool didFire = false;

    [SyncVar]
    public SyncListInt SelectedCards = new SyncListInt();

    [SyncVar]
    public SyncListInt EnemyCards = new SyncListInt();

    [SyncVar]
    public bool ready;

    [SyncVar]
    public int Health;

    public enum GameStates
    {
        Menu,
        Settings,
        Tutorial,
        Connecting,
        StartWaiting,
        Round,
        Animation,
        End
    }


    public void ShowTutorial()
    {
        currentState = GameStates.Tutorial;
        tutorialSprite.SetActive(true);
        canvasUI.SetActive(false);
    }

    public void ConnectPlayer()
    {
        tutorialSprite.SetActive(false);
        if (playerID == 0)
        {
            player = GameObject.Instantiate(bluePlayerPrefab, new Vector3(-3.05f, -0.68f, 0), Quaternion.identity).GetComponent<PlayerController>();
            
        }
        else
        {
            player = GameObject.Instantiate(redPlayerPrefab, new Vector3(3.66f, -0.68f, 0), Quaternion.Euler(0, -180, 0)).GetComponent<PlayerController>();
        }

    }

    
   


    [ClientRpc]
    public void Rpc_StartRound()
    {
        if (isLocalPlayer)
        {
            //Cmd_SetReady(false);

            for (int i = 0; i < 4; i++)
            {

               
                int temp = Random.Range(0, (int)cardDesk.totalSum);
                int index = 0;
                uint sum = cardDesk.cardDesk[index]._chanceCoefficient;

                while (sum < temp)
                {
                    index++;
                    sum += cardDesk.cardDesk[index]._chanceCoefficient;
                }
                AvailableCards[i] = index;
            }
            Cmd_InitSelectedCards();
            for (int i = 0; i < 4; i++)
            {
                
                cardSockets[i].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[i]]._NotSelectedImage;
            }

            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
    }


    [ClientRpc]
    public void Rpc_Animate(ServerBehaviour.PlayerState roundResult)
    {

        //Cmd_SetReady(false);
        switch (roundResult)
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
        }

    }

    private void Start()
    {
        cardDesk = new CardDesk();
        AvailableCards = new int[4];

    }

    private void Update()
    {

        if (Input.GetButtonDown("Start"))
        {
            if (currentState == GameStates.Tutorial)
            {
                currentState = GameStates.Connecting;
                ConnectPlayer();
            }
            else if (currentState == GameStates.Connecting)
            {
                currentState = GameStates.StartWaiting;

            }
        }
    }


    [ClientRpc]
    public void Rpc_Finish()
    {
        if (!IsAlive)
            animator.SetTrigger("Dead");
        else
            animator.SetTrigger("NoDamaged");
    }

    public int ID
    {
        get;
        set;
    }

    [Command]
    private void Cmd_InitSelectedCards()
    {
        SelectedCards.Clear();
        for (int i = 0; i < 4; i++)
            SelectedCards.Add(-1);
    }


    public void SetSelectedCard(int index)
    {
        if (SelectedCards[index] == -1)
        {
            int value = AvailableCards[index];
            Cmd_SetSelectedCard(index, value);
            cardSockets[index].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[index]]._NotSelectedImage;
        }
        else
        {
            SelectedCards[index] = -1;
            Cmd_SetSelectedCard(index, -1);
            cardSockets[index].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[index]]._NotSelectedImage;
        }

    }

    [Command]
    public void Cmd_SetSelectedCard(int index, int value)
    {
       
        SelectedCards[index] = value;
    }

    [Command]
    public void Cmd_SetReady(bool status)
    {
        ready = status;

    }

    [Command]
    public void Cmd_FinishRound()
    {
        ServerBehaviour server = GameObject.Find("GameServer").GetComponent<ServerBehaviour>();
        if (server.state == ServerBehaviour.State.Round)
            didFire = true;
        server.FinishRound();
        Cmd_InitSelectedCards();
    }





}
