using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {

    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;
    public GameObject redEnemyPrefab;
    public GameObject BlueEnemyPrefab;
    public CardDesk cardDesk;
    public int[] AvailableCards;
    public GameObject[] cardSockets;

    

    private PlayerController player;
    private EnemyController  enemy;

    //for server interaction:

    [SyncVar]
    public int ID;

    [SyncVar]
    public bool didFire = false;

    [SyncVar]
    public SyncListInt SelectedCards = new SyncListInt();

    [SyncVar]
    public SyncListInt EnemyCards = new SyncListInt();

    [SyncVar]
    public bool ready;

    [SyncVar]
    public int health;

    [SyncVar]
    public int _maxHealth;

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
    public void Rpc_ChangeHealth(int player0Health, int player1Health)
    {
        if (ID == 0)
        {

            player.health = player0Health;
            enemy.health = player1Health;
        }
        else
        {
            player.health = player1Health;
            enemy.health = player0Health;
        }

    }

    [ClientRpc]
    public void Rpc_Animate(PlayerState player0State, PlayerState player1State)
    {
        if(ID == 0)
        {
            
            player.SetState(player0State);
            enemy.SetState(player1State);
        }
        else
        {
            player.SetState(player1State);
            enemy.SetState(player0State);
        }

        

    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            cardSockets = new GameObject[4];
            cardDesk = GameObject.Find("CardDesk").GetComponent<CardDesk>();
            for (int i = 0; i < 4; i++)
            {

                cardSockets[i] = GameObject.Find("CardSockets").transform.GetChild(i).gameObject;
            }
            Cmd_InitSelectedCards();
            AvailableCards = new int[4];

            if (ID == 0)
            {
                player = GameObject.Instantiate(bluePlayerPrefab, new Vector3(-3.05f, -0.68f, 0), Quaternion.identity).GetComponent<PlayerController>();
                enemy = GameObject.Instantiate(redEnemyPrefab, new Vector3(3.66f, -0.68f, 0), Quaternion.Euler(0, -180, 0)).GetComponent<EnemyController>();
                player.gameController = this;
            }
            else
            {
                player = GameObject.Instantiate(redPlayerPrefab, new Vector3(3.66f, -0.68f, 0), Quaternion.Euler(0, -180, 0)).GetComponent<PlayerController>();
                enemy = GameObject.Instantiate(BlueEnemyPrefab, new Vector3(-3.05f, -0.68f, 0), Quaternion.identity).GetComponent<EnemyController>();
                player.gameController = this;
            }
        }
    }

    private void Update()
    {

       
    }


    [ClientRpc]
    public void Rpc_Finish(int winner)
    {

        switch (winner)
        {
            case 0:
                if (ID == 0)
                {
                    player.SetState(PlayerState.Idle);
                    enemy.SetState(PlayerState.Dead);
                }
                else
                {
                    enemy.SetState(PlayerState.Idle);
                    player.SetState(PlayerState.Dead);
                }
                break;
            case 1:
                if (ID == 1)
                {
                    player.SetState(PlayerState.Idle);
                    enemy.SetState(PlayerState.Dead);
                }
                else
                {
                    enemy.SetState(PlayerState.Idle);
                    player.SetState(PlayerState.Dead);
                }
                break;
            case -1:
                player.SetState(PlayerState.Dead);
                enemy.SetState(PlayerState.Dead);
                break;
        }
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
            cardSockets[index].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[index]]._SelectedImage;
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
