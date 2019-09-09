using TileTerrain.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using SimpleJSON;

namespace TileTerrain.Component {
    /// <summary>
    /// 地图控制行为
    /// </summary>
    public enum MapAction {
        /// <summary>
        /// 无行为
        /// </summary>
        None,
        /// <summary>
        /// 抬升
        /// </summary>
        Raise,
        /// <summary>
        /// 降低
        /// </summary>
        Down,
        /// <summary>
        /// 抹平
        /// </summary>
        Smooth,
        /// <summary>
        /// 涂刷
        /// </summary>
        Spread,
    }
    public class Map : MonoBehaviour {
        /// <summary>
        /// 单利
        /// </summary>
        public static Map instance;
        /// <summary>
        /// 方块预制体
        /// </summary>
        [HideInInspector]
        public static GameObject chunkPrefab;
        /// <summary>
        /// 当前行为
        /// </summary>
        public MapAction mapAction = MapAction.None;
        /// <summary>
        /// 生成的所有chunk
        /// </summary>
        public Dictionary<Vector3i, Chunk> chunks = new Dictionary<Vector3i, Chunk>();
        /// <summary>
        /// 另一种形时的存储
        /// </summary>
        public List<Chunk> chunks2 = new List<Chunk>();
        /// <summary>
        /// 地形宽度
        /// </summary>
        public int MapLength = 250;
        /// <summary>
        /// 单个chunk的长和宽
        /// </summary>
        public int ChunkWidth = 25;
        /// <summary>
        /// 地形高度
        /// </summary>
        public int MapHeight = 25;
        /// <summary>
        /// 一个方向上Chunk的个数
        /// </summary>
        [HideInInspector]
        public int chunkLineNumber;
        /// <summary>
        /// 单个chunk中面的总数
        /// </summary>
        [HideInInspector]
        public int tileNumberOneChunk;
        /// <summary>
        /// 地形高度图，从左下角向右排列，向上延伸
        /// </summary>
        public float[] heightMap;
        public int[] textureMap;
        /// <summary>
        /// 当前是否正在生成Chunk
        /// </summary>
        private bool spawningChunk = false;
        /// <summary>
        /// 地图加载完毕的回调
        /// </summary>
        public Action LoadMapOver;
        /// <summary>
        /// 用来记录需要更改高度点的位置
        /// </summary>
        private List<float> weightList;
        [HideInInspector]
        public string mapInfoFromInitinalDemo;
        private int number = 0;
        void Awake() {
            instance = this;
            heightMap = new float[(MapLength + 1) * (MapLength + 1)];
            textureMap = new int[MapLength * MapLength];
            tileNumberOneChunk = ChunkWidth * ChunkWidth;
            chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
            chunkLineNumber = Mathf.FloorToInt(MapLength / ChunkWidth);
            weightList = new List<float>();
        }

        Ray ray;
        RaycastHit hit;
        private Vector3 lastPos = Vector3.zero;

        private int Material = 1;
        public void ChangeMaterial(int materialNum) {
            Material = materialNum;
        }
        public void UseMaterial() {
            mapAction = MapAction.Spread;
        }
        public void Raise() {
            mapAction = MapAction.Raise;
        }
        public void Down() {
            mapAction = MapAction.Down;
        }
        public void Smooth() {
            mapAction = MapAction.Smooth;
        }

        //生成Chunk
        public void CreateChunk(Vector3i pos) {
            //if (spawningChunk) return;
            StartCoroutine(SpawnChunk(pos));
        }
        /// <summary>
        /// 生成chunk的协程
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private IEnumerator SpawnChunk(Vector3i pos) {
            spawningChunk = true;
            GameObject chunkTemp = Instantiate(chunkPrefab, pos + transform.position, Quaternion.identity);
            chunkTemp.GetComponent<Chunk>().number = number;
            chunkTemp.transform.SetParent(transform);
            number += 1;
            yield return null;
            spawningChunk = false;
        }

        //通过Chunk的坐标来判断它是否存在
        public bool ChunkExists(Vector3i worldPosition) {
            return this.ChunkExists(worldPosition.x, worldPosition.y, worldPosition.z);
        }
        //通过Chunk的坐标来判断它是否存在
        public bool ChunkExists(int x, int y, int z) {
            return chunks.ContainsKey(new Vector3i(x, y, z));
        }
        #region Function
        private float radius;
        private float changeValue;
        private Vector2 targetPos;

