using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(rectTest))]
public class MyEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        rectTest rectest = (rectTest)target;
        rectest.mRectValue = EditorGUILayout.RectField("customValue", rectest.mRectValue);
        rectest.texture = EditorGUILayout.ObjectField("图片", rectest.texture, typeof(Texture), true) as Texture;
    }
}
