using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Threading;

public class DepthSourceManager : MonoBehaviour
{   
    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data;
	private int i=0;
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
		ushort average = (ushort)(sum / ((xMax - xMin + 1) * (yMax - yMin + 1)));
		for (int i = xMin; i < xMax; i++) {
			for (int j = yMin; j < yMax; j++) {
				twoDArray [i,j] = average;
			}
		}

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
				int aCount = 0;

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

				int vol=(int)twoDArray [200, 150] / 1000;
				if (vol > 1)
					vol = 1;
				audioL.volume = vol;
				audioL.Play ();
				 vol=(int)twoDArray [200, 300] / 1000;
				if (vol > 1)
					vol = 1;
				audioM.volume = vol;
				audioM.Play ();
				 vol=(int)twoDArray [200, 450] / 1000;
				if (vol > 1)
					vol = 1;
				audioR.volume = vol;
				audioR.Play ();
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
