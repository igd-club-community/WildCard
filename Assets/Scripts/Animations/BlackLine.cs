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
    public bool lineDown = false;
    public bool lineUp = true;

	private void OnEnable()
	{
		SetToStart();
	}

	private void Update()
	{
		if(Vector3.Distance(transform.position, endPosition) <= .1f)
		{
            lineDown = true;
            lineUp = false;
			StartCoroutine(ContinueAnimation());
			needHide = true;
		} 
		else 
		{
			distCovered = (Time.time - startTime) * speed;
        	fracJourney = distCovered / animationLength;
        	transform.position = Vector3.Lerp(startPositon, endPosition, fracJourney);
		}
        if (Vector3.Distance(transform.position, startPositon) <= .1f)
            lineUp = true;
	}

	private void SetToStart()
	{
		transform.position.Set(startPositon.x, startPositon.y, startPositon.z);
		startTime = Time.time;
		animationLength = Vector3.Distance(startPositon, endPosition);
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
		if(!needHide)
		{
			yield return new WaitForSeconds(animationPause);
			endPosition += startPositon - endPosition;
			startPositon.Set(transform.position.x, transform.position.y, transform.position.z);
			SetToStart();
		}
	}
	private void OnBecameInvisible()
	{
		if(needHide){
			this.gameObject.SetActive(false);
		}
	}
}
