/************************************************************************/
/* Copyright (C) 2011-2015 Impulsonic Inc. All Rights Reserved.         */
/*                                                                      */
/* The source code, information  and  material ("Material") contained   */
/* herein is owned  by Impulsonic Inc. or its suppliers or licensors,   */
/* and title to such  Material remains  with Impulsonic  Inc.  or its   */
/* suppliers or licensors. The Material contains proprietary informa-   */
/* tion  of  Impulsonic or  its  suppliers and licensors. No  part of   */
/* the Material may be used, copied, reproduced, modified, published,   */
/* uploaded, posted, transmitted, distributed or disclosed in any way   */
/* without Impulsonic's prior express written permission. No  license   */
/* under  any patent, copyright or other intellectual property rights   */
/* in the Material is  granted  to  or  conferred  upon  you,  either   */
/* expressly, by implication, inducement, estoppel or otherwise.  Any   */
/* license  under  such intellectual property rights must  be express   */
/* and approved by Impulsonic in writing.                               */
/*                                                                      */
/* Third Party trademarks are the property of their respective owners.  */
/*                                                                      */
/* Unless otherwise  agreed upon by Impulsonic  in  writing, you  may   */
/* not remove or  alter this  notice or any other  notice embedded in   */
/* Materials by Impulsonic or Impulsonic's  suppliers or licensors in   */
/* any way.                                                             */
/************************************************************************/

using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Phonon;


//
// Phonon3DSource
// Enables binaural rendering for a given source.
//

[AddComponentMenu("Phonon/Phonon 3D Source")]
public class Phonon3DSource : MonoBehaviour
{
	//
	// Initializes the source.
	//
	void Awake()
	{
        // If no AudioSource is attached to this GameObject,
        // disable binaural filtering.
        if (GetComponent<AudioSource>() == null)
        {
            Debug.LogError("No AudioSource attached to Phonon 3D Source. Phonon 3D effects disabled for GameObject: " + gameObject.name + ".");
            return;
        }

        // If the speaker configuration does not have 2 channels,
        // disable binaural filtering.
        if (AudioSettings.speakerMode != AudioSpeakerMode.Stereo)
        {
            Debug.LogError("Phonon 3D requires stereo output. Use Edit > Project Settings > Audio > Default Speaker Mode to fix this.");
            return;
        }

        GlobalContext globalContext;
        globalContext.logCallback = IntPtr.Zero;
        globalContext.allocateCallback = IntPtr.Zero;
        globalContext.freeCallback = IntPtr.Zero;

        int numBuffers, frameSize;
        AudioSettings.GetDSPBufferSize(out frameSize, out numBuffers);

        DSPParams dspParams;
        dspParams.samplingRate = AudioSettings.outputSampleRate;
        dspParams.frameSize = frameSize;

        if (Phonon3D.iplCreate3DContext(globalContext, dspParams, null, ref context) != Error.NONE)
        {
            Debug.Log("Unable to create Phonon 3D context for object: " + gameObject.name + ". Please check the log file for details.");
            return;
        }

        inputFormat.channelLayoutType = ChannelLayoutType.SPEAKERS;
        inputFormat.channelLayout = ChannelLayout.STEREO;
        inputFormat.numSpeakers = 2;
        inputFormat.speakerDirections = null;
        inputFormat.ambisonicsOrder = -1;
        inputFormat.ambisonicsOrdering = AmbisonicsOrdering.ACN;
        inputFormat.ambisonicsNormalization = AmbisonicsNormalization.N3D;
        inputFormat.channelOrder = ChannelOrder.INTERLEAVED;

        outputFormat.channelLayoutType = ChannelLayoutType.SPEAKERS;
        outputFormat.channelLayout = ChannelLayout.STEREO;
        outputFormat.numSpeakers = 2;
        outputFormat.speakerDirections = null;
        outputFormat.ambisonicsOrder = -1;
        outputFormat.ambisonicsOrdering = AmbisonicsOrdering.ACN;
        outputFormat.ambisonicsNormalization = AmbisonicsNormalization.N3D;
        outputFormat.channelOrder = ChannelOrder.INTERLEAVED;

        if (Phonon3D.iplCreateBinauralEffect(context, inputFormat, outputFormat, ref effect) != Error.NONE)
        {
            Debug.Log("Unable to create Phonon 3D effect for object: " + gameObject.name + ". Please check the log file for details.");
            return;
        }

        inputBuffer.numSamples = frameSize;
        inputBuffer.audioFormat = inputFormat;
        inputBuffer.deInterleavedBuffer = IntPtr.Zero;

        outputBuffer.numSamples = frameSize;
        outputBuffer.audioFormat = outputFormat;
        outputBuffer.deInterleavedBuffer = IntPtr.Zero;

        listener = FindObjectOfType<AudioListener>();
        StartCoroutine(EndOfFrameUpdate());
    }

    //
    // Destroys the source.
    //
    void OnDestroy()
	{
        Phonon3D.iplDestroyBinauralEffect(ref effect);
        Phonon3D.iplDestroy3DContext(ref context);
    }

    private IEnumerator EndOfFrameUpdate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            UpdateRelativeDirection();
        }
    }

    //
    // Updates the direction of the source relative to the listener.
    // Wait until the end of the frame to update the position to get latest information.
    //
    private void UpdateRelativeDirection()
    {
        UnityEngine.Vector3 wsVector;
        UnityEngine.Vector3 lsVector;

        wsVector = transform.position - listener.transform.position;
        lsVector = listener.transform.InverseTransformDirection(wsVector);

        if (lsVector.x == 0 && lsVector.y == 0 && lsVector.z == 0)
            lsVector.y = .001f;

        lsVector.Normalize();
        relativeDirection = Phonon.Common.ConvertVector(lsVector);
    }

    
    //
    // Applies the Phonon 3D effect to dry audio.
    //
    void OnAudioFilterRead(float[] data, int channels)
	{
        if (effect == IntPtr.Zero || data == null)
            return;

        inputBuffer.interleavedBuffer = data;
        outputBuffer.interleavedBuffer = data;

        Phonon3D.iplApplyBinauralEffect(effect, inputBuffer, relativeDirection, HRTFInterpolation.NEAREST, outputBuffer);
    }

    //
    // Data members.
    //

    // Orientation data.
    Phonon.Vector3 relativeDirection;
    AudioListener listener;

    // API handles.
    IntPtr context = IntPtr.Zero;
	IntPtr effect = IntPtr.Zero;

    // Audio formats
    AudioFormat inputFormat;
    AudioFormat outputFormat;

    // Audio buffers
    AudioBuffer inputBuffer;
    AudioBuffer outputBuffer;
}
