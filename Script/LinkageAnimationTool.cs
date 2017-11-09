using System.Collections.Generic;
using UnityEngine;

public static class LinkageAnimationTool 
{
    /*添加新的属性类型至动画编辑器
     * 1、在AllowTypes中加入新类型的名称（注意：若类型实际名称与AllowTypes中的监听名称不相同，需要在ReplaceType方法中替换类型名称）
     * 2、在LAFrame类（LinkageAnimationTarget.cs）中加入新类型的List集合，用于存储新类型的值
     * 3、在AddFrameValue、SetFrameValue、GetFrameValue、RemoveFrameValue、Clone方法中分别加入对新类型的值处理方法
     * 4、在GenerateOriginalValue方法中加入新类型的初始值生成方法
     * 5、在Lerp方法中加入新类型的插值算法
     * 6、在LinkageAnimationEditorWindow类中TargetGUI方法中加入新类型的GUI显示控件
         */

    private static readonly List<string> AllowTypes = new List<string>() {
        "Bool","Color","Float","Int","Quaternion","String","Vector2","Vector3","Vector4","Sprite"
    };

    /// <summary>
    /// 检查属性类型是否是合法类型
    /// </summary>
    public static bool IsAllowType(string type)
    {
        return AllowTypes.Contains(type);
    }

    /// <summary>
    /// 替换属性类型名称为标准名称
    /// </summary>
    public static string ReplaceType(string type)
    {
        string newType = "";
        if (AllowTypes.Contains(type))
        {
            newType = type;
        }
        else
        {
            switch (type)
            {
                case "Boolean":
                    newType = "Bool";
                    break;
                case "Single":
                    newType = "Float";
                    break;
                case "Int32":
                    newType = "Int";
                    break;
                default:
                    newType = type;
                    break;
            }
        }
        return newType;
    }

    /// <summary>
    /// 根据类型生成指定的属性初始值
    /// </summary>
    public static object GenerateOriginalValue(string type)
    {
        switch (type)
        {
            case "Bool":
                return true;
            case "Color":
                return Color.white;
            case "Float":
                return 0f;
            case "Int":
                return 0;
            case "Quaternion":
                return Quaternion.identity;
            case "String":
                return "";
            case "Vector2":
                return Vector2.zero;
            case "Vector3":
                return Vector3.zero;
            case "Vector4":
                return Vector4.zero;
            case "Sprite":
                return null;
            default:
                return null;
        }
    }

    /// <summary>
    /// 根据类型在两个属性间插值
    /// </summary>
    public static object Lerp(object value1, object value2, string type, float location)
    {
        object value;
        switch (type)
        {
            case "Bool":
                value = location < 0.5f ? (bool)value1 : (bool)value2;
                break;
            case "Color":
                value = Color.Lerp((Color)value1, (Color)value2, location);
                break;
            case "Float":
                float f1 = (float)value1;
                float f2 = (float)value2;
                value = f1 + (f2 - f1) * location;
                break;
            case "Int":
                int i1 = (int)value1;
                int i2 = (int)value2;
                value = (int)(i1 + (i2 - i1) * location);
                break;
            case "Quaternion":
                value = Quaternion.Lerp((Quaternion)value1, (Quaternion)value2, location);
                break;
            case "String":
                string s1 = (string)value1;
                string s2 = (string)value2;
                int length = (int)(s1.Length + (s2.Length - s1.Length) * location);
                value = s1.Length >= s2.Length ? s1.Substring(0, length) : s2.Substring(0, length);
                break;
            case "Vector2":
                value = Vector2.Lerp((Vector2)value1, (Vector2)value2, location);
                break;
            case "Vector3":
                value = Vector3.Lerp((Vector3)value1, (Vector3)value2, location);
                break;
            case "Vector4":
                value = Vector4.Lerp((Vector4)value1, (Vector4)value2, location);
                break;
            case "Sprite":
                value = location < 0.5f ? (Sprite)value1 : (Sprite)value2;
                break;
            default:
                value = null;
                break;
        }
        return value;
    }

    /// <summary>
    /// 新增关键帧属性值
    /// </summary>
    public static void AddFrameValue(this LAFrame frame, string type, object value)
    {
        int i = 0;
        switch (type)
        {
            case "Bool":
                frame.ValueBool.Add((bool)value);
                i = frame.ValueBool.Count - 1;
                break;
            case "Color":
                frame.ValueColor.Add((Color)value);
                i = frame.ValueColor.Count - 1;
                break;
            case "Float":
                frame.ValueFloat.Add((float)value);
                i = frame.ValueFloat.Count - 1;
                break;
            case "Int":
                frame.ValueInt.Add((int)value);
                i = frame.ValueInt.Count - 1;
                break;
            case "Quaternion":
                frame.ValueQuaternion.Add((Quaternion)value);
                i = frame.ValueQuaternion.Count - 1;
                break;
            case "String":
                frame.ValueString.Add((string)value);
                i = frame.ValueString.Count - 1;
                break;
            case "Vector2":
                frame.ValueVector2.Add((Vector2)value);
                i = frame.ValueVector2.Count - 1;
                break;
            case "Vector3":
                frame.ValueVector3.Add((Vector3)value);
                i = frame.ValueVector3.Count - 1;
                break;
            case "Vector4":
                frame.ValueVector4.Add((Vector4)value);
                i = frame.ValueVector4.Count - 1;
                break;
            case "Sprite":
                frame.ValueSprite.Add((Sprite)value);
                i = frame.ValueSprite.Count - 1;
                break;
            default:
                return;
        }

        LAValueIndex lvi = new LAValueIndex();
        lvi.Type = type;
        lvi.Index = i;
        frame.ValueIndex.Add(lvi);
    }

