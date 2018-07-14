using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HistoryButton : MonoBehaviour {

	GameObject GPSButtonGO;
	GPSButton GPSButton_;

	private double longData;
	private double latData;

	void Start(){
		GPSButtonGO = GameObject.Find("GPSButton");
		GPSButton_ = GPSButtonGO.GetComponent<GPSButton>();
	}

	void Update(){}

	public void Onclick(){
		/* 保存データの取り出し */
		longData = double.Parse(PlayerPrefs.GetString("savedKeyLong", "0"));
		latData = double.Parse(PlayerPrefs.GetString("savedKeyLat", "0"));
		GPSButton_.SetSavedData(longData, latData);
	}
}
