using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridEditor
{
    [ExecuteInEditMode]
    public class LinkRenderer : MonoBehaviour
    {
        public Material material => renderer.material;

        [SerializeField] Material Material;
        public Vector3 Start;
        public Vector3 End;
        public Axis axis = Axis.Y;
        public float Width = 1;

        Transform trans;
        Mesh mesh;
        MeshFilter meshFilter;
        new MeshRenderer renderer;
        Material mat;
        Vector3 start, end;
        float width;
        Axis prevAxis;

        private void Awake()
        {
            trans = transform;
            start = Start;
            end = End;
            width = Width;
            prevAxis = axis;
            check();
        }

        private void OnEnable()
        {
            refresh();
        }

        private void Update()
        {
            if (mat != Material)
            {
                mat = Material;
                renderer.sharedMaterial = mat;
            }
            if (start != Start || end != End || width != Width || prevAxis != axis)
            {
                start = Start;
                end = End;
                width = Width;
                prevAxis = axis;
                refresh();
            }
        }

        public void SetGlobalMaterial(Material mat)
        {
            Material = mat;
        }

        public void SetPosition(Vector3 start, Vector3 end, bool worldSpace = false)
        {
            if (worldSpace)
            {
                start = trans.InverseTransformPoint(start);
                end = trans.InverseTransformPoint(end);
            }
            Start = start;
            End = end;
            refresh();
        }

        void check()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.HideAndDontSave;
            }
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.hideFlags = HideFlags.HideInInspector;
                meshFilter.sharedMesh = mesh;
            }
            if (renderer == null)
            {
                renderer = GetComponent<MeshRenderer>();
                if (renderer == null)
                    renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.hideFlags = HideFlags.HideInInspector;
            }
        }

        void refresh()
        {
            check();
            mesh.Clear();
            if (Vector3.Distance(start, end) == 0)
                return;
            Vector3 dir = Vector3.Normalize(end - start);
            Vector3 normal = Vector3.zero;
            switch (axis)
            {
                case Axis.X:
                    normal = trans.right;
                    break;
                case Axis.Y:
                    normal = trans.up;
                    break;
                case Axis.Z:
                    normal = trans.forward;
                    break;
            }
            if (Mathf.Abs(Vector3.Dot(normal, dir)) == 1)
            {
                switch (axis)
                {
                    case Axis.X:
                        normal = trans.up;
                        break;
                    case Axis.Y:
                        normal = trans.forward;
                        break;
                    case Axis.Z:
                        normal = trans.up;
                        break;
                }
            }
            Vector3 binormal = Vector3.Cross(normal, dir).normalized;
            Vector3 point0 = start + binormal * Width * .5f;
            Vector3 point1 = start - binormal * Width * .5f;
            Vector3 point2 = end + binormal * Width * .5f;
            Vector3 point3 = end - binormal * Width * .5f;
            normal = Vector3.Normalize(normal - dir * Vector3.Dot(normal, dir));
            mesh.SetVertices(new List<Vector3>() { point0, point1, point2, point3 });
            mesh.SetNormals(Enumerable.Repeat(normal, 4).ToArray());
            mesh.SetIndices(new int[] { 0, 1, 2, 1, 3, 2 }, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) });
            mesh.SetTangents(Enumerable.Repeat(new Vector4(binormal.x, binormal.y, binormal.z, 1), 4).ToArray());
        }

        public enum Axis { X, Y, Z }
    }
}