using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class LinkageAnimation : MonoBehaviour
{
    #region field
    public bool Playing = true;
    public float Speed = 1.0f;
    public bool Loop = true;
    public int FrameLength = 0;
    public List<LinkageAnimationTarget> Targets;
    public List<LinkageAnimationCallBack> CallBacks;

    private int _playIndex = 0;
    private float _playLocation = 0.0f;
    #endregion

    private void Awake()
    {
        InitComponent();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    /// <summary>
    /// 重新播放动画
    /// </summary>
    public void RePlay()
    {
        _playIndex = 0;
        _playLocation = 0f;
        Playing = true;
    }

    /// <summary>
    /// 停止播放动画
    /// </summary>
    public void Stop()
    {
        Playing = false;
        SetFrame(1);
    }

    /// <summary>
    /// 设置动画为指定关键帧
    /// </summary>
    public void SetFrame(int index)
    {
        if (index > 0 && index <= FrameLength)
        {
            _playIndex = index - 1;
            _playLocation = 0f;

            for (int i = 0; i < Targets.Count; i++)
            {
                UpdateFrame(Targets[i], _playIndex);
            }
        }
    }

    #region auxiliary method
    /// <summary>
    /// 初始化运行时控件
    /// </summary>
    private void InitComponent()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            LinkageAnimationTarget lat = Targets[i];

            if (lat.Target)
            {
                if (lat.PropertysRunTime == null)
                {
                    lat.PropertysRunTime = new List<LAPropertyRunTime>();
                }

                for (int j = 0; j < lat.Propertys.Count; j++)
                {
                    LAProperty lap = lat.Propertys[j];
                    Component cp = lat.Target.GetComponent(lap.ComponentName);
                    PropertyInfo pi = cp ? cp.GetType().GetProperty(lap.PropertyName) : null;
                    bool valid = (cp != null && pi != null);
                    LAPropertyRunTime laprt = new LAPropertyRunTime(valid, cp, pi);
                    lat.PropertysRunTime.Add(laprt);
                }
            }
        }
    }

    /// <summary>
    /// 发起回调
    /// </summary>
    private void LaunchCallBack(int frameIndex)
    {
        int index = frameIndex + 1;
        LinkageAnimationCallBack lacb = CallBacks.Find((l) => l.Index == index);
        if (lacb != null && lacb.Target)
        {
            lacb.Target.SendMessage(lacb.Method);
        }
    }
    
    /// <summary>
    /// 更新动画
    /// </summary>
    private void UpdateAnimation()
    {
        if (Playing && FrameLength > 1)
        {
            if (_playLocation > 1f)
            {
                _playLocation = 0f;
                _playIndex += 1;
                if (_playIndex >= FrameLength)
                {
                    _playIndex = 0;
                }

                LaunchCallBack(_playIndex);
            }
            else
            {
                _playLocation += Time.deltaTime * Speed;
            }

            int current = _playIndex;
            int next = _playIndex + 1;
            if (next >= FrameLength)
            {
                next = 0;
                if (!Loop)
                {
                    Playing = false;
                    _playIndex = 0;
                    _playLocation = 0f;
                    return;
                }
            }

            for (int i = 0; i < Targets.Count; i++)
            {
                UpdateFrame(Targets[i], current, next);
            }
        }
    }

    /// <summary>
    /// 更新动画帧（两个值之间插值）
    /// </summary>
    private void UpdateFrame(LinkageAnimationTarget lat, int currentIndex, int nextIndex)
    {
        if (lat.Target)
        {
            LAFrame currentLAF = lat.Frames[currentIndex];
            LAFrame nextLAF = lat.Frames[nextIndex];

            for (int i = 0; i < lat.PropertysRunTime.Count; i++)
            {
                LAProperty lap = lat.Propertys[i];
                LAPropertyRunTime laprt = lat.PropertysRunTime[i];

                if (laprt.IsValid)
                {
                    object value = LinkageAnimationTool.Lerp(currentLAF.GetFrameValue(i), nextLAF.GetFrameValue(i), lap.PropertyType, _playLocation);
                    laprt.PropertyValue.SetValue(laprt.PropertyComponent, value, null);
                }
            }
        }
    }

    /// <summary>
    /// 更新动画帧（一个固定值）
    /// </summary>
    private void UpdateFrame(LinkageAnimationTarget lat, int currentIndex)
    {
        if (lat.Target)
        {
            LAFrame currentLAF = lat.Frames[currentIndex];

            for (int i = 0; i < lat.PropertysRunTime.Count; i++)
            {
                LAProperty lap = lat.Propertys[i];
                LAPropertyRunTime laprt = lat.PropertysRunTime[i];

                if (laprt.IsValid)
                {
                    object value = currentLAF.GetFrameValue(i);
                    laprt.PropertyValue.SetValue(laprt.PropertyComponent, value, null);
                }
            }
        }
    }
    #endregion
}
