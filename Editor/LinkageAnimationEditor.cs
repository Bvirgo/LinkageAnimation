using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

[CustomEditor(typeof(LinkageAnimation))]
public class LinkageAnimationEditor : Editor 
{
    private LinkageAnimation _LA;
    private bool _showCallBack;

    private void OnEnable()
    {
        _LA = target as LinkageAnimation;
        _showCallBack = false;

        if (_LA.Targets == null)
        {
            _LA.Targets = new List<LinkageAnimationTarget>();
        }
        if (_LA.CallBacks == null)
        {
            _LA.CallBacks = new List<LinkageAnimationCallBack>();
        }
    }

    public override void  OnInspectorGUI()
    {
        if (Application.isPlaying || !_LA)
            return;

        GUI.backgroundColor = Color.cyan;
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit Animation"))
        {
            LinkageAnimationEditorWindow laew = EditorWindow.GetWindow<LinkageAnimationEditorWindow>();
            laew.titleContent = new GUIContent(EditorGUIUtility.IconContent("BuildSettings.Web.Small"));
            laew.titleContent.text = "LAWindow";
            laew.Init(_LA);
            laew.Show();
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Playing", GUILayout.Width(80));
        _LA.Playing = GUILayout.Toggle(_LA.Playing, "");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Loop", GUILayout.Width(80));
        _LA.Loop = GUILayout.Toggle(_LA.Loop, "");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Speed", GUILayout.Width(80));
        _LA.Speed = EditorGUILayout.FloatField(_LA.Speed);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _showCallBack = EditorGUILayout.Foldout(_showCallBack, "CallBack List");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add CallBack", "minibutton"))
        {
            _LA.CallBacks.Add(new LinkageAnimationCallBack());
        }
        EditorGUILayout.EndHorizontal();

        if (_showCallBack)
        {
            for (int i = 0; i < _LA.CallBacks.Count; i++)
            {
                LinkageAnimationCallBack lacb = _LA.CallBacks[i];
                GUI.enabled = true;
                EditorGUILayout.BeginVertical("HelpBox");

                if (lacb.Index < 1 || lacb.Index > _LA.FrameLength)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Frame index is out of range!", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Frame Index:", GUILayout.Width(100));
                lacb.Index = EditorGUILayout.IntField(lacb.Index);
                if (GUILayout.Button("x", "minibutton", GUILayout.Width(20)))
                {
                    _LA.CallBacks.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Call Target:", GUILayout.Width(100));
                lacb.Target = EditorGUILayout.ObjectField(lacb.Target, typeof(GameObject), true) as GameObject;
                EditorGUILayout.EndHorizontal();

                GUI.enabled = lacb.Target;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Call Method:", GUILayout.Width(100));
                if (GUILayout.Button(lacb.Method, "MiniPopup"))
                {
                    GenericMenu gm = new GenericMenu();
                    Component[] cps = lacb.Target.GetComponents<Component>();
                    for (int m = 0; m < cps.Length; m++)
                    {
                        Type type = cps[m].GetType();
                        MethodInfo[] mis = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                        for (int n = 0; n < mis.Length; n++)
                        {
                            string name = mis[n].Name;
                            if (!name.StartsWith("set_") && !name.StartsWith("get_"))
                            {
                                gm.AddItem(new GUIContent(type.Name + "/" + name), name == lacb.Method, delegate ()
                                {
                                    lacb.Method = name;
                                });
                            }
                        }
                    }
                    gm.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
    }
}
