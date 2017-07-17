using UnityEngine;
using System.Collections.Generic;

public class LinkageAnimation : MonoBehaviour
{
    public bool Playing = true;
    public float Speed = 1.0f;
    public List<AnimationFrame> Targets;

    [SerializeField]
    private int _frameLength = 0;
    private int _frameIndex = 0;
    private float _frameLocation = 0.0f;

#if UNITY_EDITOR
    /// <summary>
    /// 添加联动物体
    /// </summary>
    public void AddAnimationFrame()
    {
        if (Targets == null)
            Targets = new List<AnimationFrame>();

        AnimationFrame af = new AnimationFrame(transform);
        for (int i = 0; i < _frameLength; i++)
        {
            af.Frames.Add(new Frame());
        }
        Targets.Add(af);
    }

    /// <summary>
    /// 移除联动物体
    /// </summary>
    public void RemoveAnimationFrame(AnimationFrame frame)
    {
        if (Targets == null)
            return;

        if (Targets.Contains(frame))
        {
            Targets.Remove(frame);
            if (Targets.Count <= 0)
                _frameLength = 0;
        }
    }

    /// <summary>
    /// 移除联动物体
    /// </summary>
    public void RemoveAtAnimationFrame(int index)
    {
        if (Targets == null)
            return;

        if (index >= 0 && index < Targets.Count)
        {
            Targets.RemoveAt(index);
            if (Targets.Count <= 0)
                _frameLength = 0;
        }
    }

    /// <summary>
    /// 添加关键帧
    /// </summary>
    public void AddFrame()
    {
        if (Targets == null || Targets.Count <= 0)
            return;

        for (int i = 0; i < Targets.Count; i++)
        {
            Targets[i].Frames.Add(new Frame());
        }
        _frameLength += 1;
    }

    /// <summary>
    /// 移除关键帧
    /// </summary>
    public void RemoveAtFrame(int index)
    {
        if (Targets == null || Targets.Count <= 0)
            return;

        for (int i = 0; i < Targets.Count; i++)
        {
            if (index >= 0 && index < Targets[i].Frames.Count)
            {
                Targets[i].Frames.RemoveAt(index);
            }
        }
        _frameLength -= 1;
    }

    public int FrameLength()
    {
        return _frameLength;
    }
#endif

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (Playing && Targets != null && _frameLength > 1)
        {
            if (_frameLocation >= 1.0f)
            {
                _frameLocation = 0.0f;
                _frameIndex += 1;
                if (_frameIndex >= (_frameLength - 1))
                {
                    _frameIndex = 0;
                }
            }
            else
            {
                _frameLocation += Time.deltaTime * Speed;
            }

            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Target != null && Targets[i].Frames.Count > 0)
                    UpdateFrame(Targets[i]);
            }
        }
    }

    private void UpdateFrame(AnimationFrame af)
    {
        if (!af.Frames[_frameIndex].PositionDisabled && !af.Frames[_frameIndex + 1].PositionDisabled)
            af.Target.localPosition = Vector3.Lerp(af.Frames[_frameIndex].Position, af.Frames[_frameIndex + 1].Position, _frameLocation);
        if (!af.Frames[_frameIndex].RotationDisabled && !af.Frames[_frameIndex + 1].RotationDisabled)
            af.Target.localRotation = Quaternion.Euler(Vector3.Lerp(af.Frames[_frameIndex].Rotation, af.Frames[_frameIndex + 1].Rotation, _frameLocation));
        if (!af.Frames[_frameIndex].ScaleDisabled && !af.Frames[_frameIndex + 1].ScaleDisabled)
            af.Target.localScale = Vector3.Lerp(af.Frames[_frameIndex].Scale, af.Frames[_frameIndex + 1].Scale, _frameLocation);
    }
}
