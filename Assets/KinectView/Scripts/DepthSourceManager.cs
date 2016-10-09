using UnityEngine;
using System.Collections;
using Windows.Kinect;

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

    public ushort[] GetData()
    {
        return _Data;
    }

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
			depthPixels = new byte[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
			Debug.Log ("haha");
			minDepth = _Sensor.DepthFrameSource.DepthMinReliableDistance;
			maxDepth = ushort.MaxValue;
			int w = _Sensor.DepthFrameSource.FrameDescription.Width;
			int h = _Sensor.DepthFrameSource.FrameDescription.Height;
			Debug.Log (w);
			Debug.Log (h);
			Debug.Log (_Data.Length);


        }
    }
    

	float averageOut(int xMin,int xMax,int yMin,int yMax, ushort[][] twoDArray){
		float sum = 0;
		for (int i = xMin; i < xMax; i++) {
			for (int j = yMin; j < yMax; j++) {
				sum += twoDArray [i] [j];
			}
		}
		float result = sum / ((xMax - xMin + 1) * (yMax - yMin + 1));
		return result;
	}


    void Update () 
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);
                frame.Dispose();
                frame = null;
				twoDArray = new ushort[424,512];
                for (int x=0; x < 424; x++)
                    for (int y = 0; y < 512; y++)
                    {
						twoDArray[x,y]=_Data[x*512+y];
                    }

				averageOut (0, 100, 0, 100, twoDArray);
				averageOut (0, 100, 101, 201, twoDArray);
				averageOut (0, 100, 202,123 , twoDArray);
				averageOut (0, 100, 0, 100, twoDArray);



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
