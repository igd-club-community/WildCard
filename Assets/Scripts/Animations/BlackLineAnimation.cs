using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackLineAnimation : MonoBehaviour {

	public GameObject upLine;
	public GameObject downLine;
	public float waitBetweenAnimation = 1f;
    public float waitForMovement = 3f;
    public float speed = 10f;
	
	public Vector3 upLineStartPosition;
	public Vector3 upLineEndPosition;
	public Vector3 downLineStartPosition;
	public Vector3 downLineEndPosition;

	public BlackLine upLineMover;
	public BlackLine downLineMover;

    private void Start()
    {
        upLine = GameObject.Find("blackLineUp");
        downLine = GameObject.Find("blackLineDown");
        upLineMover = upLine.GetComponent<BlackLine>();
        downLineMover = downLine.GetComponent<BlackLine>();
    }

    private void OnEnable()
	{
		
	}

	public void doAnimation(){
		upLineMover.SetInternals(speed, waitBetweenAnimation, upLineStartPosition, upLineEndPosition);
		downLineMover.SetInternals(speed, waitBetweenAnimation, downLineStartPosition, downLineEndPosition);
		upLineMover.Enable();
        downLineMover.Enable();
    }
}
