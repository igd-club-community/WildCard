using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackLineAnimation : MonoBehaviour {

	public GameObject upLine;
	public GameObject downLine;
	public float waitBetweenAnimation = 3f;
    public float waitForMovement = 4f;
    public float speed = 10f;
	
	public Vector3 startMarker;
	public Vector3 endMarker;

    private void Start()
    {
        upLine = GameObject.Find("blackLineUp");
        downLine = GameObject.Find("blackLineDown");
    }

	public IEnumerator Animate(bool close){
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startMarker, endMarker);
        float fracJourney = 0;
        while (fracJourney < 0.99) {
            float distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / journeyLength;
            if (close)
            {
                upLine.transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
                downLine.transform.position = Vector3.Lerp(-startMarker, -endMarker, fracJourney);
            }
            else
            {
                upLine.transform.position = Vector3.Lerp(endMarker, startMarker, fracJourney);
                downLine.transform.position = Vector3.Lerp(-endMarker, -startMarker, fracJourney);
            }
            yield return null;
        }
    }
}
