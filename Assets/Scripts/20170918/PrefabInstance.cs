using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;


[ExecuteInEditMode]
public class PrefabInstance : MonoBehaviour
{
    public GameObject prefab;

#if UNITY_EDITOR
    public struct Thingy
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public List<Material> materials;
    }

    [NonSerializedAttribute]
    public List<Thingy> things = new List<Thingy>();

    protected void OnValidate()
    {
        things.Clear();

        if (enabled)
        {
            Rebuild(prefab, Matrix4x4.identity);
        }
    }

    protected void OnEnale()
    {
        things.Clear();

        if (enabled)
        {
            Rebuild(prefab, Matrix4x4.identity);
        }
    }

    private void Rebuild(GameObject source, Matrix4x4 inMatrix)
    {
        if (!source)
        {
            return;
        }

        Matrix4x4 baseMat = inMatrix*Matrix4x4.TRS(-source.transform.position, Quaternion.identity, Vector3.one);

        foreach (Renderer item in source.GetComponentsInChildren<Renderer>(true))
        {
            things.Add(new Thingy()
            {
                mesh = item.GetComponent<MeshFilter>().sharedMesh,
                matrix = baseMat * item.transform.localToWorldMatrix,
                materials = new List<Material>(item.sharedMaterials)
            });
        }

        foreach (PrefabInstance item in source.GetComponentsInChildren<PrefabInstance>(true))
        {
            if (item.enabled && item.gameObject.activeSelf)
            {
                Rebuild(item.prefab, baseMat * item.transform.localToWorldMatrix);
            }
        }
    }

    protected void Update()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }

        Matrix4x4 mat = transform.localToWorldMatrix;

        foreach (Thingy item in things)
        {
            for (int i = 0, count = item.materials.Count; i < count; i++)
            {
                Graphics.DrawMesh(item.mesh, mat * item.matrix, item.materials[i], gameObject.layer, null, i);
            }
        }
    }

    protected void DrawGizmos(Color col)
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }

        Gizmos.color = col;
        Matrix4x4 mat = transform.localToWorldMatrix;

        foreach (Thingy item in things)
        {
            Gizmos.matrix = mat*item.matrix;
            Gizmos.DrawCube(item.mesh.bounds.center, item.mesh.bounds.size);
        }
    }

    [PostProcessScene(-2)]
    public static void OnPostprocessScene()
    {
        foreach (PrefabInstance item in FindObjectsOfType(typeof(PrefabInstance)))
        {
            BakeInstance(item);
        }
    }

    private static void BakeInstance(PrefabInstance pi)
    {
        if (!pi.prefab || !pi.enabled)
        {
            return;
        }

        pi.enabled = false;

        GameObject go = PrefabUtility.InstantiatePrefab(pi.prefab) as GameObject;

        Quaternion rot = go.transform.localRotation;
        Vector3 scale = go.transform.localScale;
        go.transform.parent = pi.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = scale;
        go.transform.localRotation = rot;

        pi.prefab = null;

        foreach (PrefabInstance item in go.GetComponentsInChildren<PrefabInstance>())
        {
            BakeInstance(item);
        }
#endif
    }
}