    /// <summary>
    /// 设置关键帧属性值
    /// </summary>
    public static void SetFrameValue(this LAFrame frame, int index, object value)
    {
        LAValueIndex lvi = frame.ValueIndex[index];
        int i = lvi.Index;
        switch (lvi.Type)
        {
            case "Bool":
                frame.ValueBool[i] = (bool)value;
                break;
            case "Color":
                frame.ValueColor[i] = (Color)value;
                break;
            case "Float":
                frame.ValueFloat[i] = (float)value;
                break;
            case "Int":
                frame.ValueInt[i] = (int)value;
                break;
            case "Quaternion":
                frame.ValueQuaternion[i] = (Quaternion)value;
                break;
            case "String":
                frame.ValueString[i] = (string)value;
                break;
            case "Vector2":
                frame.ValueVector2[i] = (Vector2)value;
                break;
            case "Vector3":
                frame.ValueVector3[i] = (Vector3)value;
                break;
            case "Vector4":
                frame.ValueVector4[i] = (Vector4)value;
                break;
            case "Sprite":
                frame.ValueSprite[i] = (Sprite)value;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 获取关键帧属性值
    /// </summary>
    public static object GetFrameValue(this LAFrame frame, int index)
    {
        LAValueIndex lvi = frame.ValueIndex[index];
        int i = lvi.Index;
        switch (lvi.Type)
        {
            case "Bool":
                return frame.ValueBool[i];
            case "Color":
                return frame.ValueColor[i];
            case "Float":
                return frame.ValueFloat[i];
            case "Int":
                return frame.ValueInt[i];
            case "Quaternion":
                return frame.ValueQuaternion[i];
            case "String":
                return frame.ValueString[i];
            case "Vector2":
                return frame.ValueVector2[i];
            case "Vector3":
                return frame.ValueVector3[i];
            case "Vector4":
                return frame.ValueVector4[i];
            case "Sprite":
                return frame.ValueSprite[i];
            default:
                return null;
        }
    }

    /// <summary>
    /// 移除关键帧属性值
    /// </summary>
    public static void RemoveFrameValue(this LAFrame frame, int index)
    {
        LAValueIndex lvi = frame.ValueIndex[index];
        frame.ValueIndex.RemoveAt(index);

        int i = lvi.Index;
        switch (lvi.Type)
        {
            case "Bool":
                frame.ValueBool.RemoveAt(i);
                break;
            case "Color":
                frame.ValueColor.RemoveAt(i);
                break;
            case "Float":
                frame.ValueFloat.RemoveAt(i);
                break;
            case "Int":
                frame.ValueInt.RemoveAt(i);
                break;
            case "Quaternion":
                frame.ValueQuaternion.RemoveAt(i);
                break;
            case "String":
                frame.ValueString.RemoveAt(i);
                break;
            case "Vector2":
                frame.ValueVector2.RemoveAt(i);
                break;
            case "Vector3":
                frame.ValueVector3.RemoveAt(i);
                break;
            case "Vector4":
                frame.ValueVector4.RemoveAt(i);
                break;
            case "Sprite":
                frame.ValueSprite.RemoveAt(i);
                break;
            default:
                break;
        }

        for (int j = 0; j < frame.ValueIndex.Count; j++)
        {
            if (frame.ValueIndex[j].Type == lvi.Type && frame.ValueIndex[j].Index > i)
            {
                LAValueIndex lav = new LAValueIndex();
                lav.Type = lvi.Type;
                lav.Index = frame.ValueIndex[j].Index - 1;
                frame.ValueIndex[j] = lav;
            }
        }
    }

    /// <summary>
    /// 克隆关键帧属性值
    /// </summary>
    public static LAFrame Clone(this LAFrame frame)
    {
        LAFrame laf = new LAFrame();
        laf.ValueIndex = new List<LAValueIndex>(frame.ValueIndex);
        laf.ValueBool = new List<bool>(frame.ValueBool);
        laf.ValueColor = new List<Color>(frame.ValueColor);
        laf.ValueFloat = new List<float>(frame.ValueFloat);
        laf.ValueInt = new List<int>(frame.ValueInt);
        laf.ValueQuaternion = new List<Quaternion>(frame.ValueQuaternion);
        laf.ValueString = new List<string>(frame.ValueString);
        laf.ValueVector2 = new List<Vector2>(frame.ValueVector2);
        laf.ValueVector3 = new List<Vector3>(frame.ValueVector3);
        laf.ValueVector4 = new List<Vector4>(frame.ValueVector4);
        laf.ValueSprite = new List<Sprite>(frame.ValueSprite);
        return laf;
    }
}
