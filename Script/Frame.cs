using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AnimationFrame
{
    public Transform Target;
    public List<Frame> Frames;

    public AnimationFrame( Transform tf )
    {
        Target = tf;
        Frames = new List<Frame>();
    }
}

[System.Serializable]
public class Frame 
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public bool PositionDisabled;
    public bool RotationDisabled;
    public bool ScaleDisabled;
    public bool ShowInspector;

    public Frame()
    {
        Position = Vector3.zero;
        Rotation = Vector3.zero;
        Scale = Vector3.one;
        PositionDisabled = true;
        RotationDisabled = true;
        ScaleDisabled = true;
        ShowInspector = false;
    }
}
