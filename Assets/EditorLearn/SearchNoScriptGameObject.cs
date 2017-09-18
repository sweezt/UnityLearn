using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SearchNoScriptGameObject : EditorWindow
{
    public class EmptyComponentPropertyInfo
    {
        public GameObject targetGameObject;
        public List<int> emptyPropertyIndexes; 

        private SerializedObject m_gameSerializedObject;
        private SerializedProperty m_componetPropertys;

        public EmptyComponentPropertyInfo(GameObject gameObject, string porpertyName)
        {
            targetGameObject = gameObject;
            m_gameSerializedObject = new SerializedObject(targetGameObject);
            m_componetPropertys = m_gameSerializedObject.FindProperty(porpertyName);

            emptyPropertyIndexes = new List<int>();
        }

        public void DeletEmptyScript()
        {
            for (int i = 0, count = emptyPropertyIndexes.Count; i < count; i++)
            {
                m_componetPropertys.DeleteArrayElementAtIndex(emptyPropertyIndexes[i]);
            }

            m_gameSerializedObject.ApplyModifiedProperties();
        }
    }

    [MenuItem("Tools/SearchMissScriptGameObject")]
    public static void AddWindow()
    {
        GetWindowWithRect<SearchNoScriptGameObject>(new Rect(0, 0, 800, 500), true, "SearchMissScriptGameObject");
    }

    private Vector2 m_scroll;

    private List<GameObject> m_gameObjects = new List<GameObject>();
    private List<EmptyComponentPropertyInfo> m_componentPropertyInfos = new List<EmptyComponentPropertyInfo>();
    private List<GameObject> m_emptyGameObjectsList = new List<GameObject>();
    private List<string> m_emptyList = new List<string>();

    protected void OnEnable()
    {
        m_gameObjects = FindObjectsOfType<GameObject>().ToList();
    }

    protected void OnGUI()
    {
        if (GUILayout.Button("Seach", GUILayout.ExpandWidth(true)))
        {
            SearchNoScriptGameobjectAndName();
        }

        ShowEmptyList();

        if (GUILayout.Button("Clear miss script", GUILayout.ExpandWidth(true)))
        {
            for (int i = 0, count = m_componentPropertyInfos.Count; i < count; i++)
            {
                m_componentPropertyInfos[i].DeletEmptyScript();
            }

            AssetDatabase.Refresh();
        }

        Repaint();
    }

    protected void OnHierarchyChange()
    {
        m_gameObjects = FindObjectsOfType<GameObject>().ToList();

        SearchNoScriptGameobjectAndName();
    }

    private string PathBuild(GameObject gameObject)
    {
        GameObject temp = gameObject;
        StringBuilder path = new StringBuilder();

        path.Append(temp.name);

        while (temp.transform.parent != null)
        {
            temp = temp.transform.parent.gameObject;
            path.Insert(0, "/");
            path.Insert(0, temp.name);
        }

        return path.ToString();
    }

    private void ShowEmptyList()
    {
        m_scroll = EditorGUILayout.BeginScrollView(m_scroll);

        for (int i = 0, count = m_emptyList.Count; i < count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MissScriptGameObject : ", m_emptyList[i]);
            EditorGUILayout.ObjectField(":", m_emptyGameObjectsList[i], typeof (GameObject), true);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void SearchNoScriptGameobjectAndName()
    {
        m_emptyGameObjectsList.Clear();
        m_emptyList.Clear();
        m_componentPropertyInfos.Clear();

        for (int i = 0, count = m_gameObjects.Count; i < count; i++)
        {
            Component[] components = m_gameObjects[i].GetComponents<Component>();
            EmptyComponentPropertyInfo temp = new EmptyComponentPropertyInfo(m_gameObjects[i], "m_Component");

            for (int j = 0, componentsCount = components.Length; j < componentsCount; j++)
            {
                if (components[j] == null)
                {
                    if (!m_componentPropertyInfos.Contains(temp))
                    {
                        m_componentPropertyInfos.Add(temp);
                    }

                    temp.emptyPropertyIndexes.Add(j); 
                    
                    m_emptyGameObjectsList.Add(m_gameObjects[i]);
                    m_emptyList.Add(PathBuild(m_gameObjects[i]));
                  
                    break;
                }
            }
        }

        Selection.instanceIDs = GenerateSelecionIndexArray(m_emptyGameObjectsList);
    }

    private int[] GenerateSelecionIndexArray(List<GameObject> list)
    {
        List<int> indexList = new List<int>();

        for (int i = 0, count = list.Count; i < count; i++)
        {
            indexList.Add(list[i].GetInstanceID());
        }

        return indexList.ToArray();
    }
}
