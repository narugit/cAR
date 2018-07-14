using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

	double lastCompassUpdateTime = 0;
    Quaternion correction = Quaternion.identity;
    Quaternion targetCorrection = Quaternion.identity;

	// Androidの場合はScreen.orientationに応じてrawVectorの軸を変換
    static Vector3 compassRawVector
    {
        get
        {
            Vector3 ret = Input.compass.rawVector;
            
            if(Application.platform == RuntimePlatform.Android)
            {
                switch(Screen.orientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                        ret = new Vector3(-ret.y, ret.x, ret.z);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        ret = new Vector3(ret.y, -ret.x, ret.z);
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        ret = new Vector3(-ret.x, -ret.y, ret.z);
                        break;
                }
            }
            
            return ret;
        }
    }
	
	// Quaternionの各要素がNaNもしくはInfinityかどうかチェック
    static bool isNaN(Quaternion q)
    {
        bool ret = 
            float.IsNaN(q.x) || float.IsNaN(q.y) || 
            float.IsNaN(q.z) || float.IsNaN(q.w) || 
            float.IsInfinity(q.x) || float.IsInfinity(q.y) || 
            float.IsInfinity(q.z) || float.IsInfinity(q.w);
        
        return ret;
    }

	static Quaternion changeAxis(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, q.z, q.w);
    }

	void Start () 
    {
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
    }
	
	void Update () 
    {
		Quaternion gorientation = changeAxis(Input.gyro.attitude);

		if (Input.compass.timestamp > lastCompassUpdateTime)
		{
			lastCompassUpdateTime = Input.compass.timestamp;

			Vector3 gravity = Input.gyro.gravity.normalized;
			Vector3 rawvector = compassRawVector;
			Vector3 flatnorth = rawvector - 
				Vector3.Dot(gravity, rawvector) * gravity;

			Quaternion corientation = changeAxis(
				Quaternion.Inverse(
					Quaternion.LookRotation(flatnorth, -gravity)));

			// +zを北にするためQuaternion.Euler(0,0,180)を入れる。
			Quaternion tcorrection = corientation * 
				Quaternion.Inverse(gorientation) * 
				Quaternion.Euler(0, 0, 180);

			// 計算結果が異常値になったらエラー
			// そうでない場合のみtargetCorrectionを更新する。
			if(!isNaN(tcorrection))
				targetCorrection = tcorrection;
		}

		if (Quaternion.Angle(correction, targetCorrection) < 45)
		{
			correction = Quaternion.Slerp(
				correction, targetCorrection, 0.02f);
		}
		else 
			correction = targetCorrection;

		transform.localRotation = correction * gorientation;

		
    }

			
}
