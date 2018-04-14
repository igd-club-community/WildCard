using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    private SceneStates currentState;


    public string ip = "localhost";
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