        /// <summary>
        /// 更改地面高度
        /// </summary>
        /// <param name="pos">中心点位置</param>
        /// <param name="radius">半径</param>
        /// <param name="changeValue">中心点的改变值</param>
        /// <param name="weight">权重</param>
        public void ChangeGroundHeight(Vector3 pos, float radius, float changeValue) {
            Debug.Log(pos);

            pos += new Vector3(60f, 0f, 60f);
            //if (pos.y > 2f) {
            //    radius = 15f + (pos.y - 2) * 0.5f;
            //}
            targetPos = new Vector2(pos.x, pos.z);
            this.radius = radius;
            this.changeValue = changeValue;
            List<Vector3> newPoints = CheckPoints(pos, radius);
            List<int> chunkTemp = new List<int>();
            for (int i = 0; i < newPoints.Count; i++) {
                if ((int)newPoints[i].x > chunks2.Count - 1 || (int)newPoints[i].x < 0) {
                    continue;
                }
                chunks2[(int)newPoints[i].x].ChangePointPos((int)newPoints[i].y, (int)newPoints[i].z, weightList[i]);
                if (!chunkTemp.Contains((int)newPoints[i].x)) {
                    chunkTemp.Add((int)newPoints[i].x);
                }
            }
            for (int i = 0; i < chunkTemp.Count; i++) {
                chunks2[chunkTemp[i]].RefreshMesh();
            }
        }
        /// <summary>
        /// 抹平时的目标高度
        /// </summary>
        internal float targetHeight = 0f;
        /// <summary>
        /// 抹平地面
        /// </summary>
        /// <param name="pos">选中位置</param>
        /// <param name="radius">作用半径</param>
        /// <param name="changeValue">权重</param>
        public void SmoothGround(Vector3 pos, float radius, float changeValue) {
            pos += new Vector3(60f, 0f, 60f);
            targetPos = new Vector2(pos.x, pos.z);
            this.radius = radius;
            this.changeValue = changeValue;
            List<Vector3> newPoints = CheckPoints(pos, radius);
            List<int> chunkTemp = new List<int>();
            for (int i = 0; i < newPoints.Count; i++) {
                if ((int)newPoints[i].x > chunks2.Count - 1 || (int)newPoints[i].x < 0) {
                    continue;
                }
                chunks2[(int)newPoints[i].x].ChangePointPos((int)newPoints[i].y, (int)newPoints[i].z, weightList[i]);
                if (!chunkTemp.Contains((int)newPoints[i].x)) {
                    chunkTemp.Add((int)newPoints[i].x);
                }
            }
            for (int i = 0; i < chunkTemp.Count; i++) {
                chunks2[chunkTemp[i]].RefreshMesh();
            }
        }
        /// <summary>
        /// 更新材质
        /// </summary>
        /// <param name="pos">中心点位置</param>
        /// <param name="radius">半径</param>
        /// <param name="materialNumber">材质代号</param>
        public void UpdateMaterial(Vector3 pos, float radius, int materialNumber) {
            UpdateMaterial(pos, radius, materialNumber, true);
            UpdateMaterial(pos, radius + 3, materialNumber, false);
        }

