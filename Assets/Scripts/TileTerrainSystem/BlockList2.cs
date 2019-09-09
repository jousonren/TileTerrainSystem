using System.Collections.Generic;
using UnityEngine;
namespace TileTerrain.Component {
    /// <summary>
    /// 存储所有的Block对象的信息
    /// </summary>
    /// 底层的贴图列表
    public class BlockList2 : MonoBehaviour {
        /// <summary>
        /// 所有生成的Block
        /// </summary>
        public static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>();
        public static Dictionary<byte, Block> grassBlocks = new Dictionary<byte, Block>();
        private static bool hasInitinal = false;
        void Awake() {
            if (hasInitinal) {
                return;
            }
            hasInitinal = true;
            Block Grass1 = new Block(0, "Grass1", 6, 5);
            Block Grass2 = new Block(0, "Grass2", 7, 5);
            Block Grass3 = new Block(1, "Grass3", 0, 6);
            Block Grass4 = new Block(2, "Grass4", 1, 6);
            Block Grass5 = new Block(3, "Grass5", 2, 6);
            Block Grass6 = new Block(4, "Grass6", 3, 6);
            Block Grass7 = new Block(5, "Grass7", 4, 6);
            Block Grass8 = new Block(6, "Grass8", 5, 6);
            Block Grass9 = new Block(7, "Grass9", 6, 6);
            Block Grass10 = new Block(8, "Grass10", 7, 6);
            Block Grass11 = new Block(9, "Grass11", 0, 7);
            Block Grass12 = new Block(10, "Grass12", 1, 7);
            Block Grass13 = new Block(11, "Grass13", 2, 7);
            Block Grass14 = new Block(12, "Grass14", 3, 7);
            Block Grass15 = new Block(13, "Grass15", 4, 7);

            Block Sand = new Block(1, "Sand", 14, 5);
            Block Snow = new Block(2, "Snow", 27, 0);
            Block Brick = new Block(3, "Brick", 24, 0);
            Block Stone = new Block(4, "Stone", 26, 0);
            Block Soil = new Block(5, "Soil", 25, 0);
            blocks.Add(Grass1.id, Grass1);
            blocks.Add(Sand.id, Sand);
            blocks.Add(Snow.id, Snow);
            blocks.Add(Brick.id, Brick);
            blocks.Add(Stone.id, Stone);
            blocks.Add(Soil.id, Soil);
            grassBlocks.Add(Grass2.id, Grass2);
            grassBlocks.Add(Grass3.id, Grass3);
            grassBlocks.Add(Grass4.id, Grass4);
            grassBlocks.Add(Grass5.id, Grass5);
            grassBlocks.Add(Grass6.id, Grass6);
            grassBlocks.Add(Grass7.id, Grass7);
            grassBlocks.Add(Grass8.id, Grass8);
            grassBlocks.Add(Grass9.id, Grass9);
            grassBlocks.Add(Grass10.id, Grass10);
            grassBlocks.Add(Grass11.id, Grass11);
            grassBlocks.Add(Grass12.id, Grass12);
            grassBlocks.Add(Grass13.id, Grass13);
            grassBlocks.Add(Grass14.id, Grass14);
            grassBlocks.Add(Grass15.id, Grass15);

        }

        public static Block GetBlock(byte id) {
            if (id == 0 || id == 2) {
                return blocks[1];
            } else {
                return blocks.ContainsKey(id) ? blocks[id] : null;
            }
        }
    }
}