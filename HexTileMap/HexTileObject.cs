using UnityEditor;
using UnityEngine;
using static lLCroweTool.lLcroweUtil.HexTileMatrix;

namespace lLCroweTool.TileMap.HexTileMap
{   
    public class HexTileObject : MonoBehaviour
    {
        //헥스타일오브젝트 : 헥스타일 체크와 랜더러 기능이 존재        

        //헥스타일이 될 대상에 집어넣기
        //이걸 헥스타일로 처리하는게 맞아보임

        //클래스쪽에 집어넣는게 맞을려나
        //클래스에 있는걸 다옮겨야할듯한데

        //20230418 체크하기
        //20230420 작업하기 전 한번더 체크                        
        
        public UnitObject_Base targetBatchObject;//배치된 오브젝트(유닛)//타일맵에 미리 배치된 오브젝트(유닛&장애물)

        /// <summary>
        /// 배치된 유닛오브젝트
        /// </summary>
        public UnitObject_Base BatchUnitObject
        {
            get => targetBatchObject; 
            set
            {
                targetBatchObject = value;
                hexTileData.IsExistObject = targetBatchObject == null ? false : true;
            }
        }

        public HexTileData hexTileData = new HexTileData();
        public HexTileData GetHexTileData { get => hexTileData; }

        //컴포넌트
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;

        //랜더러관련//분리시킨거 합침
        [Header("랜더러관련")]
        public float hexTileSize = 1f;

        public HexTileType hexTileType;
        public CreateTileAxisType createTileAxisType;

        public Material material;//=>적용할 메터리얼
        public Mesh mesh;


        //헥사타일 콜라이더용에디터처리
        private void Awake()
        {
            meshRenderer = lLcroweUtil.GetAddComponent<MeshRenderer>(gameObject);
            meshCollider = lLcroweUtil.GetAddComponent<MeshCollider>(gameObject);
        }

        public void UpdateHexTileData()
        {
            //LayerMask layerMask
            //콜라이더체크//이거 에디터에서처리해주면 되지않나//자동생성을 하지않는이상구지?
            //bool checkObstacle = Physics.BoxCast(transform.position, Vector2.one * 0.5f, Vector2.up, transform.rotation, 0, layerMask);

            BatchUnitObject = targetBatchObject;
            //meshCollider.enabled = hexTileData.IsBatchTile;//이거 배치할때 쓸려고 만든거네
        }

        /// <summary>
        /// 헥스타일랜더러 초기화
        /// </summary>
        /// <param name="size">크기</param>
        /// <param name="tileType">타일타입</param>
        /// <param name="axisType">축타입</param>
        /// <param name="material">메터리얼</param>
        public void InitHexTileRenderer(float size, HexTileType tileType, CreateTileAxisType axisType, Material material)
        {
            hexTileSize = size * 0.5f;
            hexTileType = tileType;
            createTileAxisType = axisType;
            this.material = material;
            
        }

        public Color TileColor { get => meshRenderer.material.color; set => meshRenderer.material.color = value; }

        [ButtonMethod]
        public void CreateMesh()
        {
            CreateMesh(this);
        }

        /// <summary>
        /// 육각타일 메쉬 만드는 함수
        /// </summary>
        /// <param name="hexTileBatchObject"></param>
        public static void CreateMesh(HexTileObject hexTileBatchObject)
        {
            //지정된메쉬가 다른메쉬인지 체크
            Mesh newMesh;

            bool isNeedCreateNew = false;
            if (hexTileBatchObject.mesh == null)
            {
                isNeedCreateNew = true;
            }
            else
            {
                isNeedCreateNew = hexTileBatchObject.mesh.vertices.Length != 6;
            }


            if (isNeedCreateNew)
            {
                newMesh = new Mesh();
                newMesh.name = "HexTileMesh";
            }
            else
            {
                newMesh = hexTileBatchObject.mesh;
            }

            //점정가져오기
            newMesh.vertices = GetHexTilePoint(Vector3.zero, hexTileBatchObject.hexTileSize, hexTileBatchObject.hexTileType, hexTileBatchObject.createTileAxisType);
            //newMesh.vertices = GetHexTilePoint(Vector3.zero, hexTileSize, 0, hexTileType, createTileAxisType);//이건 점 박는게 이상한가보네 나중에 체크하기

            //삼각형처리//정점의 시작위치는 상관없고//특정방향으로 돌게끔 순서를 체크
            //여기도 좀 알아봐야할듯
            newMesh.triangles = new int[12]
            {
                    0,1,5,
                    1,4,5,
                    1,2,4,
                    2,3,4,
            };

            //뭔가 안되는데//여기도 좀보기//일단 잘되는데//강사님이 알려준거
            //newMesh.triangles = new int[12]
            //{
            //        0,1,2,//0 1 2
            //        1,5,4,//1 5 4
            //        1,4,2,//1 4 2
            //        5,3,4,//5 3 4 
            //};

            //newMesh.triangles = new int[12]
            //{
            //        0,1,2,
            //        1,4,5,
            //        1,2,4,
            //        5,4,3,
            //};

            //빛받는방향임
            //노말처리//방향성처리하는것//xy, xz
            Vector3 targetDir = Vector3.zero;
            switch (hexTileBatchObject.createTileAxisType)
            {
                case CreateTileAxisType.XY:
                    targetDir = hexTileBatchObject.transform.forward;
                    break;
                case CreateTileAxisType.XZ:
                    targetDir = hexTileBatchObject.transform.up;
                    break;
            }
            
            newMesh.normals = new Vector3[6]
            {
                targetDir,
                targetDir,
                targetDir,
                targetDir,
                targetDir,
                targetDir,
            };

            //UV처리?//버텍스와 동일하게 처리
            //Vector2[] uv = new Vector2[6]
            //{
            //    new Vector2(0, 0),
            //    new Vector2(1, 0),
            //    new Vector2(0, 1),
            //    new Vector2(1, 1),
            //    new Vector2(1, 1),
            //    new Vector2(1, 1),
            //};
            newMesh.uv = lLcroweUtil.ConvertVector2Array(GetHexTilePoint(Vector3.zero, 1, hexTileBatchObject.hexTileType, hexTileBatchObject.createTileAxisType));
            //newMesh.uv = lLcroweUtil.ConvertVector2Array(GetHexTilePoint(Vector3.zero, 1, 0, hexTileType, createTileAxisType));



            //컬러처리
            //Color targetVertexColor = Color.red;
            //mesh.colors = new Color[6]
            //{
            //    targetVertexColor,
            //    targetVertexColor,
            //    targetVertexColor,
            //    targetVertexColor,
            //    targetVertexColor,
            //    targetVertexColor,
            //};

            hexTileBatchObject.mesh = newMesh;

            //자동집어넣기
            if (hexTileBatchObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = hexTileBatchObject.material;
            }
            if (hexTileBatchObject.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter.mesh = newMesh;
            }

            if (hexTileBatchObject.TryGetComponent(out MeshCollider meshCollider))
            {
                meshCollider.sharedMesh = newMesh;
            }
        }

        private void OnDrawGizmos()
        {
            Vector3Int temp = hexTileData.GetTilePos();
            Handles.Label(transform.position, $"{temp.x},{temp.y}");
        }
    }
}