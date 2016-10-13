using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skin : MonoBehaviour 
{
    public static int MAX_BONE = 72;

    public SkinAsset skinAsset;
    public Material skinMat;
    public Transform rootBone;

    private List<Transform> bones = new List<Transform>();
    private Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
    private Mesh mesh;
    private MaterialPropertyBlock mpb;

    private Matrix4x4[] cachedBoneMatrices;
    private Vector3[] cachedVerts;

    private void Start()
    {
        CreateFromAsset(skinAsset);

        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = skinMat;
        mr.SetPropertyBlock(mpb);

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        mpb = new MaterialPropertyBlock();
    }

    private void CreateFromAsset(SkinAsset asset)
    {
        BuildBoneList();

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.name = "Mesh_" + asset.name;
        mesh.vertices = asset.vertices;
        mesh.normals = asset.normals;
        mesh.tangents = asset.tangents;
        mesh.colors = asset.colors;
        mesh.uv = asset.uv;
        mesh.triangles = asset.triangles;
        mesh.uv3 = asset.boneWeights1;
        mesh.uv4 = asset.boneWeights2;
        mesh.RecalculateBounds();

        cachedBoneMatrices = new Matrix4x4[asset.bones.Length];
        cachedVerts = new Vector3[asset.vertices.Length];
    }

    private Transform GetBone(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        Transform found;
        if (boneMap.TryGetValue(name, out found))
        {
            return found;
        }

        Debug.LogWarningFormat("Can't find bone {0} under {1}.", name, rootBone.name);
        return null;
    }

    private void BuildBoneList()
    {
        BuildBoneRecur(rootBone);
        bones.Clear();
        for (int i = 0; i < skinAsset.bones.Length; i++)
        {
            bones.Add(GetBone(skinAsset.bones[i]));
        }
    }

    private void BuildBoneRecur(Transform bone)
    {
        boneMap[bone.name] = bone;

        for (int i = 0; i < bone.childCount; i++)
        {
            Transform c = bone.GetChild(i);
            BuildBoneRecur(c);
        }
    }

    private void OnDestroy()
    {
        if (mesh != null)
        {
            Object.Destroy(mesh);
            mesh = null;
        }
    }

    private void UpdateMatrix()
    {
        for (int i = 0; i < cachedBoneMatrices.Length; i++)
        {
            Transform b = bones[i];
            cachedBoneMatrices[i] = transform.worldToLocalMatrix * b.localToWorldMatrix * skinAsset.bindPose[i];
        }
    }

    private void UpdateMesh()
    {
        for (int i = 0; i < cachedVerts.Length; i++)
        {
            int boneIndex0 = (int)(skinAsset.boneWeights1[i].x / 10);
            int boneIndex1 = (int)(skinAsset.boneWeights1[i].y / 10);
            int boneIndex2 = (int)(skinAsset.boneWeights2[i].x / 10);
            int boneIndex3 = (int)(skinAsset.boneWeights2[i].y / 10);

            float boneWeight0 = skinAsset.boneWeights1[i].x - boneIndex0 * 10;
            float boneWeight1 = skinAsset.boneWeights1[i].y - boneIndex1 * 10;
            float boneWeight2 = skinAsset.boneWeights2[i].x - boneIndex2 * 10;
            float boneWeight3 = skinAsset.boneWeights2[i].y - boneIndex3 * 10;

            Matrix4x4 bm0 = cachedBoneMatrices[boneIndex0];
            Matrix4x4 bm1 = cachedBoneMatrices[boneIndex1];
            Matrix4x4 bm2 = cachedBoneMatrices[boneIndex2];
            Matrix4x4 bm3 = cachedBoneMatrices[boneIndex3];

            Matrix4x4 vertexMatrix = new Matrix4x4();

            for (int n = 0; n < 16; n++)
            {
                vertexMatrix[n] =
                    bm0[n] * boneWeight0 +
                    bm1[n] * boneWeight1 +
                    bm2[n] * boneWeight2 +
                    bm3[n] * boneWeight3;
            }

            cachedVerts[i] = vertexMatrix.MultiplyPoint3x4(skinAsset.vertices[i]);
        }

        mesh.vertices = cachedVerts;
        mesh.RecalculateBounds();
    }

    private void LateUpdate()
    {
        UpdateMatrix();
        UpdateMesh();
    }
}
