using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class LinkageAnimationTarget
{
    //动画目标GameObject
    public GameObject Target;
    //动画目标在编辑器面板的锚点
    public Vector2 Anchor;
    //动画目标在编辑器面板的高度
    public int Height;
    //动画目标属性是否在编辑器面板显示
    public bool ShowPropertys;
    //动画目标属性
    public List<LAProperty> Propertys;
    //动画目标属性实例
    public List<LAPropertyRunTime> PropertysRunTime;
    //动画目标属性值关键帧
    public List<LAFrame> Frames;

    public LinkageAnimationTarget(GameObject target, Vector2 anchor)
    {
        Target = target;
        Anchor = anchor;
        Height = 100;
        ShowPropertys = true;
        Propertys = new List<LAProperty>();
        PropertysRunTime = new List<LAPropertyRunTime>();
        Frames = new List<LAFrame>();
    }
}

[System.Serializable]
public class LAProperty
{
    //属性所属组件名称
    public string ComponentName;
    //属性类型
    public string PropertyType;
    //属性名
    public string PropertyName;

    public LAProperty(string componentName, string type, string name)
    {
        ComponentName = componentName;
        PropertyType = type;
        PropertyName = name;
    }
}

public class LAPropertyRunTime
{
    //属性是否有效
    public bool IsValid;
    //属性所属组件
    public Component PropertyComponent;
    //属性值
    public PropertyInfo PropertyValue;

    public LAPropertyRunTime(bool valid, Component component, PropertyInfo value)
    {
        IsValid = valid;
        PropertyComponent = component;
        PropertyValue = value;
    }
}

[System.Serializable]
public class LAFrame
{
    //属性值仓库
    public List<LAValueIndex> ValueIndex;
    public List<bool> ValueBool;
    public List<Color> ValueColor;
    public List<float> ValueFloat;
    public List<int> ValueInt;
    public List<Quaternion> ValueQuaternion;
    public List<string> ValueString;
    public List<Vector2> ValueVector2;
    public List<Vector3> ValueVector3;
    public List<Vector4> ValueVector4;
    public List<Sprite> ValueSprite;

    public LAFrame()
    {
        ValueIndex = new List<LAValueIndex>();
        ValueBool = new List<bool>();
        ValueColor = new List<Color>();
        ValueFloat = new List<float>();
        ValueInt = new List<int>();
        ValueQuaternion = new List<Quaternion>();
        ValueString = new List<string>();
        ValueVector2 = new List<Vector2>();
        ValueVector3 = new List<Vector3>();
        ValueVector4 = new List<Vector4>();
        ValueSprite = new List<Sprite>();
    }
}

[System.Serializable]
public struct LAValueIndex
{
    public string Type;
    public int Index;
}

[System.Serializable]
public class LinkageAnimationCallBack
{
    //关键帧索引
    public int Index;
    //回调目标
    public GameObject Target;
    //回调方法
    public string Method;

    public LinkageAnimationCallBack()
    {
        Index = 1;
        Target = null;
        Method = "";
    }
}
