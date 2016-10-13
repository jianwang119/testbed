using UnityEngine;
using System.Collections;

public class SkinAsset : ScriptableObject
{
    public string rootBone;
    public string[] bones;
    public Matrix4x4[] bindPose;
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector4[] tangents;
    public Vector2[] uv;
    public int[] triangles;
    public Color[] colors;
    public Vector2[] boneWeights1;
    public Vector2[] boneWeights2;
}