using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Networking;

public class SceneController : MonoBehaviour {

    private SceneStates currentState;
    private string defaultIP = "localhost";
    private int defaultPort = 5;

    public GameObject canvasUI;
    public GameObject tutorialSprite;
    public GameObject finishSprite;

    public GameObject StartButton;
    public GameObject SettingsButton;

    public enum SceneStates
    {
        Menu,
        Tutorial,
        Game,
        Finish
    }



    public void ShowMenu()
    {

    }


    public void ShowTutorial()
    {
        currentState = SceneStates.Tutorial;
        StartButton.SetActive(false);
        SettingsButton.SetActive(false);
        tutorialSprite.SetActive(true);
        canvasUI.SetActive(false);
    }

    public void StartGame()
    {

        currentState = SceneStates.Game;
        tutorialSprite.SetActive(false);
        string ipAddress = GameObject.Find("IPField").GetComponent<Text>().text;
        if (ipAddress == "")
            ipAddress = defaultIP;
        ConnectionManager connection = GameObject.Find("GameManager").GetComponent<ConnectionManager>();
        connection.networkAddress = ipAddress;
        connection.networkPort = defaultPort;
        if (GameObject.Find("IsHost").GetComponent<Toggle>().isOn)
            connection.StartHost();
        else
            connection.StartClient();
    }

    public void FinishGame()
    {
        finishSprite.SetActive(true);

    }

    // Use this for initialization
    void Start () {
        currentState = SceneStates.Menu;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
