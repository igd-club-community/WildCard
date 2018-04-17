using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerBehaviour : NetworkBehaviour
{
    public const double roundTime = 10;
    public double endTime;
    public State state = State.Connecting;

    private struct PlayerActionStruct
    {
        public int damage;
        public int heal;
        public int evade;
    }
    
    public enum State
    {
         Connecting, Start, Round, Animation, Finish
    }

    public List<GameObject> players = new List<GameObject>();

    // Use this for initialization
    void Start () {
		
	}

    public void StartRound()
    {
        endTime = Time.fixedTime + roundTime;
        foreach(GameObject player in players){
            player.GetComponent<GameController>().ready = false;
            player.GetComponent<GameController>().didFire = false;
            player.GetComponent<GameController>().Rpc_StartRound();
        }
        state = State.Round;
    }

    public void Animate(PlayerState[] nextStates)
    {
        for(int i = 0; i < players.Count; i++)
        {
            GameController player = players[i].GetComponent<GameController>();
            player.ready = false;
            player.EnemyCards.Clear();
            foreach(int card in players[1 - i].GetComponent<GameController>().SelectedCards){
                player.EnemyCards.Add(card);
            }
            player.Rpc_Animate(nextStates[0], nextStates[1]);
        }
        state = State.Animation;
    }

    public PlayerState[] ApplyActions()
    {
        PlayerActionStruct[] playerActions = new PlayerActionStruct[2];
        for (int i = 0; i < players[0].GetComponent<GameController>().SelectedCards.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int cdIndex = players[j].GetComponent<GameController>().SelectedCards[i];
                if (cdIndex < 0)
                    continue;
                Card card = GameObject.Find("GameController").GetComponent<CardDesk>().cardDesk[cdIndex];
                switch (card._ActionType)
                {
                    case Action.DamageBoth:
                        playerActions[j].damage += (int)card._value;
                        playerActions[1 - j].damage += (int)card._value;
                        break;
                    case Action.DamageEnemy:
                        playerActions[1 - j].damage += (int)card._value;
                        break;
                    case Action.DamageSelf:
                        playerActions[j].damage += (int)card._value;
                        break;
                    case Action.Evade:
                        playerActions[j].evade += (int)card._value;
                        break;
                    case Action.Heal:
                        playerActions[j].heal += (int)card._value;
                        break;
                }
            }
        }
        PlayerState[] playerStates = new PlayerState[2];
        for (int j = 0; j < 2; j++)
        {
            int playerHP = players[j].GetComponent<GameController>().health -
            Mathf.Max(0, playerActions[j].damage - playerActions[j].evade) +
            playerActions[j].heal;
            playerHP = Mathf.Min(players[j].GetComponent<GameController>()._maxHealth, playerHP);
            if (playerHP > players[j].GetComponent<GameController>().health)
                playerStates[j] = PlayerState.Healing;
            else if (playerHP < players[j].GetComponent<GameController>().health)
                playerStates[j] = PlayerState.Bleeding;
            else
                playerStates[j] = PlayerState.Idle;
            players[j].GetComponent<GameController>().health = playerHP;
        }
        return playerStates;
    }

    public void FinishRound()
    {
        if (state.Equals(State.Round))
        {
            PlayerState[] nextStates = ApplyActions();
            if (players[0].GetComponent<GameController>().health>0 && players[1].GetComponent<GameController>().health>0)
                Animate(nextStates);
            else
                state = State.Finish;
        }
    }



    bool isFinished = false;
    // Update is called once per frame
    void Update () {
        if (isServer)
        {
            if (state.Equals(State.Start) || state.Equals(State.Animation))
            {
                bool pl2ready = players[1].GetComponent<GameController>().ready;
                bool pl1ready = players[0].GetComponent<GameController>().ready;
                if (pl1ready && pl2ready)
                {
                    Debug.Log("Both true");
                    StartRound();
                }
            }
            else if (state.Equals(State.Round))
            {
                if (endTime < Time.fixedTime)
                {
                    FinishRound();
                }
            }
            else if (state.Equals(State.Finish) && !isFinished)
            {
                isFinished = true;
                int winner = -1;
                if (players[0].GetComponent<GameController>().health > 0)
                    winner = 0;
                if (players[1].GetComponent<GameController>().health > 0)
                    winner = 1;
                foreach (GameObject player in players)
                {
                    player.GetComponent<GameController>().Rpc_Finish(winner);
                    player.GetComponent<GameController>().ready = false;
                }
                state = State.Start;
            }
        }
	}
}
