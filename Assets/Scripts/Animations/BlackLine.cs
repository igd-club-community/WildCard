using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackLine : MonoBehaviour {
	private float speed = 1f;
	private float animationPause = 1f;
	private Vector3 startPositon;
	private Vector3 endPosition;
	private float startTime;
	private float animationLength;
	private float distCovered;
	private float fracJourney;
	private bool needHide = false;
    public bool active = false;
    public State state
    {
        get;
        private set;
    }

    public enum State
    {
        Moving, Down, Up
    }

	public void Enable()
	{
        SetToStart();
        active = true;
	}

	private void Update()
	{
        if (!active)
            return;
		if(Mathf.Abs(Vector3.Distance(transform.position, endPosition)) <= .1f)
		{
            StartCoroutine(ContinueAnimation());
		} 
		else 
		{
			distCovered = (Time.time - startTime) * speed;
        	fracJourney = distCovered / animationLength;
        	transform.position = Vector3.Lerp(startPositon, endPosition, fracJourney);
		}
	}

	private void SetToStart()
	{
		transform.position.Set(startPositon.x, startPositon.y, startPositon.z);
		startTime = Time.time;
		animationLength = Vector3.Distance(startPositon, endPosition);
        state = State.Moving;
	}

	public void SetInternals(float speed, float animationPause,Vector3 startPositon, Vector3 endPosition)
	{
		this.speed = speed;
		this.animationPause = animationPause;
		this.startPositon = startPositon;
		this.endPosition = endPosition;
	}

	private IEnumerator ContinueAnimation()
	{
        if (!needHide)
        {
            state = State.Down;
            yield return new WaitForSeconds(animationPause);
            state = State.Moving;
            endPosition += startPositon - endPosition;
            startPositon.Set(transform.position.x, transform.position.y, transform.position.z);
            SetToStart();
            needHide = true;
        }
        else
        {
            active = false;
            state = State.Up;
        }
    }
}
