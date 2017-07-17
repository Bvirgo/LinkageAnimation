using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( LinkageAnimation ) )]
public class LinkageAnimationEditor : Editor 
{
    private LinkageAnimation _linkageAnimation;
    private int _showFrameIndex;

    private void OnEnable()
    {
        _linkageAnimation = target as LinkageAnimation;
        _showFrameIndex = -1;
    }

	public override void  OnInspectorGUI()
    {
        #region 是否播放动画、动画播放速度
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Linkage Animation"
            + "\r\nTarget Number:" + (_linkageAnimation.Targets == null ? 0: _linkageAnimation.Targets.Count) 
            + "\r\nFrame Number:" + _linkageAnimation.FrameLength()
            , MessageType.Info);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _linkageAnimation.Playing = GUILayout.Toggle( _linkageAnimation.Playing, "" , "SoloToggle");
        GUI.enabled = _linkageAnimation.Playing;
        GUILayout.Label(_linkageAnimation.Playing ? "Playing" : "Pause");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( "Speed" );
        _linkageAnimation.Speed = EditorGUILayout.FloatField( _linkageAnimation.Speed );
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        #endregion

        #region 添加动画物体、添加动画帧
        if( Application.isPlaying )
            return;

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.cyan;
        if ( GUILayout.Button( "Add Target", "LargeButtonLeft" ) )
        {
            _linkageAnimation.AddAnimationFrame();
        }
        if( GUILayout.Button( "Add Frame", "LargeButtonRight" ) )
        {
            _linkageAnimation.AddFrame();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        #endregion

        #region 动画帧数据
        if( _linkageAnimation.Targets != null )
        {
            #region 当前动画帧数
            EditorGUILayout.BeginHorizontal("HelpBox");
            for( int i = 0; i < _linkageAnimation.FrameLength(); i++ )
            {
                if( GUILayout.Button("Frame" + i, _showFrameIndex == i ? "TE toolbarbutton" : "toolbarbutton", GUILayout.Width( 60 )) )
                {
                    _showFrameIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region 动画信息
            for( int i = 0; i < _linkageAnimation.Targets.Count; i++ )
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.red;
                if( GUILayout.Button( "Delete", "minibutton", GUILayout.Width( 50 ) ) )
                {
                    _linkageAnimation.RemoveAtAnimationFrame( i );
                    continue;
                }
                GUI.color = Color.white;
                _linkageAnimation.Targets[i].Target = EditorGUILayout.ObjectField("", _linkageAnimation.Targets[i].Target, typeof(Transform), true) as Transform;
                EditorGUILayout.EndHorizontal();

                if( _showFrameIndex >= 0 && _showFrameIndex < _linkageAnimation.FrameLength() )
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space( 50 );
                    bool b = _linkageAnimation.Targets[i].Frames[_showFrameIndex].ShowInspector;
                    if ( GUILayout.Button( "Transform：", b ? "OL Plus" : "OL Minus") )
                    {
                        _linkageAnimation.Targets[i].Frames[_showFrameIndex].ShowInspector = !b;
                    }
                    EditorGUILayout.EndHorizontal();

                    if(!b)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space( 80 );
                        if( GUILayout.Button( "Get", "minibutton", GUILayout.Width( 30 ) ) )
                        {
                            if (_linkageAnimation.Targets[i].Target != null)
                                _linkageAnimation.Targets[i].Frames[_showFrameIndex].Position = _linkageAnimation.Targets[i].Target.localPosition;
                        }
                        if( GUILayout.Button( "Forbid", "minibutton", GUILayout.Width( 50 ) ) )
                        {
                            _linkageAnimation.Targets[i].Frames[_showFrameIndex].PositionDisabled = !_linkageAnimation.Targets[i].Frames[_showFrameIndex].PositionDisabled;
                        }
                        GUI.enabled = !_linkageAnimation.Targets[i].Frames[_showFrameIndex].PositionDisabled;
                        GUILayout.Label( "P：" );
                        _linkageAnimation.Targets[i].Frames[_showFrameIndex].Position = EditorGUILayout.Vector3Field("", _linkageAnimation.Targets[i].Frames[_showFrameIndex].Position);
                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space( 80 );
                        if( GUILayout.Button( "Get", "minibutton", GUILayout.Width( 30 ) ) )
                        {
                            if( _linkageAnimation.Targets[ i ].Target != null )
                                _linkageAnimation.Targets[ i ].Frames[ _showFrameIndex ].Rotation = _linkageAnimation.Targets[ i ].Target.localRotation.eulerAngles;
                        }
                        if( GUILayout.Button( "Forbid", "minibutton", GUILayout.Width( 50 ) ) )
                        {
                            _linkageAnimation.Targets[i].Frames[_showFrameIndex].RotationDisabled = !_linkageAnimation.Targets[i].Frames[_showFrameIndex].RotationDisabled;
                        }
                        GUI.enabled = !_linkageAnimation.Targets[ i ].Frames[ _showFrameIndex ].RotationDisabled;
                        GUILayout.Label( "R：" );
                        _linkageAnimation.Targets[i].Frames[_showFrameIndex].Rotation = EditorGUILayout.Vector3Field("", _linkageAnimation.Targets[i].Frames[_showFrameIndex].Rotation);
                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space( 80 );
                        if( GUILayout.Button( "Get", "minibutton", GUILayout.Width( 30 ) ) )
                        {
                            if( _linkageAnimation.Targets[ i ].Target != null )
                                _linkageAnimation.Targets[ i ].Frames[ _showFrameIndex ].Scale = _linkageAnimation.Targets[ i ].Target.localScale;
                        }
                        if( GUILayout.Button( "Forbid", "minibutton", GUILayout.Width( 50 ) ) )
                        {
                            _linkageAnimation.Targets[i].Frames[_showFrameIndex].ScaleDisabled = !_linkageAnimation.Targets[i].Frames[_showFrameIndex].ScaleDisabled;
                        }
                        GUI.enabled = !_linkageAnimation.Targets[ i ].Frames[ _showFrameIndex ].ScaleDisabled;
                        GUILayout.Label( "S：" );
                        _linkageAnimation.Targets[i].Frames[_showFrameIndex].Scale = EditorGUILayout.Vector3Field("", _linkageAnimation.Targets[i].Frames[_showFrameIndex].Scale);
                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            #endregion

            #region 删除动画帧
            if( _showFrameIndex >= 0 && _showFrameIndex < _linkageAnimation.FrameLength() )
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.cyan;
                if( GUILayout.Button( "Delete Frame", "LargeButton") )
                {
                    _linkageAnimation.RemoveAtFrame( _showFrameIndex );
                    _showFrameIndex = -1;
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion
        }
        #endregion
    }
}
