using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static lLCroweTool.lLcroweUtil.HexTileMatrix;

namespace lLCroweTool.Test
{
    public class CreateTestHexTileRender : MonoBehaviour
    {
        public List<Face> faceList = new List<Face>();
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Mesh mesh;
        public Material material;

        public HexTileType hexTileType;
        public CreateTileAxisType createTileAxisType;
        public float outterSize;
        public float innerSize;
        public float height;


        protected virtual void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            mesh = new Mesh();
            mesh.name = "HexTile";

            meshFilter.mesh = mesh;
            meshRenderer.material = material;

           
            DrawMesh();
        }


        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                DrawMesh();
            }
        }
        public void DrawMesh()
        {
            DrawFace();
            CombineFaces();
        }
        public Face CreateFace(float innerRadian, float outterRadian, float heightA, float heightB, int index, bool reverse = false)
        {
            Vector3 pointA = GetHexTilePoint(Vector3.zero, innerRadian, heightB, index, hexTileType, createTileAxisType);
            Vector3 pointB = GetHexTilePoint(Vector3.zero, innerRadian, heightB, (index < 5) ? index + 1 : 0, hexTileType, createTileAxisType);
            Vector3 pointC = GetHexTilePoint(Vector3.zero, outterRadian, heightA, (index < 5) ? index + 1 : 0, hexTileType, createTileAxisType);
            Vector3 pointD = GetHexTilePoint(Vector3.zero, outterRadian, heightA, index, hexTileType, createTileAxisType);

            //Vector3 pointA = GetPoint(innerRadian, heightB, index);
            //Vector3 pointB = GetPoint(innerRadian, heightB, (index < 5) ? index + 1 : 0);
            //Vector3 pointC = GetPoint(outterRadian, heightA, (index < 5) ? index + 1 : 0);
            //Vector3 pointD = GetPoint(outterRadian, heightA, index);


            var vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
            var triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
            var uvs = new List<Vector2>() { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };



            if (reverse)
            {
                vertices.Reverse();
            }


            return new Face(vertices, triangles, uvs);
        }

        public Vector3 GetPoint(float size, float height, int index)
        {
            var angle_deg = 60 * index;
            var angle_rad = Mathf.PI / 180 * angle_deg;
            return new Vector3(size * Mathf.Cos(angle_rad), height, size * Mathf.Sin(angle_rad));
        }


        public void DrawFace()
        {
            faceList = new List<Face>();

            //윗면그리기
            for (int i = 0; i < 6; i++)
            {
                faceList.Add(CreateFace(innerSize, outterSize, height * 0.5f, height * 0.5f, i));
            }

            //아랫면 그리기
            for (int i = 0; i < 6; i++)
            {
                faceList.Add(CreateFace(innerSize, outterSize, height * 0.5f, -height * 0.5f, i, true));
            }

            //바깥쪽
            for (int i = 0; i < 6; i++)
            {
                faceList.Add(CreateFace(outterSize, outterSize, height * 0.5f, -height * 0.5f, i, true));
            }

            //안쪽
            for (int i = 0; i < 6; i++)
            {
                faceList.Add(CreateFace(innerSize, innerSize, height * 0.5f, -height * 0.5f, i, false));
            }

        }
        public void CombineFaces()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();


            for (int i = 0; i < faceList.Count; i++)
            {
                //점추가
                vertices.AddRange(faceList[i].vertices);
                uvs.AddRange(faceList[i].uvs);

                //삼각형만들기
                int offset = (4 * i);

                for (int j = 0; j < faceList[i].triangles.Count; j++)
                {
                    triangles.Add(faceList[i].triangles[j] + offset);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
        }


        public struct Face
        {
            public List<Vector3> vertices { get; private set; }
            public List<int> triangles { get; private set; }
            public List<Vector2> uvs { get; private set; }

            public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
            {
                this.vertices = vertices;
                this.triangles = triangles;
                this.uvs = uvs;
            }

        }

    }
}