        /// <summary>
        /// 更新材质
        /// </summary>
        /// <param name="pos">中心点位置</param>
        /// <param name="radius">半径</param>
        /// <param name="materialNumber">材质代号</param>
        /// <param name="changeMaterial">是否更改材质</param>
        public void UpdateMaterial(Vector3 pos, float radius, int materialNumber, bool changeMaterial) {
            pos += new Vector3(60f, 0f, 60f);
            targetPos = new Vector2(pos.x, pos.z);
            this.radius = radius;
            List<Vector2Int> newPoints = CheckSurfaces(pos, radius);
            List<int> chunkTemp = new List<int>();
            //预更新材质信息
            for (int i = 0; i < newPoints.Count; i++) {
                int meshNumberForLine = (Mathf.FloorToInt(newPoints[i].x / chunkLineNumber) * ChunkWidth + Mathf.FloorToInt(newPoints[i].y / ChunkWidth)) * MapLength + newPoints[i].x % chunkLineNumber * ChunkWidth + newPoints[i].y % ChunkWidth;
                if (changeMaterial) {
                    textureMap[meshNumberForLine] = materialNumber;
                }
            }
            char[] tempChar = new char[8];
            int posTemp;
            //更新UV
            for (int i = 0; i < newPoints.Count; i++) {
                int meshNumberForLine = (Mathf.FloorToInt(newPoints[i].x / chunkLineNumber) * ChunkWidth + Mathf.FloorToInt(newPoints[i].y / ChunkWidth)) * MapLength + newPoints[i].x % chunkLineNumber * ChunkWidth + newPoints[i].y % ChunkWidth;
                posTemp = meshNumberForLine;
                if (posTemp + MapLength - 1 < textureMap.Length && textureMap[posTemp + MapLength - 1] == textureMap[meshNumberForLine]) {
                    tempChar[7] = '1';
                } else {
                    tempChar[7] = '0';
                }
                if (posTemp + MapLength < textureMap.Length && textureMap[posTemp + MapLength] == textureMap[meshNumberForLine]) {
                    tempChar[6] = '1';
                } else {
                    tempChar[6] = '0';
                }
                if (posTemp + MapLength + 1 < textureMap.Length && textureMap[posTemp + MapLength + 1] == textureMap[meshNumberForLine]) {
                    tempChar[5] = '1';
                } else {
                    tempChar[5] = '0';
                }
                if (posTemp - 1 > 0 && textureMap[posTemp - 1] == textureMap[meshNumberForLine]) {
                    tempChar[4] = '1';
                } else {
                    tempChar[4] = '0';
                }
                if (posTemp + 1 < textureMap.Length && textureMap[posTemp + 1] == textureMap[meshNumberForLine]) {
                    tempChar[3] = '1';
                } else {
                    tempChar[3] = '0';
                }
                if (posTemp - MapLength - 1 > 0 && textureMap[posTemp - MapLength - 1] == textureMap[meshNumberForLine]) {
                    tempChar[2] = '1';
                } else {
                    tempChar[2] = '0';
                }
                if (posTemp - MapLength > 0 && textureMap[posTemp - MapLength] == textureMap[meshNumberForLine]) {
                    tempChar[1] = '1';
                } else {
                    tempChar[1] = '0';
                }
                if (posTemp - MapLength + 1 > 0 && textureMap[posTemp - MapLength + 1] == textureMap[meshNumberForLine]) {
                    tempChar[0] = '1';
                } else {
                    tempChar[0] = '0';
                }
                chunks2[newPoints[i].x].ReSetMaterial(newPoints[i].y, textureMap[meshNumberForLine], new string(tempChar));
                if (!chunkTemp.Contains(newPoints[i].x)) {
                    chunkTemp.Add(newPoints[i].x);
                }
            }
            for (int i = 0; i < chunkTemp.Count; i++) {
                chunks2[chunkTemp[i]].RefreshMesh();
            }
        }
        private float distance;
        /// <summary>
        /// 检测需要更新高度的点
        /// </summary>
        /// <returns></returns>
        private List<Vector3> CheckPoints(Vector3 pos, float radius) {
            Vector2Int tempStartV2 = new Vector2Int(Mathf.RoundToInt(pos.x - radius), Mathf.RoundToInt(pos.z - radius));
            Vector2Int tempEndV2 = new Vector2Int(Mathf.RoundToInt(pos.x + radius), Mathf.RoundToInt(pos.z + radius));
            tempStartV2 = new Vector2Int(Mathf.Clamp(tempStartV2.x, 0, MapLength), Mathf.Clamp(tempStartV2.y, 0, MapLength));
            tempEndV2 = new Vector2Int(Mathf.Clamp(tempEndV2.x, 0, MapLength), Mathf.Clamp(tempEndV2.y, 0, MapLength));
            List<Vector2> points = new List<Vector2>();
            for (int i = tempStartV2.x; i <= tempEndV2.x; i++) {
                for (int j = tempStartV2.y; j <= tempEndV2.y; j++) {
                    if (Vector2.Distance(new Vector2(i, j), new Vector2(pos.x, pos.z)) < radius) {
                        points.Add(new Vector2(i, j));
                    }
                }
            }
            return MapPoint2ChunkPoint(points);
        }
        List<Vector2> SurfacesPoints;
        /// <summary>
        /// 检测需要更新材质的面
        /// </summary>
        /// <param name="pos">作用中心点</param>
        /// <param name="radius">作用半径</param>
        /// <returns>作用面。x表示第几个chunk，y表示第几个面</returns>
        private List<Vector2Int> CheckSurfaces(Vector3 pos, float radius) {
            Vector2Int tempStartV2 = new Vector2Int(Mathf.RoundToInt(pos.x - radius), Mathf.RoundToInt(pos.z - radius));
            Vector2Int tempEndV2 = new Vector2Int(Mathf.RoundToInt(pos.x + radius), Mathf.RoundToInt(pos.z + radius));
            tempStartV2 = new Vector2Int(Mathf.Clamp(tempStartV2.x, 0, MapLength), Mathf.Clamp(tempStartV2.y, 0, MapLength));
            tempEndV2 = new Vector2Int(Mathf.Clamp(tempEndV2.x, 0, MapLength), Mathf.Clamp(tempEndV2.y, 0, MapLength));
            SurfacesPoints = new List<Vector2>();
            for (int i = tempStartV2.x; i < tempEndV2.x; i++) {
                for (int j = tempStartV2.y; j < tempEndV2.y; j++) {
                    if (Vector2.Distance(new Vector2(i, j), new Vector2(pos.x, pos.z)) < radius) {
                        SurfacesPoints.Add(new Vector2(i, j));
                    }
                }
            }
            return MapPoint2Surfaces(SurfacesPoints);
        }
        /// <summary>
        /// 从地图坐标转换到堆坐标
        /// </summary>
        /// <param name="points"></param>
        /// <returns>堆坐标集，x表示第几个堆，y表示堆的第几行(0开始)，z表示堆的第几列(0开始)</returns>
        private List<Vector3> MapPoint2ChunkPoint(List<Vector2> points) {
            List<Vector3> newPoints = new List<Vector3>();
            weightList.Clear();
            int xNumber;
            int zNumber;
            for (int i = 0; i < points.Count; i++) {
                xNumber = Mathf.FloorToInt(points[i].x / ChunkWidth);
                zNumber = Mathf.FloorToInt(points[i].y / ChunkWidth);
                //交界点向左延伸一行(添加隔壁面的点)
                if (xNumber != 0 && points[i].x == xNumber * ChunkWidth) {
                    newPoints.Add(new Vector3(xNumber - 1 + zNumber * chunkLineNumber, ChunkWidth, points[i].y - zNumber * ChunkWidth));
                    AddWeight(points[i]);
                }
                if (points[i].y == zNumber * ChunkWidth && zNumber != 0) {
                    if (points[i].x - xNumber * ChunkWidth == 0 && xNumber != 0) {
                        //交界点向右延伸一行
                        newPoints.Add(new Vector3(xNumber - 1 + (zNumber - 1) * chunkLineNumber, ChunkWidth, ChunkWidth));
                        AddWeight(points[i]);
                    }
                    //交界点向上延伸一行
                    if (xNumber != chunkLineNumber) {
                        newPoints.Add(new Vector3(xNumber + (zNumber - 1) * chunkLineNumber, points[i].x - xNumber * ChunkWidth, ChunkWidth));
                        AddWeight(points[i]);
                    }
                }
                if (xNumber != chunkLineNumber) {
                    newPoints.Add(new Vector3(xNumber + zNumber * chunkLineNumber, points[i].x - xNumber * ChunkWidth, points[i].y - zNumber * ChunkWidth));
                    AddWeight(points[i]);
                }
            }
            return newPoints;
        }
        /// <summary>
        /// 从地图坐标转换到面集合
        /// </summary>
        /// <param name="points"></param>
        /// <returns>面集合，表示第几个堆(x)的第几个面(y)</returns>
        private List<Vector2Int> MapPoint2Surfaces(List<Vector2> points) {
            List<Vector2Int> newSurfaces = new List<Vector2Int>();
            int xNumber;
            int zNumber;
            for (int i = 0; i < points.Count; i++) {
                xNumber = Mathf.FloorToInt(points[i].x / ChunkWidth);
                zNumber = Mathf.FloorToInt(points[i].y / ChunkWidth);
                newSurfaces.Add(new Vector2Int(xNumber + zNumber * chunkLineNumber, (int)(points[i].x - xNumber * ChunkWidth + (points[i].y - zNumber * ChunkWidth) * ChunkWidth)));
            }
            return newSurfaces;
        }
        /// <summary>
        /// 使用函数为点阵增加坡度
        /// </summary>
        /// <param name="pos"></param>
        private void AddWeight(Vector2 pos) {
            distance = Vector2.Distance(targetPos, pos);
            if (mapAction == MapAction.Raise || mapAction == MapAction.Down) {
                //4f坡度系数
                float heightTemp = (radius - distance) / radius * ((radius - distance) / radius) * changeValue / 4f;
                weightList.Add(heightTemp);
            } else if (mapAction == MapAction.Smooth) {
                float heightTemp = (radius - distance) / radius * ((radius - distance) / radius);
                weightList.Add(heightTemp);
            }
        }

