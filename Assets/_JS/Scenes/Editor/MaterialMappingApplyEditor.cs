using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[CustomEditor(typeof(MaterialMappingProfile))]
public class MaterialMappingApplyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(6);
        if (GUILayout.Button("Apply to Selected Models"))
        {
            ApplyToSelection();
        }
    }

    private void ApplyToSelection()
    {
        MaterialMappingProfile profile = (MaterialMappingProfile)target;
        if (profile.mappings == null || profile.mappings.Length == 0)
        {
            Debug.LogWarning("[MaterialMapping] Profile has no entries to apply.");
            return;
        }

        // ���õ� ��� GameObject�� ���� �ݺ�
        foreach (GameObject go in Selection.gameObjects)
        {
            ApplyProfileToModel(profile, go);
        }

        // ���� ���� ����
        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
    }

    private void ApplyProfileToModel(MaterialMappingProfile profile, GameObject root)
    {
        // ����, ���� �´��� ������ �˻�(��Ʈ�� ��� �ְų�, �������� �ϳ��� ������ ��ŵ)
        var anyRenderer = root.GetComponentInChildren<Renderer>(true);
        if (anyRenderer == null)
        {
            Debug.LogWarning($"[MaterialMapping] '{root.name}' has no Renderer component in children. Skipped.");
            return;
        }

        // ���� ��� MeshRenderer/SkinnedMeshRenderer�� �ѹ��� ������
        var meshRenderers = root.GetComponentsInChildren<MeshRenderer>(true);
        var skinnedRenders = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // ���ݺ��� SO�� ����� �� Entry(��Ρ��Ƽ����)�� ��ȸ
        foreach (var entry in profile.mappings)
        {
            // 1) entry.transformPath�� root �������� ã��
            Transform targetTransform = root.transform.Find(entry.transformPath);
            if (targetTransform == null)
            {
                Debug.LogWarning($"[MaterialMapping] Path '{entry.transformPath}' not found under '{root.name}'.");
                continue;
            }

            // 2) �ش� Transform ���� MeshRenderer �Ǵ� SkinnedMeshRenderer�� �پ� �ִ��� �˻�
            var mr = targetTransform.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Apply Material Mapping");
                mr.sharedMaterial = entry.material;
                continue;
            }

            var smr = targetTransform.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                Undo.RecordObject(smr, "Apply Material Mapping");
                smr.sharedMaterial = entry.material;
                continue;
            }

            Debug.LogWarning($"[MaterialMapping] No MeshRenderer/SkinnedMeshRenderer on '{entry.transformPath}' in '{root.name}'.");
        }

        Debug.Log($"[MaterialMapping] Applied profile '{profile.name}' to '{root.name}'.");
    }
}