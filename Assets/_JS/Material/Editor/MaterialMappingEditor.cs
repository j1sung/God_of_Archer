using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections.Generic;

public class MaterialMappingEditor : EditorWindow
{
    private MaterialMappingProfile _targetProfile;
    private GameObject _targetModel;

    [MenuItem("Tools/Material Mapping/Save Current Mapping")]
    public static void ShowWindow()
    {
        var window = GetWindow<MaterialMappingEditor>("Material Mapping");
        window.minSize = new Vector2(300, 120);
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        EditorGUILayout.LabelField("1) Profile (SO) to Save Into", EditorStyles.boldLabel);
        _targetProfile = (MaterialMappingProfile)EditorGUILayout.ObjectField(
            "Mapping Profile",
            _targetProfile,
            typeof(MaterialMappingProfile),
            false
        );

        GUILayout.Space(6);
        EditorGUILayout.LabelField("2) Model (GameObject) to Read From", EditorStyles.boldLabel);
        _targetModel = (GameObject)EditorGUILayout.ObjectField(
            "Model GameObject",
            _targetModel,
            typeof(GameObject),
            true
        );

        GUILayout.Space(10);
        GUI.enabled = (_targetProfile != null && _targetModel != null);
        if (GUILayout.Button("Save Current Mapping"))
        {
            SaveMappingToProfile();
        }
        GUI.enabled = true;
    }

    private void SaveMappingToProfile()
    {
        if (_targetProfile == null || _targetModel == null) return;

        // ������ �ִ� ��� MeshRenderer�� SkinnedMeshRenderer�� ��Ƽ�,
        // ����Ʈ ��� ��� ��� + ������ ��Ƽ���� ����Ʈ�� ����
        List<MaterialMappingProfile.Entry> entries = new List<MaterialMappingProfile.Entry>();

        // MeshRenderer ���
        foreach (var mr in _targetModel.GetComponentsInChildren<MeshRenderer>(true))
        {
            // ��Ʈ�������� ��� ��� ����
            string path = GetRelativePath(_targetModel.transform, mr.transform);

            // ù ��°(0��) ��Ƽ���� �����´ٰ� ����(���� ������ ������ �ݺ������� ������ ��)
            var mat = mr.sharedMaterial;
            if (mat != null)
            {
                entries.Add(new MaterialMappingProfile.Entry
                {
                    transformPath = path,
                    material = mat
                });
            }
        }

        // SkinnedMeshRenderer ���
        foreach (var smr in _targetModel.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            string path = GetRelativePath(_targetModel.transform, smr.transform);
            var mat = smr.sharedMaterial; // SkinnedMeshRenderer�� sharedMaterial�� ������ (���� �����̶� ����)
            if (mat != null)
            {
                entries.Add(new MaterialMappingProfile.Entry
                {
                    transformPath = path,
                    material = mat
                });
            }
        }

        // SO�� �����
        Undo.RecordObject(_targetProfile, "Save Material Mapping");
        _targetProfile.mappings = entries.ToArray();
        EditorUtility.SetDirty(_targetProfile);
        AssetDatabase.SaveAssets();

        Debug.Log($"[MaterialMapping] Saved {entries.Count} entries into '{_targetProfile.name}'.");
    }

    // rootTransform�������� targetTransform������ ��� ��θ� "A/B/C" ���·� ����
    private string GetRelativePath(Transform rootTransform, Transform targetTransform)
    {
        if (rootTransform == targetTransform)
            return "";
        List<string> parts = new List<string>();
        Transform cur = targetTransform;
        while (cur != null && cur != rootTransform)
        {
            parts.Add(cur.name);
            cur = cur.parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }
}