        #endregion
        #region 地图信息存储
        /// <summary>
        /// 重新加载地形
        /// </summary>
        public void ReloadTileMap() {

        }
        /// <summary>
        /// 获取地形高度
        /// </summary>
        public float[] GetHeightMap() {
            float[] heightMap = new float[(MapLength + 1) * (MapLength + 1)];
            for (int i = 0; i < MapLength; i++) {
                for (int j = 0; j < MapLength; j++) {
                }
            }
            for (int i = 0; i < chunks2.Count; i++) {
                for (int j = 0; j < ChunkWidth; j++) {
                    for (int k = 0; k < ChunkWidth; k++) {

                    }
                }
            }
            return heightMap;
        }

        private byte[] byteMsg = new byte[2];
        List<byte> byteList = new List<byte>();
        byte[] byteTemp = new byte[2];
        private int heightIntNumber;
        private int heightFloatNumber;
        private int TextureNumber;
        /// <summary>
        /// 将数据转换成地形(根据地形数据加载地形)
        /// </summary>
        public void MsgToMap(string mapMsg) {
            if (transform.childCount > 1) {
                for (int i = 0; i < chunks2.Count; i++) {
                    Destroy(chunks2[i].gameObject);
                }
                chunks = new Dictionary<Vector3i, Chunk>();
                chunks2 = new List<Chunk>();
                number = 0;
            }
            heightMap = new float[(MapLength + 1) * (MapLength + 1)];
            textureMap = new int[MapLength * MapLength];
            byte[] byteArray = new byte[(MapLength + 1) * (MapLength + 1) * 2];
            byteArray = System.Convert.FromBase64String(mapMsg);
            for (int j = 0; j < MapLength + 1; j++) {
                for (int i = 0; i < MapLength + 1; i++) {
                    byteMsg[0] = byteArray[(i + j * (MapLength + 1)) * 2];
                    byteMsg[1] = byteArray[(i + j * (MapLength + 1)) * 2 + 1];
                    TileTerrainMsgTool.ByteToTileMsg(byteMsg, out heightIntNumber, out heightFloatNumber, out TextureNumber);
                    if (i == MapLength || j == MapLength) {
                        heightMap[i + j * (MapLength + 1)] = heightIntNumber - 6f + HeightIntToFloat(heightFloatNumber);
                    } else {
                        heightMap[i + j * (MapLength + 1)] = heightIntNumber - 6f + HeightIntToFloat(heightFloatNumber);
                        textureMap[i + j * MapLength] = TextureNumber;
                    }
                }
            }
            for (int z = 0; z < 0 + ChunkWidth * chunkLineNumber; z += ChunkWidth) {
                for (int x = 0; x < 0 + ChunkWidth * chunkLineNumber; x += ChunkWidth) {
                    int xx = ChunkWidth * Mathf.FloorToInt(x / ChunkWidth);
                    int zz = ChunkWidth * Mathf.FloorToInt(z / ChunkWidth);
                    if (!ChunkExists(xx, 0, zz)) {
                        CreateChunk(new Vector3i(xx, 0, zz));
                    }
                }
            }
            LoadMapOver?.Invoke();
        }
        /// <summary>
        /// 初始化默认地形
        /// </summary>
        public void InitinalMap() {
            string mapMsg="";
            if (transform.childCount > 1) {
                for (int i = 0; i < chunks2.Count; i++) {
                    Destroy(chunks2[i].gameObject);
                }
                chunks = new Dictionary<Vector3i, Chunk>();
                chunks2 = new List<Chunk>();
                number = 0;
            }
            heightMap = new float[(MapLength + 1) * (MapLength + 1)];
            textureMap = new int[MapLength * MapLength];
            byte[] byteArray = new byte[(MapLength + 1) * (MapLength + 1) * 2];
            StudentCourseInfo info = JsonMapper.ToObject<StudentCourseInfo>(Resources.Load<DefaultSceneInfo>("Config/DefaultSceneInfo").info);
            mapInfoFromInitinalDemo = info.code;
            JSONNode obj = JSON.Parse(mapInfoFromInitinalDemo);
            mapMsg = obj["mapInfo"]["mapData"];
            byteArray = System.Convert.FromBase64String(mapMsg);
            for (int j = 0; j < MapLength + 1; j++) {
                for (int i = 0; i < MapLength + 1; i++) {
                    byteMsg[0] = byteArray[(i + j * (MapLength + 1)) * 2];
                    byteMsg[1] = byteArray[(i + j * (MapLength + 1)) * 2 + 1];
                    TileTerrainMsgTool.ByteToTileMsg(byteMsg, out heightIntNumber, out heightFloatNumber, out TextureNumber);
                    if (i == MapLength || j == MapLength) {
                        heightMap[i + j * (MapLength + 1)] = heightIntNumber - 6f + HeightIntToFloat(heightFloatNumber);
                    } else {
                        heightMap[i + j * (MapLength + 1)] = heightIntNumber - 6f + HeightIntToFloat(heightFloatNumber);
                        textureMap[i + j * MapLength] = TextureNumber;
                    }
                }
            }
            for (int z = 0; z < 0 + ChunkWidth * chunkLineNumber; z += ChunkWidth) {
                for (int x = 0; x < 0 + ChunkWidth * chunkLineNumber; x += ChunkWidth) {
                    int xx = ChunkWidth * Mathf.FloorToInt(x / ChunkWidth);
                    int zz = ChunkWidth * Mathf.FloorToInt(z / ChunkWidth);
                    if (!ChunkExists(xx, 0, zz)) {
                        CreateChunk(new Vector3i(xx, 0, zz));
                    }
                }
            }
            LoadMapOver?.Invoke();
        }
        /// <summary>
        /// 将地形转换成数据
        /// </summary>
        public string MapToMsg() {
            byteList = new List<byte>();
            byte[] byteArray = new byte[(MapLength + 1) * (MapLength + 1) * 2];
            for (int j = 0; j < MapLength + 1; j++) {
                for (int i = 0; i < MapLength + 1; i++) {
                    if (i == MapLength || j == MapLength) {
                        byteTemp = TileTerrainMsgTool.TileMsgToByte(Mathf.FloorToInt(heightMap[i + j * (MapLength + 1)] + 6f), HeightFloatToInt(heightMap[i + j * (MapLength + 1)]), 0);
                    } else {
                        byteTemp = TileTerrainMsgTool.TileMsgToByte(Mathf.FloorToInt(heightMap[i + j * (MapLength + 1)] + 6f), HeightFloatToInt(heightMap[i + j * (MapLength + 1)]), textureMap[i + j * MapLength]);
                    }
                    byteList.Add(byteTemp[0]);
                    byteList.Add(byteTemp[1]);
                }
            }
            byteArray = byteList.ToArray();
            return System.Convert.ToBase64String(byteArray);
        }
        #endregion

        float temp;
        /// <summary>
        /// 将高度的小数部分转换成代码数字
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public int HeightFloatToInt(float height) {
            return Mathf.FloorToInt((height - Mathf.FloorToInt(height)) * 100);
        }
        /// <summary>
        /// 从代码数字转换成高度的小数部分
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public float HeightIntToFloat(int height) {
            return height / 100f;
        }
    }
}