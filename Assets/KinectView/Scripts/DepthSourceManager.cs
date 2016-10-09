using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Threading;
using System;

public class DepthSourceManager : MonoBehaviour
{   
    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data;
	private int count=0;
	private byte[] depthPixels;
	private ushort minDepth;
	private ushort maxDepth;
	private const int MapDepthToByte = 8000 / 256;
	private ushort[,] twoDArray;
	public GameObject audioLeft, audioMid, audioRight;
	private UnityEngine.AudioSource audioL, audioM, audioR;
    public ushort[] GetData()
    {
        return _Data;
    }

    void Start () 
    {
		audioL = audioLeft.GetComponent (typeof(UnityEngine.AudioSource)) as UnityEngine.AudioSource;
		audioM = audioMid.GetComponent (typeof(UnityEngine.AudioSource)) as UnityEngine.AudioSource;
		audioR = audioRight.GetComponent (typeof(UnityEngine.AudioSource)) as UnityEngine.AudioSource;
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null) 
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
			depthPixels = new byte[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
			//Debug.Log ("haha");
			minDepth = _Sensor.DepthFrameSource.DepthMinReliableDistance;
			maxDepth = ushort.MaxValue;
			int w = _Sensor.DepthFrameSource.FrameDescription.Width;
			int h = _Sensor.DepthFrameSource.FrameDescription.Height;
		//	Debug.Log (w);
	//		Debug.Log (h);
	//		Debug.Log (_Data.Length);


        }
    }
    

	void averageOut(int xMin,int xMax,int yMin,int yMax, ushort[,] twoDArray){
		ushort sum = 0;
		for (int i = xMin; i < xMax; i++) {
			for (int j = yMin; j < yMax; j++) {
				sum += twoDArray [i,j];
			}
		}
		ushort average = (ushort)(sum / ((xMax - xMin ) * (yMax - yMin )));
	//	Debug.Log (average);
		for (int i = xMin; i < xMax; i++) {
			for (int j = yMin; j < yMax; j++) {
				twoDArray [i,j] = average;
			}
		}

	}

	double[,] intensityCalc(ushort[][] twoDArray){
		double[,] result = new double[424,512];
		for (int x = 0; x < 424; x++){
			for (int y = 0; y < 512; y++) {
				int depth = twoDArray[x][y];
				int absx = Math.Abs (x - 212);
				int absy = Math.Abs (y - 256);
				double distance = 10 / (Math.Log ((depth / 10) * (depth / 10) + (absx * depth / 10000) * (absx * depth / 10000) + (absy * depth / 10000) * (absy * depth / 10000)));
				if (distance > 1) {
					distance = 1;
				}
				result[x,y] = distance;
			}
		}
		return result;
	}

	double intensityCalc_S(int x,int y,int depth){
		int absx = Math.Abs (x - 212);
		int absy = Math.Abs (y - 256);
		double distance = 10 / (Math.Log ((depth / 10) * (depth / 10) + (absx * depth / 10000) * (absx * depth / 10000) + (absy * depth / 10000) * (absy * depth / 10000)));
		if (distance > 1) {
			distance = 1;
		}
		return distance;
	}

    void Update () 
    {
        if (_Reader != null)
		{  
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);
				twoDArray = new ushort[424,512];
				for (int x = 0; x < 424; x++) {
					for (int y = 0; y < 512; y++) {
						twoDArray [x, y] = _Data [x * 512 + y];
					}
				}
					
				int j = 0;
				int jGap = 100;
				while (j < 512) {
					if (j > 380) {
						jGap = 512 - j ;
					}
					for (int a = 0; a < 424; a += 100) {
						if (a > 280) {
							averageOut (a, 424, j, j + jGap,twoDArray);
						} else {
							averageOut (a, a + 100, j, j + jGap,twoDArray);
						}
					}
					j = j + jGap;
				}
				//int aCount = 0;

				/*
				for (int z=50; z<512; z+=100){
					for (int i=50; i<424; i+=100){
						int vol=(int)twoDArray [i, z] / 2000;
						if (vol > 1)
							vol = 1;

						audioSource.volume = 1;
						audioSource.PlayOneShot (audioClip [aCount]);
						aCount++;
					}
					Thread.Sleep (1);
					aCount = 0;
				}
*/

				float vol=(float)(twoDArray [200, 150] / 5);
				float pitch = 0;
				if (vol > 1)
					vol = 1;
				//pitch = vol * 6 - 3;		
				
			//	if (vol < 0.5)
			//		vol = 0;
		//		Debug.Log (vol);
		//		Debug.Log ("left");
				//float vol = (float)intensityCalc_S(200,150,twoDArray[200,150]);
				//Debug.Log (vol);


				if (!audioL.isPlaying) {
					
					audioL.volume = vol;
					//audioL.pitch = pitch;
					audioL.Play ();
					audioL.time = 13.21f;
				}

				if (audioL.time > 14.20f) {
					audioL.Stop ();
				}


				vol=(float)(twoDArray [200, 300] / 5);
				if (vol > 1)
					vol = 1;
				//pitch = vol * 6 - 3;	
			//	if (vol < 0.5)
			//		vol = 0;
		//		Debug.Log (vol);
		//		Debug.Log ("mid");
				//vol = (float)intensityCalc_S(200,300,twoDArray[200,300]);
				//Debug.Log (vol);


				if (!audioM.isPlaying) {
					audioM.volume = vol;
					//audioL.pitch = pitch;
					audioM.Play ();
					audioM.time = 13.21f;
				}

				if (audioM.time > 14.20f) {
					audioM.Stop ();
				}

				vol=(float)(twoDArray [200, 450] / 5);
				if (vol > 1)
					vol = 1;
				//pitch = vol * 6 - 3;	
			//	if (vol < 0.5)
			//		vol = 0;
		//		Debug.Log (vol);
		//		Debug.Log ("right");
				//vol = (float)intensityCalc_S(200,450,twoDArray[200,450]);
				//Debug.Log (vol);


				if (!audioL.isPlaying) {
					audioR.volume = vol;
					//audioL.pitch = pitch;
					audioR.Play ();
					audioR.time = 13.21f;
				}

				if (audioR.time > 14.20f) {
					audioR.Stop ();
				}
				frame.Dispose();
				frame = null;

				//		if (i / 20 == 1) {
	//				i = 0;
                    //
					//for (int j = 0; j < _Data.Length; j++) {
				//		depthPixels [j] = (byte)(_Data[j] >= minDepth && _Data[j] <= maxDepth ? (_Data[j] / MapDepthToByte) : 0);
			//		}
			//		string[] temp = new string[_Data.Length];
			//		for (int j = 0; j < _Data.Length; j++) {
			//			temp [j] = _Data [j].ToString ();
			//		}
	//				System.IO.File.WriteAllLines (@"C:\Users\Administrator\Desktop\WriteLines.txt", temp);
		//		} else {
			//		i++;
		//		}
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
