    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SceneController : MonoBehaviour {

    private SceneStates currentState;
    private string defaultIP = "localhost";
    private int defaultPort = 1337;

    public GameObject canvasUI;
    public GameObject tutorialSprite;
    public GameObject finishSprite;

    public GameObject StartButton;
    public GameObject SettingsButton;

    public Text ipAddressField;
    public Toggle isHost;

    public GameObject cardSockets;
    

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
        cardSockets.SetActive(true);
        currentState = SceneStates.Game;
        tutorialSprite.SetActive(false);
        string ipAddress = ipAddressField.text;
        if (ipAddress == "")
            ipAddress = defaultIP;
        ConnectionManager connection = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        connection.networkAddress = ipAddress;
        connection.networkPort = defaultPort;
        if (isHost.GetComponent<Toggle>().isOn)
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

        if (Input.GetButtonDown("Start") && currentState == SceneStates.Tutorial)
        {
            StartGame();
            Debug.Log("Start");
        }

        if (Input.GetButtonDown("Start") && currentState == SceneStates.Menu)
        {
            ShowTutorial();
            Debug.Log("Start");
        }

    }
}
