using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iPhoneCamera : MonoBehaviour {

	// Cam
	private WebCamTexture cam;
	public RawImage background;
	public AspectRatioFitter fit;

	private bool arReady = false;

	private void Start () {
		
		for(int i = 0; i < WebCamTexture.devices.Length; i++){
			if(!WebCamTexture.devices[i].isFrontFacing){
				cam = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height);
				break;
			}
		}

		if(cam == null){
			Debug.Log("unable to find back camera");
			return;
		}

		cam.Play();
		background.texture = cam;

		arReady = true;
	}

	
	private void Update () {
		if(arReady){
			//Update Camera
			float ratio = (float)cam.width / (float)cam.height;
			fit.aspectRatio = ratio;
			
			float scaleY = cam.videoVerticallyMirrored ? -1f: 1f;
			background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

			int orient = -cam.videoRotationAngle;
			background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
		}
	}
}
