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
using System.Runtime.InteropServices;

using UnityEngine;


namespace Phonon
{

    //
    // Basic types.
    //

    // Boolean values.
    public enum Bool
    {
        FALSE,
        TRUE
    }

    // Error codes.
    public enum Error
    {
        NONE,
        FAIL,
        OUT_OF_MEMORY,
        INITIALIZATION
    }

    // Global context.
    [StructLayout(LayoutKind.Sequential)]
    public struct GlobalContext
    {
        public IntPtr logCallback;
        public IntPtr allocateCallback;
        public IntPtr freeCallback;
    };


    //
    // Geometric types.
    //

    // Point in 3D space.
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
    }


    //
    // Audio pipeline.
    //

    // Supported channel layout types.
    public enum ChannelLayoutType
    {
        SPEAKERS,
        AMBISONICS
    }

    // Supported channel layouts.
    public enum ChannelLayout
    {
        MONO,
        STEREO,
        QUADRAPHONIC,
        FIVE_POINT_ONE,
        SEVEN_POINT_ONE,
        CUSTOM
    }

    // Supported channel order.
    public enum ChannelOrder
    {
        INTERLEAVED,
        DEINTERLEAVED
    }

    // Supported Ambisonics ordering.
    public enum AmbisonicsOrdering
    {
        FURSEMALHAM,
        ACN
    }

    // Supported Ambisonics normalization.
    public enum AmbisonicsNormalization
    {
        FURSEMALHAM,
        SN3D,
        N3D
    }

    // Audio format.
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioFormat
    {
        public ChannelLayoutType channelLayoutType;
        public ChannelLayout channelLayout;
        public int numSpeakers;
        public Vector3[] speakerDirections;
        public int ambisonicsOrder;
        public AmbisonicsOrdering ambisonicsOrdering;
        public AmbisonicsNormalization ambisonicsNormalization;
        public ChannelOrder channelOrder;
    }

    // Audio format.
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioBuffer
    {
        public AudioFormat audioFormat;
        public int numSamples;
        public float[] interleavedBuffer;
        public IntPtr deInterleavedBuffer;
    }

    // DSP parameters.
    [StructLayout(LayoutKind.Sequential)]
    public struct DSPParams
    {
        public int samplingRate;
        public int frameSize;
    }

    //
    // Common API functions.
    //
    public static class Common
    {
        public static Vector3 ConvertVector(UnityEngine.Vector3 point)
        {
            Vector3 convertedPoint;
            convertedPoint.x = point.x;
            convertedPoint.y = point.y;
            convertedPoint.z = -point.z;

            return convertedPoint;
        }
    }
}
