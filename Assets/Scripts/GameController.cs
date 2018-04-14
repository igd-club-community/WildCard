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

   


    public SceneController sceneController;


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
    public int Health;

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

            player.Health = player0Health;
            enemy.Health = player1Health;
        }
        else
        {
            player.Health = player1Health;
            enemy.Health = player0Health;
        }

    }

    [ClientRpc]
    public void Rpc_Animate(ServerBehaviour.PlayerState player0State, ServerBehaviour.PlayerState player1State)
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
        cardDesk = new CardDesk();
        AvailableCards = new int[4];
        sceneController.StartGame();
        if (ID == 0)
        {
            player = GameObject.Instantiate(bluePlayerPrefab, new Vector3(-3.05f, -0.68f, 0), Quaternion.identity).GetComponent<PlayerController>();
            enemy = GameObject.Instantiate(redEnemyPrefab, new Vector3(3.66f, -0.68f, 0), Quaternion.Euler(0, -180, 0)).GetComponent<EnemyController>();
        }
        else
        {
            player = GameObject.Instantiate(redPlayerPrefab, new Vector3(3.66f, -0.68f, 0), Quaternion.Euler(0, -180, 0)).GetComponent<PlayerController>();
            enemy = GameObject.Instantiate(BlueEnemyPrefab, new Vector3(-3.05f, -0.68f, 0), Quaternion.identity).GetComponent<EnemyController>();
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
                    player.SetState(ServerBehaviour.PlayerState.NoDamaged);
                    enemy.SetState(ServerBehaviour.PlayerState.Dead);
                }
                else
                {
                    enemy.SetState(ServerBehaviour.PlayerState.NoDamaged);
                    player.SetState(ServerBehaviour.PlayerState.Dead);
                }
                break;
            case 1:
                if (ID == 1)
                {
                    player.SetState(ServerBehaviour.PlayerState.NoDamaged);
                    enemy.SetState(ServerBehaviour.PlayerState.Dead);
                }
                else
                {
                    enemy.SetState(ServerBehaviour.PlayerState.NoDamaged);
                    player.SetState(ServerBehaviour.PlayerState.Dead);
                }
                break;
            case -1:
                player.SetState(ServerBehaviour.PlayerState.Dead);
                enemy.SetState(ServerBehaviour.PlayerState.Dead);
                break;
        }
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
