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
    public GameObject[] enemyCardSockets;

    public Sprite[] hpBarSprites;
    
    public GameObject leftHPBar;
    public GameObject rightHPBar;




    public GameObject enemyCardSocketsObject;

    public const float preRoundTime = 3;
    public GameObject preRoundTimer;
    public const float charactersAnimationTime = 5;

    private PlayerController player;
    private EnemyController enemy;

    public Card emptyCard;

    //for server interaction:

    [SyncVar]
    public int ID;

    [SyncVar]
    public bool didFire = false;

    [SyncVar]
    public SyncListInt SelectedCards = new SyncListInt();

    [SyncVar]
    public SyncListInt EnemySelectedCards = new SyncListInt();

    [SyncVar]
    public bool ready;

    [SyncVar]
    public int health;

    [SyncVar]
    public int enemyHealth;

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


    IEnumerator StartRoundWithDelay(float time)
    {
        yield return new WaitForSeconds(time);

        preRoundTimer.SetActive(false);

        Debug.Log("Shuffle cards");
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

    [ClientRpc]
    public void Rpc_StartRound(int roundNumber)
    {

        if (isLocalPlayer)
        {
            Cmd_SetReady(false); //#TODO check is it need or not
            preRoundTimer.SetActive(true);
            preRoundTimer.GetComponent<Animator>().SetTrigger("StartTimer");
            StartCoroutine(StartRoundWithDelay(preRoundTime));
           

        }
    }


    [ClientRpc]
    public void Rpc_ChangeHealth(int player0Health, int player1Health)
    {
        if (isLocalPlayer)
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
    }




    [ClientRpc]
    public void Rpc_Animate(PlayerState player0State, PlayerState player1State, int shooterID)
    {
        if (isLocalPlayer)
            StartCoroutine(Animate(player0State, player1State, shooterID));

    }

    private IEnumerator Animate(PlayerState player0State, PlayerState player1State, int shooterID)
    {

        yield return AnimatePlayedCards();
        yield return AnimateShoot(shooterID);
        if (ID == 0)
        {
            leftHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[health];
            rightHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[enemyHealth];
        }
        else
        {
            leftHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[enemyHealth];
            rightHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[health];
        }
        yield return AnimateCharacters(player0State, player1State);
        Cmd_SetReady(true); // I hope it would work
    }

    private IEnumerator AnimateShoot(int shooterID)
    {
        
        if (ID == shooterID)
        {
            player.SetState(PlayerState.Shooting);
            enemy.SetState(PlayerState.Dodge);
        }
        else if(ID == -1)
        {
            player.SetState(PlayerState.Shooting);
            enemy.SetState(PlayerState.Shooting);
        }
        else
        {
            player.SetState(PlayerState.Dodge);
            enemy.SetState(PlayerState.Shooting);
        }
        yield return new WaitForSeconds(2);

    }

    private IEnumerator AnimatePlayedCards()
    {
        BlackLineAnimation lineAnimations = GetComponent<BlackLineAnimation>();
        yield return lineAnimations.Animate(true);
        enemyCardSocketsObject.SetActive(true);
        for (int i=0; i<4; i++)
        {
            //for enemy
            enemyCardSockets[i].GetComponent<SpriteRenderer>().enabled = true;

            if (EnemySelectedCards[i] != -1)
                enemyCardSockets[i].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[EnemySelectedCards[i]]._SelectedImage;
            else
                enemyCardSockets[i].GetComponent<SpriteRenderer>().sprite = emptyCard._SelectedImage;

            cardSockets[i].GetComponent<SpriteRenderer>().enabled = true;
            //for player
            if (SelectedCards[i] != -1)
                cardSockets[i].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[i]]._SelectedImage;
            else
                cardSockets[i].GetComponent<SpriteRenderer>().sprite = emptyCard._SelectedImage;
        }


        
        yield return new WaitForSeconds(lineAnimations.waitBetweenAnimation);
        enemyCardSocketsObject.SetActive(false);
        //for (int i = 0; i < 4; i++)
        //{
        //    enemyCardSockets[i].SetActive(false);
        //}
        yield return lineAnimations.Animate(false);
    }

    private IEnumerator AnimateCharacters(PlayerState player0State, PlayerState player1State)
    {
        if (ID == 0)
        {

            player.SetState(player0State);
            enemy.SetState(player1State);
        }
        else
        {
            player.SetState(player1State);
            enemy.SetState(player0State);
        }
        yield return new WaitForSeconds(charactersAnimationTime);

    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            leftHPBar = GameObject.Find("leftHPBar");
            rightHPBar = GameObject.Find("rightHPBar");
            emptyCard = Resources.Load<Card>("EmptyCard");
            preRoundTimer = GameObject.Find("preRoundTimer");
            cardSockets = new GameObject[4];
            enemyCardSockets = new GameObject[4];
            cardDesk = GameObject.Find("CardDesk").GetComponent<CardDesk>();
            for (int i = 0; i < 4; i++)
            {
                enemyCardSockets[i] = GameObject.Find("EnemyCardSockets").transform.GetChild(i).gameObject;
                cardSockets[i] = GameObject.Find("CardSockets").transform.GetChild(i).gameObject;
            }
            enemyCardSocketsObject = GameObject.Find("EnemyCardSockets");
            enemyCardSocketsObject.SetActive(false);
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
            Cmd_SetReady(true);
        }
    }

    private void Update()
    {

       
    }


    [ClientRpc]
    public void Rpc_Finish(int winner)
    {
        if (isLocalPlayer)
        {
            switch (winner)
            {
                case 0:
                    if (ID == 0)
                    {
                        player.SetState(PlayerState.Idle);
                        enemy.SetState(PlayerState.Dead);
                        rightHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    }
                    else
                    {
                        enemy.SetState(PlayerState.Idle);
                        player.SetState(PlayerState.Dead);
                        leftHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    }
                    break;
                case 1:
                    if (ID == 1)
                    {
                        player.SetState(PlayerState.Idle);
                        enemy.SetState(PlayerState.Dead);
                        leftHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    }
                    else
                    {
                        enemy.SetState(PlayerState.Idle);
                        player.SetState(PlayerState.Dead);
                        rightHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    }
                    break;
                case -1:
                    player.SetState(PlayerState.Dead);
                    enemy.SetState(PlayerState.Dead);
                    rightHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    leftHPBar.GetComponent<SpriteRenderer>().sprite = hpBarSprites[0];
                    break;
            }
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
            if(SelectedCards[index] == value)
                cardSockets[index].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[value]._SelectedImage;
            
        }
        else
        {
            SelectedCards[index] = -1;
            Cmd_SetSelectedCard(index, -1);
            if (SelectedCards[index] == -1)
                cardSockets[index].GetComponent<SpriteRenderer>().sprite = cardDesk.cardDesk[AvailableCards[index]]._NotSelectedImage;
        }

    }

    [Command]
    public void Cmd_SetSelectedCard(int index, int value)
    {
        if (GameObject.Find("GameServer").GetComponent<ServerBehaviour>().state == ServerBehaviour.State.Round)
        {
            SelectedCards[index] = value;
        }
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
        {
            didFire = true;
            server.FinishRound(ID);
            
        }
    }





}
