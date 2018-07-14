using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour {

	public float tempLatitude;
	public float tempLongitude;

	public LocationUpdater LocationUpdater_;
	
	// Use this for initialization
	void Start () {
		LocationUpdater_ = gameObject.GetComponent<LocationUpdater>();
	}
	
	// Update is called once per frame
	void Update () {
		tempLatitude = LocationUpdater_.Location.latitude;
		tempLongitude= LocationUpdater_.Location.longitude;
	}
}
