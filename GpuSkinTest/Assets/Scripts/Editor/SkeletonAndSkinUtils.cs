using UnityEngine;
using System.Collections;
using UnityEditor;

public class SkeletonAndSkinUtils
{
    [MenuItem("Assets/Tools/ExportSkin")]
    public static void ExportSkin()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            Debug.LogWarning("Select SkinnedMeshRenderer first.");
            return;
        }

        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogWarning("Select SkinnedMeshRenderer first.");
            return;
        }

        if (smr.bones.Length > Skin.MAX_BONE)
        {
            Debug.LogWarningFormat("Too many bones {0}, limits to {1}.", smr.bones.Length, Skin.MAX_BONE);
            return;
        }

        // write skin
        SkinAsset asset = CreateSkinAsset(smr);
        string name = string.Format("Assets/{0}_skin.asset", go.name);
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private static T[] NewArray<T>(T[] src)
    {
        T[] ret = new T[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            ret[i] = src[i];
        }
        return ret;
    }

    private static SkinAsset CreateSkinAsset(SkinnedMeshRenderer smr)
    {
        SkinAsset asset = ScriptableObject.CreateInstance<SkinAsset>();

        Mesh mesh = smr.sharedMesh;

        asset.rootBone = smr.rootBone.name;
        asset.bones = new string[smr.bones.Length];
        for (int i = 0; i < smr.bones.Length; i++)
        {
            asset.bones[i] = smr.bones[i].name;
        }
        asset.bindPose = NewArray<Matrix4x4>(mesh.bindposes);
        asset.vertices = NewArray<Vector3>(mesh.vertices);
        asset.normals = NewArray<Vector3>(mesh.normals);
        asset.tangents = NewArray<Vector4>(mesh.tangents);
        asset.uv = NewArray<Vector2>(mesh.uv);
        asset.triangles = NewArray<int>(mesh.triangles);
        asset.colors = NewArray<Color>(mesh.colors);

        BoneWeight[] boneWeights = mesh.boneWeights;
        asset.boneWeights1 = new Vector2[boneWeights.Length];
        asset.boneWeights2 = new Vector2[boneWeights.Length];
        for (int i = 0; i < boneWeights.Length; i++)
        {
            BoneWeight bw = mesh.boneWeights[i];
            asset.boneWeights1[i].x = (bw.boneIndex0 * 10) + bw.weight0;
            asset.boneWeights1[i].y = (bw.boneIndex1 * 10) + bw.weight1;
            asset.boneWeights2[i].x = (bw.boneIndex2 * 10) + bw.weight2;
            asset.boneWeights2[i].y = (bw.boneIndex3 * 10) + bw.weight3;
        }
        return asset;
    }
}
