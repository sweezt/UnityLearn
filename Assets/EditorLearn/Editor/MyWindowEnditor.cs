using NUnit.Framework.Internal.Execution;
using UnityEditor;
using UnityEngine;

public class MyWindowEnditor : EditorWindow 
{
    [MenuItem("Window/MyWindow")]
    private static void Addwindow()
    {
        Rect rt = new Rect(0, 0, 500, 500);
        GetWindowWithRect<MyWindowEnditor>(rt, true, "window name");
    }

    private string m_text;
    private Texture m_texture;

    private void OnGUI()
    {
        m_text = EditorGUILayout.TextField("输入文字：", m_text);
        EditorGUILayout.TextArea(m_text, GUILayout.Height(60));
        if (GUILayout.Button("打开通知", GUILayout.Width(200)))
        {
            ShowNotification(new GUIContent("this is a notification"));
        }

        if (GUILayout.Button("关闭通知", GUILayout.Width(200)))
        {
            RemoveNotification();
        }

        EditorGUILayout.LabelField("鼠标在窗口位置", Event.current.mousePosition.ToString());

        m_texture = EditorGUILayout.ObjectField("贴图",m_texture, typeof (Texture), true) as Texture;

        if (GUILayout.Button("关闭窗口", GUILayout.Width(200)))
        {
            Close();
        }
    }

    private void Update()
    {
    }

    private void OnFocus()
    {
        Debug.Log("OnFocus");
    }

    private void OnLostFocus()
    {
        Debug.Log("OnLostFocus");
    }

    private void OnHierachyChange()
    {
        Debug.Log("OnHierachyChange");
    }

    private void OnInsepctorUpdate()
    {
        this.Repaint();
    }

    private void OnSelectionChange()
    {
        foreach (Transform t in Selection.transforms)
        {
            Debug.Log("OnSelectionChange" + t.name);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
