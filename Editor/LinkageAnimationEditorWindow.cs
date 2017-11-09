using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public class LinkageAnimationEditorWindow : EditorWindow
{
    #region field
    private LinkageAnimation _LA;
    private int _showFrameIndex;
    private int _showTargetIndex;
    private int _width;
    private int _space;
    private Vector2 _mouseDownPos;
    private Vector2 _hscroll;
    #endregion

    public void Init(LinkageAnimation la)
    {
        _LA = la;
        _showFrameIndex = -1;
        _showTargetIndex = -1;
        _width = 200;
        _space = 5;
        _mouseDownPos = Vector2.zero;

        if (_LA.Targets == null)
        {
            _LA.Targets = new List<LinkageAnimationTarget>();
        }
        if (_LA.CallBacks == null)
        {
            _LA.CallBacks = new List<LinkageAnimationCallBack>();
        }
    }
    private void OnGUI()
    {
        if (Application.isPlaying || !_LA)
            return;

        ViewGUI();
        TitleGUI();
        FrameGUI();
    }
    private void TitleGUI()
    {
        SetGUIState(Color.white, Color.white, true);
        EditorGUILayout.BeginHorizontal("Toolbar");
        if (GUILayout.Button(_LA.name, "Toolbarbutton"))
        {
            Selection.activeGameObject = _LA.gameObject;
        }
        if (GUILayout.Button("Target Number:" + _LA.Targets.Count, "Toolbarbutton"))
        {
            GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Clear None Target"), false, delegate () {
                if (EditorUtility.DisplayDialog("提示", "是否删除所有空的联动物体！", "确定", "取消"))
                {
                    ClearNoneTarget();
                }
            });
            gm.ShowAsContext();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Target", "Toolbarbutton"))
        {
            AddTarget(new Vector2(position.width / 2, position.height / 2));
        }
        if (GUILayout.Button("Add Frame", "Toolbarbutton"))
        {
            AddFrame();
        }
        if (GUILayout.Button("Find Target", "Toolbarbutton"))
        {
            GenericMenu gm = new GenericMenu();
            for (int i = 0; i < _LA.Targets.Count; i++)
            {
                if (_LA.Targets[i].Target)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(_LA.Targets[j].Target.name), _showTargetIndex == j, delegate () {
                        FindTarget(j);
                    });
                }
            }
            gm.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
    }
    private void FrameGUI()
    {
        _hscroll = EditorGUILayout.BeginScrollView(_hscroll);
        EditorGUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("Key Frame:");
        for (int i = 0; i < _LA.FrameLength; i++)
        {
            if (GUILayout.Button("Frame" + (i + 1), _showFrameIndex == i ? "TE toolbarbutton" : "toolbarbutton", GUILayout.Width(60)))
            {
                _showFrameIndex = i;
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        if (_showFrameIndex != -1)
        {
            EditorGUILayout.BeginHorizontal();
            SetGUIState(Color.white, Color.white, true);
            if (GUILayout.Button("Get Value In Scene", "ButtonLeft"))
            {
                GetPropertyValue(_showFrameIndex);
            }
            if (GUILayout.Button("Set Value To Scene", "ButtonMid"))
            {
                SetPropertyValue(_showFrameIndex);
            }
            if (GUILayout.Button("Clone Frame", "ButtonMid"))
            {
                CloneFrame(_showFrameIndex);
            }
            if (GUILayout.Button("Delete Frame", "ButtonRight"))
            {
                if (EditorUtility.DisplayDialog("提示", "是否删除当前选中的关键帧！", "确定", "取消"))
                {
                    RemoveAtFrame(_showFrameIndex);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
    }
    private void ViewGUI()
    {
        #region 右键菜单
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GenericMenu gm = new GenericMenu();
                Vector2 mousePos = Event.current.mousePosition;
                gm.AddItem(new GUIContent("Add Target"), false, delegate ()
                {
                    AddTarget(mousePos);
                });
                gm.AddItem(new GUIContent("Add Frame"), false, delegate ()
                {
                    AddFrame();
                });
                gm.ShowAsContext();
            }
        }
        #endregion

        #region 移动视野
        if (Event.current.button == 2)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                _mouseDownPos = Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                Vector2 direction = (Event.current.mousePosition - _mouseDownPos);
                for (int i = 0; i < _LA.Targets.Count; i++)
                {
                    _LA.Targets[i].Anchor += direction;
                }
                _mouseDownPos = Event.current.mousePosition;
                Repaint();
            }
            Event.current.Use();
        }
        #endregion

        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            LinkageAnimationTarget lat = _LA.Targets[i];
            
            #region 窗体锚点
            SetGUIState(Color.white, Color.white, true);
            if (GUI.RepeatButton(new Rect(lat.Anchor.x - 25, lat.Anchor.y - 15, 50, 30), ""))
            {
                lat.Anchor = Event.current.mousePosition;
                _showTargetIndex = i;
                Repaint();
            }
            #endregion

            string style = (_showTargetIndex == i ? "flow node 0 on" : "flow node 0");
            GUI.BeginGroup(new Rect(lat.Anchor.x - _width / 2, lat.Anchor.y, _width, lat.Height), new GUIStyle(style));

            #region 动画目标物体
            int h = 5;
            SetGUIState(lat.Target ? Color.white : Color.red, Color.white, true);
            lat.Target = EditorGUI.ObjectField(new Rect(5, h, _width - 56, 16), lat.Target, typeof(GameObject), true) as GameObject;
            SetGUIState(Color.white, Color.cyan, true);
            if (GUI.Button(new Rect(_width - 46, h, 18, 16), lat.ShowPropertys ? "-" : "+"))
            {
                lat.ShowPropertys = !lat.ShowPropertys;
            }
            if (GUI.Button(new Rect(_width - 23, h, 18, 16), "x"))
            {
                if (EditorUtility.DisplayDialog("提示", "是否删除物体 " + (lat.Target ? lat.Target.name: "None") + " 及其所有动画属性、动画帧！", "确定", "取消"))
                {
                    RemoveTarget(lat);
                    break;
                }
            }
            h = h + 16 + _space;
            #endregion

            #region 属性列表
            if (lat.ShowPropertys)
            {
                SetGUIState(Color.white, Color.white, lat.Target);
                EditorGUI.LabelField(new Rect(5, h, _width - 10, 16), new GUIContent("Property List:"), "BoldLabel");
                h = h + 16 + _space;

                for (int j = 0; j < lat.Propertys.Count; j++)
                {
                    LAProperty lap = lat.Propertys[j];

                    string property = lap.ComponentName + "." + lap.PropertyName + "[" + lap.PropertyType + "]";
                    if (GUI.Button(new Rect(5, h, 18, 16), "x"))
                    {
                        if (EditorUtility.DisplayDialog("提示", "是否删除 " + lat.Target.name + " 的属性 " + property + "！", "确定", "取消"))
                        {
                            RemoveProperty(lat, j);
                            break;
                        }
                    }
                    EditorGUI.LabelField(new Rect(23, h, _width - 28, 16), property);
                    h = h + 16 + _space;

                    if (_showFrameIndex != -1)
                    {
                        switch (lap.PropertyType)
                        {
                            case "Bool":
                                bool oldB = (bool)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                bool newB = EditorGUI.Toggle(new Rect(23, h, _width - 28, 16), oldB);
                                h = h + 16 + _space;
                                if (oldB != newB)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newB);
                                }
                                break;
                            case "Color":
                                Color oldC = (Color)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                Color newC = EditorGUI.ColorField(new Rect(23, h, _width - 28, 16), oldC);
                                h = h + 16 + _space;
                                if (oldC != newC)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newC);
                                }
                                break;
                            case "Float":
                                float oldF = (float)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                float newF = EditorGUI.FloatField(new Rect(23, h, _width - 28, 16), oldF);
                                h = h + 16 + _space;
                                if (oldF != newF)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newF);
                                }
                                break;
                            case "Int":
                                int oldI = (int)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                int newI = EditorGUI.IntField(new Rect(23, h, _width - 28, 16), oldI);
                                h = h + 16 + _space;
                                if (oldI != newI)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newI);
                                }
                                break;
                            case "Quaternion":
                                Vector3 oldQ = ((Quaternion)lat.Frames[_showFrameIndex].GetFrameValue(j)).eulerAngles;
                                Vector3 newQ = EditorGUI.Vector3Field(new Rect(23, h, _width - 28, 16), "", oldQ);
                                h = h + 16 + _space;
                                if (oldQ != newQ)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, Quaternion.Euler(newQ));
                                }
                                break;
                            case "String":
                                string oldS = (string)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                string newS = EditorGUI.TextField(new Rect(23, h, _width - 28, 16), oldS);
                                h = h + 16 + _space;
                                if (oldS != newS)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newS);
                                }
                                break;
                            case "Vector2":
                                Vector2 oldV2 = (Vector2)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                Vector2 newV2 = EditorGUI.Vector2Field(new Rect(23, h, _width - 28, 16), "", oldV2);
                                h = h + 16 + _space;
                                if (oldV2 != newV2)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newV2);
                                }
                                break;
                            case "Vector3":
                                Vector3 oldV3 = (Vector3)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                Vector3 newV3 = EditorGUI.Vector3Field(new Rect(23, h, _width - 28, 16), "", oldV3);
                                h = h + 16 + _space;
                                if (oldV3 != newV3)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newV3);
                                }
                                break;
                            case "Vector4":
                                Vector4 oldV4 = (Vector4)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                Vector4 newV4 = EditorGUI.Vector4Field(new Rect(23, h, _width - 28, 16), "", oldV4);
                                h = h + 16 + _space;
                                if (oldV4 != newV4)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newV4);
                                }
                                break;
                            case "Sprite":
                                Sprite oldSp = (Sprite)lat.Frames[_showFrameIndex].GetFrameValue(j);
                                Sprite newSp = EditorGUI.ObjectField(new Rect(23, h, _width - 28, 16), oldSp, typeof(Sprite), true) as Sprite;
                                h = h + 16 + _space;
                                if (oldSp != newSp)
                                {
                                    lat.Frames[_showFrameIndex].SetFrameValue(j, newSp);
                                }
                                break;
                        }
                    }
                }
            }
            #endregion

            #region 添加属性
            if (lat.ShowPropertys)
            {
                if (GUI.Button(new Rect(5, h, _width - 10, 16), "Add Property"))
                {
                    GenericMenu gm = new GenericMenu();
                    Component[] cps = lat.Target.GetComponents<Component>();
                    for (int m = 0; m < cps.Length; m++)
                    {
                        Type type = cps[m].GetType();
                        PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        for (int n = 0; n < pis.Length; n++)
                        {
                            PropertyInfo pi = pis[n];
                            string propertyType = pi.PropertyType.Name;
                            propertyType = LinkageAnimationTool.ReplaceType(propertyType);
                            bool allow = LinkageAnimationTool.IsAllowType(propertyType);

                            if (allow)
                            {
                                if (pi.CanRead && pi.CanWrite)
                                {
                                    gm.AddItem(new GUIContent(type.Name + "/" + "[" + propertyType + "] " + pi.Name), false, delegate ()
                                    {
                                        LAProperty lap = new LAProperty(type.Name, propertyType, pi.Name);
                                        AddProperty(lat, lap);
                                    });
                                }
                            }
                        }
                    }
                    gm.ShowAsContext();
                }
                h = h + 16 + _space;
            }
            lat.Height = h;
            #endregion

            GUI.EndGroup();
        }
    }

    #region auxiliary method
    private void SetGUIState(Color color, Color bgColor, bool enabled)
    {
        GUI.color = color;
        GUI.backgroundColor = bgColor;
        GUI.enabled = enabled;
    }

    /// <summary>
    /// 添加联动物体
    /// </summary>
    private void AddTarget(Vector2 pos)
    {
        LinkageAnimationTarget lat = new LinkageAnimationTarget(null, pos);
        for (int i = 0; i < _LA.FrameLength; i++)
        {
            lat.Frames.Add(new LAFrame());
        }
        _LA.Targets.Add(lat);
    }

    /// <summary>
    /// 移除联动物体
    /// </summary>
    private void RemoveTarget(LinkageAnimationTarget target)
    {
        if (_LA.Targets.Contains(target))
        {
            _LA.Targets.Remove(target);
            if (_LA.Targets.Count <= 0)
            {
                _LA.FrameLength = 0;
                _showFrameIndex = -1;
                _showTargetIndex = -1;
            }
            _showTargetIndex = -1;
        }
    }

    /// <summary>
    /// 移除联动物体
    /// </summary>
    private void RemoveTarget(int index)
    {
        if (index >= 0 && index < _LA.Targets.Count)
        {
            _LA.Targets.RemoveAt(index);
            if (_LA.Targets.Count <= 0)
            {
                _LA.FrameLength = 0;
                _showFrameIndex = -1;
                _showTargetIndex = -1;
            }
            _showTargetIndex = -1;
        }
    }

    /// <summary>
    /// 清除空联动物体
    /// </summary>
    private void ClearNoneTarget()
    {
        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            if (!_LA.Targets[i].Target)
            {
                RemoveTarget(_LA.Targets[i]);
                i -= 1;
            }
        }
    }

    /// <summary>
    /// 查找联动物体
    /// </summary>
    private void FindTarget(int index)
    {
        _LA.Targets[index].Anchor = new Vector2(position.width / 2, position.height / 2);
        _showTargetIndex = index;
    }

    /// <summary>
    /// 添加关键帧
    /// </summary>
    private void AddFrame()
    {
        if (_LA.Targets.Count <= 0)
            return;

        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            LinkageAnimationTarget lat = _LA.Targets[i];
            LAFrame laf = new LAFrame();
            
            for (int j = 0; j < lat.Propertys.Count; j++)
            {
                LAProperty lap = lat.Propertys[j];
                object value = LinkageAnimationTool.GenerateOriginalValue(lap.PropertyType);
                laf.AddFrameValue(lap.PropertyType, value);
            }
            lat.Frames.Add(laf);
        }
        _LA.FrameLength += 1;
    }

    /// <summary>
    /// 克隆关键帧
    /// </summary>
    private void CloneFrame(int index)
    {
        if (_LA.Targets.Count <= 0)
            return;

        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            LAFrame laf = new LAFrame();
            laf = _LA.Targets[i].Frames[index].Clone();
            _LA.Targets[i].Frames.Add(laf);
        }
        _LA.FrameLength += 1;
    }

    /// <summary>
    /// 移除关键帧
    /// </summary>
    private void RemoveAtFrame(int index)
    {
        if (_LA.Targets.Count <= 0)
            return;

        if (index >= 0 && index < _LA.FrameLength)
        {
            for (int i = 0; i < _LA.Targets.Count; i++)
            {
                _LA.Targets[i].Frames.RemoveAt(index);
            }
            _LA.FrameLength -= 1;
            _showFrameIndex = -1;
        }
    }

    /// <summary>
    /// 添加动画目标属性
    /// </summary>
    private void AddProperty(LinkageAnimationTarget lat, LAProperty lap)
    {
        lat.Propertys.Add(lap);

        object value = LinkageAnimationTool.GenerateOriginalValue(lap.PropertyType);
        for (int i = 0; i < lat.Frames.Count; i++)
        {
            lat.Frames[i].AddFrameValue(lap.PropertyType, value);
        }
    }

    /// <summary>
    /// 移除动画目标属性
    /// </summary>
    private void RemoveProperty(LinkageAnimationTarget lat, int index)
    {
        lat.Propertys.RemoveAt(index);

        for (int i = 0; i < lat.Frames.Count; i++)
        {
            lat.Frames[i].RemoveFrameValue(index);
        }
    }

    /// <summary>
    /// 获取目标属性值并替换当前帧数据
    /// </summary>
    private void GetPropertyValue(int index)
    {
        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            LinkageAnimationTarget lat = _LA.Targets[i];
            if (lat.Target)
            {
                LAFrame laf = lat.Frames[index];
                for (int j = 0; j < lat.Propertys.Count; j++)
                {
                    Component cp = lat.Target.GetComponent(lat.Propertys[j].ComponentName);
                    if (cp != null)
                    {
                        PropertyInfo pi = cp.GetType().GetProperty(lat.Propertys[j].PropertyName);
                        if (pi != null)
                        {
                            object value = pi.GetValue(cp, null);
                            laf.SetFrameValue(j, value);
                        }
                        else
                        {
                            Debug.LogWarning("目标物体 " + lat.Target.name + " 的组件 " + lat.Propertys[j].ComponentName + " 不存在属性 " + lat.Propertys[j].PropertyName + "！");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("目标物体 " + lat.Target.name + " 不存在组件 " + lat.Propertys[j].ComponentName + "！");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置当前帧数据至目标属性值
    /// </summary>
    private void SetPropertyValue(int index)
    {
        for (int i = 0; i < _LA.Targets.Count; i++)
        {
            LinkageAnimationTarget lat = _LA.Targets[i];
            if (lat.Target)
            {
                LAFrame laf = lat.Frames[index];
                for (int j = 0; j < lat.Propertys.Count; j++)
                {
                    Component cp = lat.Target.GetComponent(lat.Propertys[j].ComponentName);
                    if (cp != null)
                    {
                        PropertyInfo pi = cp.GetType().GetProperty(lat.Propertys[j].PropertyName);
                        if (pi != null)
                        {
                            pi.SetValue(cp, laf.GetFrameValue(j), null);
                        }
                        else
                        {
                            Debug.LogWarning("目标物体 " + lat.Target.name + " 的组件 " + lat.Propertys[j].ComponentName + " 不存在属性 " + lat.Propertys[j].PropertyName + "！");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("目标物体 " + lat.Target.name + " 不存在组件 " + lat.Propertys[j].ComponentName + "！");
                    }
                }
            }
        }
    }
    #endregion
}
