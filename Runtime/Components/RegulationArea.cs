using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;

namespace LandscapeDesignTool
{
    public class RegulationArea : MonoBehaviour
    {
        [SerializeField] float areaHeight = 10;
        [SerializeField] List<Vector3> vertices = new List<Vector3>();
        [SerializeField] Color AreaColor;

        public List<Vector2> _Contours;
        public List<Vector2> Vertexes = new List<Vector2>();

        public List<Vector3> Vertices => vertices;
        public bool IsMeshGenerated => GetComponent<MeshFilter>() != null;

        public List<Vector2> GetVertex2D()
        {
            List<Vector2> lst = new List<Vector2>();
            foreach (Vector3 v in vertices)
            {
                lst.Add(new Vector2(v.x, v.z));
            }
            return lst;
        }
        public float GetHeight()
        {
            return areaHeight;
        }

        public void SetHeight(float h)
        {
            areaHeight = h;
            GenMesh();
        }

        public Color GetAreaColor()
        {
            return AreaColor;
        }
        public void SetAreaColor(Color c)
        {
            AreaColor = c;
        }

        public void AddVertex(Vector3 vertex)
        {
            vertices.Add(vertex);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                Vector3 val0 = Vertices[i];
                Vector3 v0 = new Vector3(val0.x, val0.y + 1, val0.z);
                Vector3 val1 = Vertices[i + 1];
                Vector3 v1 = new Vector3(val1.x, val1.y + 1, val1.z);

                Gizmos.DrawLine(v0, v1);
            }

            if (Vertices.Count > 3 && IsMeshGenerated)
            {
                Vector3 val0 = Vertices[Vertices.Count - 1];
                Vector3 v0 = new Vector3(val0.x, val0.y + 1, val0.z);
                Vector3 val1 = Vertices[0];
                Vector3 v1 = new Vector3(val1.x, val1.y + 1, val1.z);

                Gizmos.DrawLine(v0, v1);
            }
        }

        public void GenMesh()
        {
            if (vertices.Count < 3)
                return;

            UpdateContour();

            var mesh = GenerateMeshFromContour();

            var mr = gameObject.GetComponent<MeshRenderer>();
            if (mr == null)
                mr = gameObject.AddComponent<MeshRenderer>();

            Material material = LDTTools.MakeMaterial(AreaColor);

            mr.sharedMaterial = material;

            var mf = gameObject.GetComponent<MeshFilter>();
            if (mf == null)
                mf = gameObject.AddComponent<MeshFilter>();

            mf.mesh = mesh;

            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = mesh;

            mesh.RecalculateBounds();
        }

        private void UpdateContour()
        {
            _Contours = new List<Vector2>();

            {
                int i = 0;
                foreach (Vector3 v3 in vertices)
                {
                    Vector3 v0 = v3;
                    Vector2 cont = new Vector2(v3.x, v3.z);
                    _Contours.Add(cont);
                    Vertexes.Add(cont);

                    Vector3 v1;
                    if (i < vertices.Count - 1)
                    {
                        v1 = vertices[i + 1];
                    }
                    else
                    {
                        v1 = vertices[0];
                    }
                    float length = Vector3.Distance(v0, v1);
                    int d = (int)(length / 3.0f);

                    float dx = (v1.x - v0.x) / (float)d;
                    float dy = (v1.z - v0.z) / (float)d;

                    for (int j = 1; j < d; j++)
                    {
                        float x = v0.x + dx * (float)j;
                        float y = v0.z + dy * (float)j;
                        Vector2 v2 = new Vector2(x, y);
                        _Contours.Add(v2);
                    }

                    i++;
                }
            }
        }

        private Mesh GenerateMeshFromContour()
        {
            Polygon poly = new Polygon();
            poly.Add(_Contours);
            var triangleNetMesh = (TriangleNetMesh)poly.Triangulate();

            var mesh = triangleNetMesh.GenerateUnityMesh();

            Vector3[] nv = new Vector3[mesh.vertices.Length * 2];

            int vco = mesh.vertices.Length;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Vector3 ov = mesh.vertices[i];
                Vector3 rayOrigin = new Vector3(ov.x, 100000, ov.y);
                var ray = new Ray(rayOrigin, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
                {
                    nv[i] = new Vector3(ov.x, hit.point.y + areaHeight, ov.y);
                    nv[i + mesh.vertices.Length] = new Vector3(ov.x, hit.point.y, ov.y);
                }
                else
                {
                    nv[i] = new Vector3(ov.x, 0, ov.y);
                    nv[i + mesh.vertices.Length] = new Vector3(ov.x, 0, ov.y);
                }
            }

            int[] triangles = new int[(_Contours.Count + 1) * 2 * 3 + mesh.triangles.Length];
            {
                int i = 0;
                foreach (int idx in mesh.triangles)
                {
                    triangles[i] = idx;
                    i++;
                }

                int c1 = _Contours.Count - 1;
                int n = mesh.triangles.Length;
                for (int count = 0; count < c1; count++)
                {
                    int k1 = count;
                    int k2 = count + vco;
                    int k3 = k1 + 1;
                    int k4 = k2 + 1;

                    // TODO: 外形が右回りの場合側面のメッシュが内向きになってしまう問題の解消
                    triangles[n++] = k1;
                    triangles[n++] = k3;
                    triangles[n++] = k4;
                    triangles[n++] = k1;
                    triangles[n++] = k4;
                    triangles[n++] = k2;
                }
                triangles[n++] = vco - 1;
                triangles[n++] = 0;
                triangles[n++] = vco;
                triangles[n++] = vco - 1;
                triangles[n++] = vco;
                triangles[n++] = vco * 2 - 1;
            }

            mesh.vertices = nv;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}