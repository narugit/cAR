using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GPSButton : MonoBehaviour {
	GameObject GPSGO;
	GPS GPS_;
	
	/* ボタン押下後非表示にするオブジェクト */
	GameObject compassGO;

	GameObject textSavedGO;
	GameObject textNowGO;

	GameObject arrowRotGO;
	GameObject arrowGO;
	private float rotationAngle = 0.0f; //矢印の角度
	private float scaleSize = 0.0f;	//矢印のサイズ
	private float a = 0.3303f;
	private float b = 19.6697f;

	private float debugGain = 1.0f; 

	private double longNow;	//現在の経度
	private double latNow;	//現在の緯度
	private double longParking;	//駐車位置の経度
	private double latParking;	//駐車位置の緯度
	private int countBtn = 0;	//ボタン押した判定
	private double r = 6378.137;
	private double distance;	//現在位置と駐車位置間の距離
	private double delta;
	private double phi;	//現在位置から見た駐車位置の方位（北：0° 東：90°　・・・）

	void Awake(){
		arrowGO = GameObject.Find("arrow");
		arrowGO.SetActive(false);

		textSavedGO = GameObject.Find("DebugTextSaved");
		textSavedGO.SetActive(false);
		textNowGO = GameObject.Find("DebugTextNow");
		textNowGO.SetActive(false);
	}

	void Start(){
		GPSGO = GameObject.Find("GPSObject");
		GPS_ = GPSGO.GetComponent<GPS>();

		arrowRotGO = GameObject.Find("VirtualOrigin");
		
		compassGO = GameObject.Find("Compass");
	}

	public void OnClick() {
		longParking = double.Parse((GPS_.tempLongitude).ToString()) * Math.PI / 180;
		latParking = double.Parse((GPS_.tempLatitude).ToString()) * Math.PI / 180;

		arrowGO.SetActive(true);
		compassGO.SetActive(false);
		textSavedGO.SetActive(true);
		textNowGO.SetActive(true);

		countBtn++;

		/* データの保存 */
		PlayerPrefs.SetString("savedKeyLong", longParking.ToString());
		PlayerPrefs.SetString("savedKeyLat", latParking.ToString());

  	}

	public void SetSavedData(double savedDataLong, double savedDataLat){
		longParking = savedDataLong;
		latParking = savedDataLat;

		arrowGO.SetActive(true);
		compassGO.SetActive(false);
		textSavedGO.SetActive(true);
		textNowGO.SetActive(true);

		countBtn++;
	}

	void Update(){
		if(countBtn > 0){
			//switch(countBtn % 2){
				//case 1:
					//debugGain = 1.0f;

					/* デバッグ用表示 */
					GameObject.Find("DebugTextSaved").GetComponent<Text>().text
						= "保存した位置情報\n" +  
						"long: " + longParking * 180 / Math.PI + "°\n" + 
						"lat: " + latParking * 180 / Math.PI + "°\n";

					GameObject.Find("DebugTextNow").GetComponent<Text>().text 
						= "現在の位置情報\n" +  
						"long: " + longNow * 180 / Math.PI + "°\n" + 
						"lat: " + latNow * 180 / Math.PI+ "°\n" + 
						"距離：" + Math.Round(distance, 2, MidpointRounding.AwayFromZero) + "m\n" + 
						"北からの角度：" + Math.Round(phi, 2, MidpointRounding.AwayFromZero) + "°";
					//break;
				
				//case 0:
				//	debugGain = 0.1f;

					/* デバッグ用表示 */
					/*GameObject.Find("DebugTextSaved").GetComponent<Text>().text 
						= "デバッグモード\n" + "保存した位置情報\n" +  
						"long: " + longParking * 180 / Math.PI + "°\n" + 
						"lat: " + latParking * 180 / Math.PI + "°\n";

					GameObject.Find("DebugTextNow").GetComponent<Text>().text 
						= "現在の位置情報\n" +  
						"long: " + longNow * 180 / Math.PI + "°\n" + 
						"lat: " + latNow * 180 / Math.PI+ "°\n" + 
						"距離：" + Math.Round(distance, 2, MidpointRounding.AwayFromZero) + "m\n" + 
						"北からの角度：" + Math.Round(phi, 2, MidpointRounding.AwayFromZero) + "°";*/
					//break;
			//}

			longNow = double.Parse((GPS_.tempLongitude).ToString()) * Math.PI /180;
			latNow = double.Parse((GPS_.tempLatitude).ToString()) * Math.PI /180;

			/* 角度・距離の計算 */
			/* 計算参考サイト
				角度：http://gbrid.mobi/pc/gbrid/etc/14.html
				距離：http://keisan.casio.jp/exec/system/1257670779
				実行結果確認：http://keisan.casio.jp/exec/system/1257670779
			*/
			delta = longParking - longNow;
			distance = r * Math.Acos( Math.Sin(latNow) * Math.Sin(latParking) + Math.Cos(latNow) * Math.Cos(latParking) * Math.Cos(delta) ) * 1000;

			phi = Math.Atan2( Math.Cos(latParking) * Math.Sin(delta), 
							Math.Cos(latNow) * Math.Sin(latParking) - Math.Sin(latNow) * Math.Cos(latParking) * Math.Cos(delta) );

			phi = ((phi * 180 / Math.PI) + 360 ) % 360;
			
			/* 矢印の角度変更 */
			rotationAngle = (float)phi;
			arrowRotGO.transform.rotation = Quaternion.Euler(0.0f, rotationAngle, 0.0f);	

			/* 矢印のサイズ変更 */
			scaleSize = (float)distance;
			scaleSize = scaleSize * a / debugGain + b;
			
			if( scaleSize < 20 ){
				scaleSize = 0.0f;
			}

			else if( scaleSize > 350 ){
				scaleSize = 350.0f;
			}

			arrowGO.transform.localScale = new Vector3( scaleSize, scaleSize, scaleSize);	

			/* 矢印の色変更 */
			if( distance <= (1000 * debugGain) ){
				arrowGO.GetComponent<Renderer>().material.color = new Color(0f/255f, 90f/255f, 200f/255f, 255f/255f);
			}

			else if( distance > (1000 * debugGain) && distance <= (5000 * debugGain) ){
				arrowGO.GetComponent<Renderer>().material.color = new Color(200f/255f, 150f/255f, 0f/255f, 255f/255f);
			}

			else if( distance > (5000 * debugGain) ){
				arrowGO.GetComponent<Renderer>().material.color = new Color(255f/255f, 0f/255f, 0f/255f, 255f/255f);
			}
		} 
	}
}
