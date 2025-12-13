using System.Collections.Generic;
using UnityEngine;

namespace Mesh
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PolygonMeshFromTransforms : MonoBehaviour
    {
        public List<Transform> points = new List<Transform>();

        [ContextMenu("Generate Mesh")]
        public void GenerateMesh()
        {
            if (points.Count < 3)
            {
                Debug.LogError("Need at least 3 points to generate a polygon.");
                return;
            }

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            mesh.name = "Generated Polygon Mesh";

            // Convert transforms to local-space vertices
            Vector3[] vertices = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = transform.InverseTransformPoint(points[i].position);
            }

            // Triangulate
            int[] triangles = Triangulate(vertices);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        // -----------------------------
        // Ear Clipping Triangulation
        // -----------------------------
        int[] Triangulate(Vector3[] verts)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < verts.Length; i++)
                indices.Add(i);

            List<int> triangles = new List<int>();

            int guard = 0;
            while (indices.Count > 3 && guard < 5000)
            {
                guard++;

                for (int i = 0; i < indices.Count; i++)
                {
                    int prev = indices[(i - 1 + indices.Count) % indices.Count];
                    int curr = indices[i];
                    int next = indices[(i + 1) % indices.Count];

                    if (IsEar(prev, curr, next, indices, verts))
                    {
                        triangles.Add(prev);
                        triangles.Add(next);
                        triangles.Add(curr);
                        indices.RemoveAt(i);
                        break;
                    }
                }
            }

            // Last triangle
            if (indices.Count == 3)
            {
                triangles.Add(indices[0]);
                triangles.Add(indices[1]);
                triangles.Add(indices[2]);
            }
            
            Debug.Log($"Triangles count: {triangles.Count / 3}");

            return triangles.ToArray();
        }

        bool IsEar(int a, int b, int c, List<int> indices, Vector3[] verts)
        {
            Vector2 A = ToXZ(verts[a]);
            Vector2 B = ToXZ(verts[b]);
            Vector2 C = ToXZ(verts[c]);

            // Convex check (CCW)
            if (Cross(B - A, C - B) <= 0)
                return false;

            // Check if any other point is inside the triangle
            for (int i = 0; i < indices.Count; i++)
            {
                int p = indices[i];
                if (p == a || p == b || p == c) continue;

                if (PointInTriangle(ToXZ(verts[p]), A, B, C))
                    return false;
            }

            return true;
        }
        
        float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        Vector2 ToXZ(Vector3 v) => new Vector2(v.x, v.z);

        bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float area = TriangleArea(a, b, c);
            float a1 = TriangleArea(p, b, c);
            float a2 = TriangleArea(a, p, c);
            float a3 = TriangleArea(a, b, p);
            return Mathf.Abs(area - (a1 + a2 + a3)) < 0.001f;
        }

        float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs((a.x * (b.y - c.y) +
                              b.x * (c.y - a.y) +
                              c.x * (a.y - b.y)) * 0.5f);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == null) continue;

                Gizmos.DrawSphere(points[i].position, 0.05f);

                int next = (i + 1) % points.Count;
                if (points[next] != null)
                    Gizmos.DrawLine(points[i].position, points[next].position);
            }
        }
    }
}
