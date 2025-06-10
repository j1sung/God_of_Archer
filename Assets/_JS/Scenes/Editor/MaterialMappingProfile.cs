// Assets/Scripts/MaterialMapping/MaterialMappingProfile.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialMappingProfile", menuName = "Material Mapping/Profile")]
public class MaterialMappingProfile : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        // ����Ʈ �� �ٷ� �Ʒ��������� �����ϴ� ��Ρ�
        // ��) "Body/Torso", "Body/LeftArm/Shoulder" ���� ������ ���
        public string transformPath;

        // ������ ��Ƽ���� ����
        public Material material;
    }

    // ���� ���� (��� �� ��Ƽ����) ����
    public Entry[] mappings;
}