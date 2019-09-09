//using TileTerrain.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TileTerrain.Component {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    ///方块堆
    public class Chunk : MonoBehaviour {
        /// <summary>
        /// 第几个chunk(从0开始)
        /// </summary>
        public int number;
        /// <summary>
        /// 该方块堆的长度和宽度(底部为一个正方形)
        /// </summary>
        private static int width;
        /// <summary>
        /// 该方块堆的高度
        /// </summary>
        private static int height;
        /// <summary>
        /// 该堆所处的位置
        /// </summary>
        private TileTerrain.Util.Vector3i position;
        /// <summary>
        /// 自动生成的mesh
        /// </summary>
        private Mesh mesh;
        /// <summary>
        /// 生成面所需要的所有点
        /// </summary>
        private List<Vector3> vertices = new List<Vector3>();
        /// <summary>
        /// 生成三边面时用到的vertices的index
        /// </summary>
        private List<int> triangles = new List<int>();
        /// <summary>
        /// 所有的UV信息
        /// </summary>
        private List<Vector2> uv = new List<Vector2>();
        /// <summary>
        /// 所有的UV信息
        /// </summary>
        private List<Vector2> uv2 = new List<Vector2>();
        /// <summary>
        /// uv贴图每行每列的宽度(0~1)，贴图为32×32，所以是1/32
        /// </summary>
        private static float textureOffset = 1 / 32f;
        /// <summary>
        /// 每个UV收缩的长度，避免出现它旁边的贴图
        /// </summary>
        private static float shrinkSize = 0.001f;
        /// <summary>
        /// 当前Chunk是否正在生成中
        /// </summary>
        private bool isWorking = false;

        void Start() {
            mesh = new Mesh {
                name = "Chunk"
            };

            width = Map.instance.ChunkWidth;
            height = Map.instance.MapHeight;
            position = new TileTerrain.Util.Vector3i(transform.position);
            if (Map.instance.ChunkExists(position)) {
                Debug.Log("此方块已存在" + position);
                Destroy(this);
            } else {
                Map.instance.chunks.Add(position, this);
                Map.instance.chunks2.Add(this);
                name = "(" + position.x + "," + position.y + "," + position.z + ")";
                StartFunction();
            }
        }

        void StartFunction() {
            StartCoroutine(CreateMap());
        }
        /// <summary>
        /// 生成Chunk
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateMap() {
            while (isWorking) {
                yield return null;
            }
            isWorking = true;
            StartCoroutine(CreateMesh());
        }
        /// <summary>
        /// 生成Chunk所需要的所有面
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateMesh() {
            vertices.Clear();
            triangles.Clear();
            //把所有面的点和面的索引添加进去
            for (int z = 0; z < width; z++) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        ///所有面的第几个面(从下往上从左往右根据行排序)
                        int meshNumberForLine = (Mathf.FloorToInt(number / Map.instance.chunkLineNumber) * Map.instance.ChunkWidth + z) * Map.instance.MapLength + number % Map.instance.chunkLineNumber * Map.instance.ChunkWidth + x;
                        byte tempByte = (byte)Map.instance.textureMap[meshNumberForLine];
                        char[] tempChar = new char[8];

                        if (meshNumberForLine + Map.instance.MapLength - 1 < Map.instance.textureMap.Length && Map.instance.textureMap[meshNumberForLine + Map.instance.ChunkWidth * Map.instance.chunkLineNumber - 1] == tempByte) {
                            tempChar[7] = '1';
                        } else {
                            tempChar[7] = '0';
                        }
                        if (meshNumberForLine + Map.instance.MapLength < Map.instance.textureMap.Length && Map.instance.textureMap[meshNumberForLine + Map.instance.MapLength] == tempByte) {
                            tempChar[6] = '1';
                        } else {
                            tempChar[6] = '0';
                        }
                        if (meshNumberForLine + Map.instance.MapLength + 1 < Map.instance.textureMap.Length && Map.instance.textureMap[meshNumberForLine + Map.instance.MapLength + 1] == tempByte) {
                            tempChar[5] = '1';
                        } else {
                            tempChar[5] = '0';
                        }
                        if (meshNumberForLine - 1 >= 0 && Map.instance.textureMap[meshNumberForLine - 1] == tempByte) {
                            tempChar[4] = '1';
                        } else {
                            tempChar[4] = '0';
                        }
                        if (meshNumberForLine + 1 < Map.instance.textureMap.Length && Map.instance.textureMap[meshNumberForLine + 1] == tempByte) {
                            tempChar[3] = '1';
                        } else {
                            tempChar[3] = '0';
                        }
                        if (meshNumberForLine - Map.instance.MapLength - 1 >= 0 && Map.instance.textureMap[meshNumberForLine - Map.instance.MapLength - 1] == tempByte) {
                            tempChar[2] = '1';
                        } else {
                            tempChar[2] = '0';
                        }
                        if (meshNumberForLine - Map.instance.MapLength >= 0 && Map.instance.textureMap[meshNumberForLine - Map.instance.MapLength] == tempByte) {
                            tempChar[1] = '1';
                        } else {
                            tempChar[1] = '0';
                        }
                        if (meshNumberForLine - Map.instance.MapLength + 1 >= 0 && Map.instance.textureMap[meshNumberForLine - Map.instance.MapLength + 1] == tempByte) {
                            tempChar[0] = '1';
                        } else {
                            tempChar[0] = '0';
                        }
                        //获取当前坐标的Block对象
                        Block block = BlockList.GetBlock(tempByte, new string(tempChar));
                        Block block2 = BlockList2.GetBlock(tempByte);
                        if (block == null) continue;
                        AddTopFace(x, y, z, block, block2);
                    }
                }
            }

            //为点和index赋值
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv4 = uv.ToArray();
            mesh.uv = uv2.ToArray();

            //重新计算顶点和法线
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            //将生成好的面赋值给组件
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;

            yield return null;
            isWorking = false;
        }

        //上面
        void AddTopFace(int x, int y, int z, Block block, Block block2) {

            //第一个三角面
            triangles.Add(1 + vertices.Count);
            triangles.Add(vertices.Count);
            triangles.Add(3 + vertices.Count);

            //第二个三角面
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);

            //添加4个点
            vertices.Add(new Vector3(1 + x, Map.instance.heightMap[(int)(transform.position.x + 60f + x + (transform.position.z + 60f + z) * (Map.instance.MapLength + 1f)) + 1], z));
            vertices.Add(new Vector3(1 + x, Map.instance.heightMap[(int)(transform.position.x + 60f + x + (transform.position.z + 1f + 60f + z) * (Map.instance.MapLength + 1f)) + 1], 1 + z));
            vertices.Add(new Vector3(x, Map.instance.heightMap[(int)(transform.position.x + 60f + x + (transform.position.z + 1f + 60f + z) * (Map.instance.MapLength + 1f))], 1 + z));
            vertices.Add(new Vector3(x, Map.instance.heightMap[(int)(transform.position.x + 60f + x + (transform.position.z + 60f + z) * (Map.instance.MapLength + 1f))], z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));

            uv2.Add(new Vector2(block2.textureTopX * textureOffset, block2.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv2.Add(new Vector2(block2.textureTopX * textureOffset + textureOffset, block2.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv2.Add(new Vector2(block2.textureTopX * textureOffset + textureOffset, block2.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv2.Add(new Vector2(block2.textureTopX * textureOffset, block2.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        /// <summary>
        /// 更改点的位置
        /// </summary>
        /// <param name="xPos">X轴方向第几个方格面</param>
        /// <param name="yPos">Z轴方向第几个方格面</param>
        /// <param name="height">高度变化</param>
        public void ChangePointPos(int xPos, int yPos, float height) {
            StartCoroutine(ReSetVerticesPosIEnumerator(xPos, yPos, height));
        }
        /// <summary>
        /// 更改点高度的协程
        /// </summary>
        /// <param name="xPos">X轴方向第几个方格面</param>
        /// <param name="yPos">Z轴方向第几个方格面</param>
        /// <param name="height">高度变化/权重</param>
        /// <returns></returns>
        IEnumerator ReSetVerticesPosIEnumerator(int xPos, int yPos, float height) {
            List<int> verticesTemp = Pos2CubePos(xPos, yPos);
            if (Map.instance.mapAction == MapAction.Down || Map.instance.mapAction == MapAction.Raise) {
                for (int i = 0; i < verticesTemp.Count; i++) {
                    vertices[verticesTemp[i]] = new Vector3(vertices[verticesTemp[i]].x, GetGradientHeight(vertices[verticesTemp[i]].y + height), vertices[verticesTemp[i]].z);
                    if (transform.position.z + 60f == 0) {
                        Map.instance.heightMap[(int)(transform.position.x + 60f + vertices[verticesTemp[i]].x + (transform.position.z + 60f + vertices[verticesTemp[i]].z) * (Map.instance.MapLength + 1))] = vertices[verticesTemp[i]].y;
                    } else {
                        Map.instance.heightMap[(int)(transform.position.x + 60f + vertices[verticesTemp[i]].x + (transform.position.z + 60f + vertices[verticesTemp[i]].z) * (Map.instance.MapLength + 1))] = vertices[verticesTemp[i]].y;
                    }
                }
            } else if (Map.instance.mapAction == MapAction.Smooth) {
                for (int i = 0; i < verticesTemp.Count; i++) {
                    if (vertices[verticesTemp[i]].y != Map.instance.targetHeight) {
                        vertices[verticesTemp[i]] = new Vector3(vertices[verticesTemp[i]].x, GetGradientHeight(Mathf.Lerp(vertices[verticesTemp[i]].y, Map.instance.targetHeight, height)), vertices[verticesTemp[i]].z);
                        if (transform.position.z + 60f == 0) {
                            Map.instance.heightMap[(int)(transform.position.x + 60f + vertices[verticesTemp[i]].x + (transform.position.z + 60f + vertices[verticesTemp[i]].z) * (Map.instance.MapLength + 1))] = vertices[verticesTemp[i]].y;
                        } else {
                            Map.instance.heightMap[(int)(transform.position.x + 60f + vertices[verticesTemp[i]].x + (transform.position.z + 60f + vertices[verticesTemp[i]].z) * (Map.instance.MapLength + 1))] = vertices[verticesTemp[i]].y;
                        }
                    }
                }
            }
            yield return null;
            isWorking = false;
        }
        /// <summary>
        /// 更改点的位置
        /// </summary>
        /// <param name="xPos">X轴方向第几个方格面</param>
        /// <param name="yPos">Z轴方向第几个方格面</param>
        /// <param name="height">高度变化</param>
        public void ChangePointPos2(int xPos, int yPos, float height) {
            StartCoroutine(ReSetVerticesPosIEnumerator2(xPos, yPos, height));
        }
        /// <summary>
        /// 更改点高度的协程
        /// </summary>
        /// <param name="xPos">X轴方向第几个方格面</param>
        /// <param name="yPos">Z轴方向第几个方格面</param>
        /// <param name="height">高度变化/权重</param>
        /// <returns></returns>
        IEnumerator ReSetVerticesPosIEnumerator2(int xPos, int yPos, float height) {
            List<int> verticesTemp = Pos2CubePos(xPos, yPos);
            for (int i = 0; i < verticesTemp.Count; i++) {
                vertices[verticesTemp[i]] = new Vector3(vertices[verticesTemp[i]].x, Map.instance.heightMap[(int)(transform.position.x + 60f + vertices[verticesTemp[i]].x + (transform.position.z + 60f + vertices[verticesTemp[i]].z) * (Map.instance.MapLength + 1))], vertices[verticesTemp[i]].z);
            }
            yield return null;
            isWorking = false;
        }
        /// <summary>
        /// 更新材质
        /// </summary>
        /// <param name="Pos">第几个面</param>
        /// <param name="materialNumber">材质编号</param>
        public void ReSetMaterial(int Pos, int materialNumber, string textureNumber) {
            StartCoroutine(ReSetMaterialIEnumerator(Pos, materialNumber, textureNumber));
        }

        /// <summary>
        /// 为每个面重新设置坐标uv偏移
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="materialNumber"></param>
        /// <returns></returns>
        IEnumerator ReSetMaterialIEnumerator(int Pos, int materialNumber, string textureNumber) {
            byte byteForMaterial = (byte)materialNumber;
            Block block = BlockList.GetBlock(byteForMaterial, textureNumber);
            Block block2 = BlockList2.GetBlock(byteForMaterial);
            uv[Pos * 4] = new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize);
            uv[Pos * 4 + 1] = new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize);
            uv[Pos * 4 + 2] = new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize);
            uv[Pos * 4 + 3] = new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize);
            uv2[Pos * 4] = new Vector2(block2.textureLeftX * textureOffset, block2.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize);
            uv2[Pos * 4 + 1] = new Vector2(block2.textureLeftX * textureOffset + textureOffset, block2.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize);
            uv2[Pos * 4 + 2] = new Vector2(block2.textureLeftX * textureOffset + textureOffset, block2.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize);
            uv2[Pos * 4 + 3] = new Vector2(block2.textureLeftX * textureOffset, block2.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize);
            yield return null;
            isWorking = false;
        }
        public void RefreshMesh() {
            //为点和index赋值
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv2.ToArray();
            mesh.uv4 = uv.ToArray();

            //重新计算顶点和法线
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //将生成好的面赋值给组件
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        /// <summary>
        /// 获取梯度高度
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        private float GetGradientHeight(float height) {
            height=Mathf.Clamp(height,-6f,24f);
            return (float)System.Math.Round(height,2,System.MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// 计算该位置的点被哪些正方形面占用
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<int> Pos2CubePos(int x, int y) {
            List<int> verticesTemp = new List<int>();
            //该点是在右上方方块中顶点的位置
            int posTemp = 4 * x + 4 * y * width;
            if (x != width && y != width) {
                verticesTemp.Add(posTemp + 3);
            }
            if (x != 0 && y != width) {
                //该点是左上方方块中顶点的位置
                verticesTemp.Add(posTemp - 4);
            }
            if (y != 0 && x != width) {
                //该点是右下方方块中顶点的位置
                verticesTemp.Add(posTemp + 2 - 4 * width);
            }
            if (x != 0 && y != 0) {
                //该点是左下方方块中顶点的位置
                verticesTemp.Add(posTemp - 3 - 4 * width);
            }
            return verticesTemp;
        }
    }
}