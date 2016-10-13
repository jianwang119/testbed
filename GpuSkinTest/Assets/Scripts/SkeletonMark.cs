using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkeletonMark : MonoBehaviour 
{
    [System.Serializable]
    public class BoneMark
    {
        public string name;
        public Transform trans;
        public string parent;

        public BoneMark(string name, Transform b, BoneMark p)
        {
            this.name = name;
            this.trans = b;
            this.parent = p==null?null:p.name;
        }
    }

    [SerializeField]
    private List<BoneMark> bones = new List<BoneMark>();

    private Dictionary<string, BoneMark> boneMap = new Dictionary<string, BoneMark>();

    public BoneMark GetBone(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        
        BoneMark found;
        if (boneMap.TryGetValue(name, out found))
        {
            return found;
        }

        for (int i = 0; i < bones.Count; i++)
        {
            if (bones[i].name == name)
            {
                return bones[i];
            }
        }

        Debug.LogWarningFormat("Can't find bone {0} under {1}.", name, gameObject.name);
        return null;
    }

    private void Awake()
    {
        if (bones.Count == 0)
        {
            BuildBoneList();
        }

        if (bones.Count > 0)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                BoneMark b = bones[i];
                boneMap[b.name] = b;
            }
        }
    }

    private void BuildBoneList()
    {
        BoneMark b = new BoneMark(transform.name, transform, null);
        bones.Add(b);
        BuildBoneRecur(b);
    }

    private void BuildBoneRecur(BoneMark parent)
    {
        for (int i = 0; i < parent.trans.childCount; i++)
        {
            Transform c = parent.trans.GetChild(i);
            BoneMark b = new BoneMark(c.name, c, parent);
            bones.Add(b);
            BuildBoneRecur(b);
        }
    }

    private void OnDrawGizmos()
    {
        Color old = Gizmos.color;
        for (int i = 0; i < bones.Count; i++)
        {
            BoneMark b = bones[i];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(b.trans.position, 0.01f);

            if (!string.IsNullOrEmpty(b.parent))
            {
                BoneMark p = GetBone(b.parent);
                if (p != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(p.trans.position, b.trans.position);
                }
            }
        }
        Gizmos.color = old;
    }
}
