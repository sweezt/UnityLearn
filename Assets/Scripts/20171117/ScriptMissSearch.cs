using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


public class ScriptMissSearch : EditorWindow
{
    public class EmptyComponentPropertyInfo
    {
        public GameObject TargetGameObject;

        public List<int> EmptyPropertyIndexes;

        private readonly SerializedObject m_gameSerializedObject;

        private readonly SerializedProperty m_componetPropertys;

        public EmptyComponentPropertyInfo(GameObject gameObject, string porpertyName)
        {
            this.TargetGameObject = gameObject;
            this.m_gameSerializedObject = new SerializedObject(this.TargetGameObject);
            this.m_componetPropertys = this.m_gameSerializedObject.FindProperty(porpertyName);
            this.EmptyPropertyIndexes = new List<int>();
        }

        public void DeletEmptyScript()
        {
            int i = 0;
            int count = this.EmptyPropertyIndexes.Count;
            while (i < count)
            {
                this.m_componetPropertys.DeleteArrayElementAtIndex(this.EmptyPropertyIndexes[i]);
                i++;
            }
            this.m_gameSerializedObject.ApplyModifiedProperties();
        }
    }

    private Vector2 m_scroll;

    private List<GameObject> m_gameObjects = new List<GameObject>();

    private readonly List<ScriptMissSearch.EmptyComponentPropertyInfo> m_componentPropertyInfos = new List<ScriptMissSearch.EmptyComponentPropertyInfo>();

    private readonly List<GameObject> m_emptyGameObjectsList = new List<GameObject>();

    private readonly List<string> m_emptyList = new List<string>();

    [MenuItem("NewcomeLib/搜索/脚本丢失搜索")]
    public static void AddWindow()
    {
        EditorWindow.GetWindowWithRect<ScriptMissSearch>(new Rect(0f, 0f, 800f, 500f), true, "SearchMissScriptGameObject");
    }

    protected void OnEnable()
    {
        this.m_gameObjects = FindObjectsOfType<GameObject>().ToList<GameObject>();
    }

    protected void OnGUI()
    {
        bool flag = GUILayout.Button("Seach", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
        if (flag)
        {
            this.SearchNoScriptGameobjectAndName();
        }
        this.ShowEmptyList();
        bool flag2 = GUILayout.Button("Clear miss script", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
        if (flag2)
        {
            int i = 0;
            int count = this.m_componentPropertyInfos.Count;
            while (i < count)
            {
                this.m_componentPropertyInfos[i].DeletEmptyScript();
                i++;
            }
            AssetDatabase.Refresh();
        }
        base.Repaint();
    }

    protected void OnHierarchyChange()
    {
        this.m_gameObjects = FindObjectsOfType<GameObject>().ToList<GameObject>();
        this.SearchNoScriptGameobjectAndName();
    }

    private string PathBuild(GameObject gameObject)
    {
        GameObject gameObject2 = gameObject;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(gameObject2.name);
        while (gameObject2.transform.parent != null)
        {
            gameObject2 = gameObject2.transform.parent.gameObject;
            stringBuilder.Insert(0, "/");
            stringBuilder.Insert(0, gameObject2.name);
        }
        return stringBuilder.ToString();
    }

    private void ShowEmptyList()
    {
        this.m_scroll = EditorGUILayout.BeginScrollView(this.m_scroll, new GUILayoutOption[0]);
        int i = 0;
        int count = this.m_emptyList.Count;
        while (i < count)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.LabelField("MissScriptGameObject : ", this.m_emptyList[i], new GUILayoutOption[0]);
            EditorGUILayout.ObjectField(":", this.m_emptyGameObjectsList[i], typeof(GameObject), true, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            i++;
        }
        EditorGUILayout.EndScrollView();
    }

    private void SearchNoScriptGameobjectAndName()
    {
        this.m_emptyGameObjectsList.Clear();
        this.m_emptyList.Clear();
        this.m_componentPropertyInfos.Clear();
        int i = 0;
        int count = this.m_gameObjects.Count;
        while (i < count)
        {
            Component[] components = this.m_gameObjects[i].GetComponents<Component>();
            ScriptMissSearch.EmptyComponentPropertyInfo emptyComponentPropertyInfo = new ScriptMissSearch.EmptyComponentPropertyInfo(this.m_gameObjects[i], "m_Component");
            int j = 0;
            int num = components.Length;
            while (j < num)
            {
                bool flag = components[j] == null;
                if (flag)
                {
                    bool flag2 = !this.m_componentPropertyInfos.Contains(emptyComponentPropertyInfo);
                    if (flag2)
                    {
                        this.m_componentPropertyInfos.Add(emptyComponentPropertyInfo);
                    }
                    emptyComponentPropertyInfo.EmptyPropertyIndexes.Add(j);
                    this.m_emptyGameObjectsList.Add(this.m_gameObjects[i]);
                    this.m_emptyList.Add(this.PathBuild(this.m_gameObjects[i]));
                    break;
                }
                j++;
            }
            i++;
        }
        Selection.instanceIDs = this.GenerateSelecionIndexArray(this.m_emptyGameObjectsList);
    }

    private int[] GenerateSelecionIndexArray(List<GameObject> list)
    {
        List<int> list2 = new List<int>();
        int i = 0;
        int count = list.Count;
        while (i < count)
        {
            list2.Add(list[i].GetInstanceID());
            i++;
        }
        return list2.ToArray();
    }
}

