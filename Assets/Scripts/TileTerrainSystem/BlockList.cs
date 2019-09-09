using System.Collections.Generic;
using UnityEngine;
namespace TileTerrain.Component {
    /// <summary>
    /// 存储所有的Block对象的信息
    /// </summary>
    ///表层的贴图列表
    public class BlockList : MonoBehaviour {
        /// <summary>
        /// 所有生成的Block
        /// </summary>
        public static Dictionary<byte, Block> grassBlocks = new Dictionary<byte, Block>();
        /// <summary>
        /// 所有生成的Block
        /// </summary>
        public static Dictionary<byte, Block> sandBlocks = new Dictionary<byte, Block>();
        /// <summary>
        /// 所有生成的Block
        /// </summary>
        public static Dictionary<byte, Block> snowBlocks = new Dictionary<byte, Block>();
        /// <summary>
        /// 地形数据和对应block位置的映射(只相对于图集左下角第一个材质，其他材质需要做相应的偏移)
        /// </summary>
        public static Dictionary<string, int> blocksMapping=new Dictionary<string, int>();
        private static bool hasInitinal = false;

        void Awake() {
            if (hasInitinal) {
                return;
            }
            hasInitinal = true;
            Block temp;
            for (int j = 0; j < 8; j++) {
                for (int i = 0; i < 8; i++) {
                    temp = new Block((byte)(i + j * 8), "Grass", (byte)i, (byte)j);
                    grassBlocks.Add(temp.id, temp);
                }
            }
            for (int j = 0; j < 8; j++) {
                for (int i = 8; i < 16; i++) {
                    temp = new Block((byte)(i-8 + j * 8), "Sand", (byte)i, (byte)j);
                    sandBlocks.Add(temp.id, temp);
                }
            }
            for (int j = 0; j < 8; j++) {
                for (int i = 16; i < 24; i++) {
                    temp = new Block((byte)(i - 16 + j * 8), "Snow", (byte)i, (byte)j);
                    snowBlocks.Add(temp.id, temp);
                }
            }
            blocksMapping.Add("00000000", 0);
            blocksMapping.Add("00000001", 0);
            blocksMapping.Add("00000100", 0);
            blocksMapping.Add("00100000", 0);
            blocksMapping.Add("10000000", 0);
            blocksMapping.Add("00100100", 0);
            blocksMapping.Add("10000001", 0);
            blocksMapping.Add("10100000", 0);
            blocksMapping.Add("00100001", 0);
            blocksMapping.Add("00000101", 0);
            blocksMapping.Add("10000100", 0);
            blocksMapping.Add("10100001", 0);
            blocksMapping.Add("10100100", 0);
            blocksMapping.Add("10000101", 0);
            blocksMapping.Add("00100101", 0);
            blocksMapping.Add("10100101", 0);
            blocksMapping.Add("00000010", 1);
            blocksMapping.Add("00000011", 1);
            blocksMapping.Add("00000110", 1);
            blocksMapping.Add("00100010", 1);
            blocksMapping.Add("10000010", 1);
            blocksMapping.Add("00000111", 1);
            blocksMapping.Add("00100011", 1);
            blocksMapping.Add("10000011", 1);
            blocksMapping.Add("00100110", 1);
            blocksMapping.Add("10000110", 1);
            blocksMapping.Add("10100010", 1);
            blocksMapping.Add("00100111", 1);
            blocksMapping.Add("10000111", 1);
            blocksMapping.Add("10100011", 1);
            blocksMapping.Add("10100110", 1);
            blocksMapping.Add("10100111", 1);
            blocksMapping.Add("00001000", 2);
            blocksMapping.Add("00001001", 2);
            blocksMapping.Add("00001100", 2);
            blocksMapping.Add("00101000", 2);
            blocksMapping.Add("10001000", 2);
            blocksMapping.Add("00001101", 2);
            blocksMapping.Add("00101001", 2);
            blocksMapping.Add("10001001", 2);
            blocksMapping.Add("00101100", 2);
            blocksMapping.Add("10001100", 2);
            blocksMapping.Add("10101000", 2);
            blocksMapping.Add("00101101", 2);
            blocksMapping.Add("10001101", 2);
            blocksMapping.Add("10101001", 2);
            blocksMapping.Add("10101100", 2);
            blocksMapping.Add("10101101", 2);
            blocksMapping.Add("00001010", 3);
            blocksMapping.Add("10001010", 3);
            blocksMapping.Add("00101010", 3);
            blocksMapping.Add("00001110", 3);
            blocksMapping.Add("10101010", 3);
            blocksMapping.Add("10001110", 3);
            blocksMapping.Add("00101110", 3);
            blocksMapping.Add("10101110", 3);
            blocksMapping.Add("00001011", 4);
            blocksMapping.Add("00001111", 4);
            blocksMapping.Add("00101111", 4);
            blocksMapping.Add("00101011", 4);
            blocksMapping.Add("10001011", 4);
            blocksMapping.Add("10101111", 4);
            blocksMapping.Add("10001111", 4);
            blocksMapping.Add("10101011", 4);
            blocksMapping.Add("00010000", 5);
            blocksMapping.Add("00010001", 5);
            blocksMapping.Add("00010100", 5);
            blocksMapping.Add("00110000", 5);
            blocksMapping.Add("10010000", 5);
            blocksMapping.Add("00010101", 5);
            blocksMapping.Add("00110001", 5);
            blocksMapping.Add("10010001", 5);
            blocksMapping.Add("00110100", 5);
            blocksMapping.Add("10010100", 5);
            blocksMapping.Add("10110000", 5);
            blocksMapping.Add("00110101", 5);
            blocksMapping.Add("10010101", 5);
            blocksMapping.Add("10110001", 5);
            blocksMapping.Add("10110100", 5);
            blocksMapping.Add("10110101", 5);
            blocksMapping.Add("00010010", 6);
            blocksMapping.Add("10010010", 6);
            blocksMapping.Add("00110010", 6);
            blocksMapping.Add("00010011", 6);
            blocksMapping.Add("00110011", 6);
            blocksMapping.Add("10010011", 6);
            blocksMapping.Add("10110010", 6);
            blocksMapping.Add("10110011", 6);
            blocksMapping.Add("00010110", 7);
            blocksMapping.Add("00010111", 7);
            blocksMapping.Add("10010111", 7);
            blocksMapping.Add("10010110", 7);
            blocksMapping.Add("10110111", 7);
            blocksMapping.Add("00110111", 7);
            blocksMapping.Add("10110110", 7);
            blocksMapping.Add("00110110", 7);
            blocksMapping.Add("00011000", 8);
            blocksMapping.Add("00011100", 8);
            blocksMapping.Add("00011001", 8);
            blocksMapping.Add("10011000", 8);
            blocksMapping.Add("00111000", 8);
            blocksMapping.Add("00011101", 8);
            blocksMapping.Add("00111001", 8);
            blocksMapping.Add("10011001", 8);
            blocksMapping.Add("00111100", 8);
            blocksMapping.Add("10011100", 8);
            blocksMapping.Add("10111000", 8);
            blocksMapping.Add("10111100", 8);
            blocksMapping.Add("10111001", 8);
            blocksMapping.Add("10011101", 8);
            blocksMapping.Add("00111101", 8);
            blocksMapping.Add("10111101", 8);
            blocksMapping.Add("00011010", 9);
            blocksMapping.Add("10011010", 9);
            blocksMapping.Add("00111010", 9);
            blocksMapping.Add("10111010", 9);
            blocksMapping.Add("00011011", 10);
            blocksMapping.Add("10011011", 10);
            blocksMapping.Add("00111011", 10);
            blocksMapping.Add("10111011", 10);
            blocksMapping.Add("00011110", 11);
            blocksMapping.Add("10011110", 11);
            blocksMapping.Add("00111110", 11);
            blocksMapping.Add("10111110", 11);
            blocksMapping.Add("00011111", 12);
            blocksMapping.Add("10011111", 12);
            blocksMapping.Add("00111111", 12);
            blocksMapping.Add("10111111", 12);
            blocksMapping.Add("01000000", 13);
            blocksMapping.Add("01000001", 13);
            blocksMapping.Add("01000100", 13);
            blocksMapping.Add("01100000", 13);
            blocksMapping.Add("11000000", 13);
            blocksMapping.Add("01000101", 13);
            blocksMapping.Add("01100001", 13);
            blocksMapping.Add("11000001", 13);
            blocksMapping.Add("01100100", 13);
            blocksMapping.Add("11000100", 13);
            blocksMapping.Add("11100000", 13);
            blocksMapping.Add("01100101", 13);
            blocksMapping.Add("11000101", 13);
            blocksMapping.Add("11100001", 13);
            blocksMapping.Add("11100100", 13);
            blocksMapping.Add("11100101", 13);
            blocksMapping.Add("01000010", 14);
            blocksMapping.Add("01000110", 14);
            blocksMapping.Add("01000011", 14);
            blocksMapping.Add("11000010", 14);
            blocksMapping.Add("01100010", 14);
            blocksMapping.Add("01000111", 14);
            blocksMapping.Add("01100011", 14);
            blocksMapping.Add("11000011", 14);
            blocksMapping.Add("01100110", 14);
            blocksMapping.Add("11000110", 14);
            blocksMapping.Add("11100010", 14);
            blocksMapping.Add("11100110", 14);
            blocksMapping.Add("11100011", 14);
            blocksMapping.Add("11000111", 14);
            blocksMapping.Add("01100111", 14);
            blocksMapping.Add("11100111", 14);
            blocksMapping.Add("01001000", 15);
            blocksMapping.Add("01001001", 15);
            blocksMapping.Add("01001100", 15);
            blocksMapping.Add("11001000", 15);
            blocksMapping.Add("01001101", 15);
            blocksMapping.Add("11001001", 15);
            blocksMapping.Add("11001100", 15);
            blocksMapping.Add("11001101", 15);
            blocksMapping.Add("01001010", 16);
            blocksMapping.Add("01001110", 16);
            blocksMapping.Add("11001010", 16);
            blocksMapping.Add("11001110", 16);
            blocksMapping.Add("01001011", 17);
            blocksMapping.Add("11001011", 17);
            blocksMapping.Add("01001111", 17);
            blocksMapping.Add("11001111", 17);
            blocksMapping.Add("01010000", 18);
            blocksMapping.Add("01010001", 18);
            blocksMapping.Add("01010100", 18);
            blocksMapping.Add("01110000", 18);
            blocksMapping.Add("01010101", 18);
            blocksMapping.Add("01110100", 18);
            blocksMapping.Add("01110001", 18);
            blocksMapping.Add("01110101", 18);
            blocksMapping.Add("01010010", 19);
            blocksMapping.Add("01010011", 19);
            blocksMapping.Add("01110010", 19);
            blocksMapping.Add("01110011", 19);
            blocksMapping.Add("01010110", 20);
            blocksMapping.Add("01110110", 20);
            blocksMapping.Add("01010111", 20);
            blocksMapping.Add("01110111", 20);
            blocksMapping.Add("01011000", 21);
            blocksMapping.Add("01011001", 21);
            blocksMapping.Add("01011100", 21);
            blocksMapping.Add("01011101", 21);
            blocksMapping.Add("01011010", 22);
            blocksMapping.Add("01011011", 23);
            blocksMapping.Add("01011110", 24);
            blocksMapping.Add("01011111", 25);
            blocksMapping.Add("01101000", 26);
            blocksMapping.Add("11101000", 26);
            blocksMapping.Add("01101001", 26);
            blocksMapping.Add("11101001", 26);
            blocksMapping.Add("11101100", 26);
            blocksMapping.Add("11101101", 26);
            blocksMapping.Add("01101101", 26);
            blocksMapping.Add("01101100", 26);
            blocksMapping.Add("01101010", 27);
            blocksMapping.Add("11101010", 27);
            blocksMapping.Add("01101110", 27);
            blocksMapping.Add("11101110", 27);
            blocksMapping.Add("01101011", 28);
            blocksMapping.Add("01101111", 28);
            blocksMapping.Add("11101011", 28);
            blocksMapping.Add("11101111", 28);
            blocksMapping.Add("01111000", 29);
            blocksMapping.Add("01111001", 29);
            blocksMapping.Add("01111101", 29);
            blocksMapping.Add("01111100", 29);
            blocksMapping.Add("01111010", 30);
            blocksMapping.Add("01111011", 31);
            blocksMapping.Add("01111110", 32);
            blocksMapping.Add("01111111", 33);
            blocksMapping.Add("11010000", 34);
            blocksMapping.Add("11010100", 34);
            blocksMapping.Add("11110100", 34);
            blocksMapping.Add("11110000", 34);
            blocksMapping.Add("11010001", 34);
            blocksMapping.Add("11110001", 34);
            blocksMapping.Add("11110101", 34);
            blocksMapping.Add("11010101", 34);
            blocksMapping.Add("11010010", 35);
            blocksMapping.Add("11010011", 35);
            blocksMapping.Add("11110010", 35);
            blocksMapping.Add("11110011", 35);
            blocksMapping.Add("11010110", 36);
            blocksMapping.Add("11010111", 36);
            blocksMapping.Add("11110110", 36);
            blocksMapping.Add("11110111", 36);
            blocksMapping.Add("11011000", 37);
            blocksMapping.Add("11011100", 37);
            blocksMapping.Add("11011001", 37);
            blocksMapping.Add("11011101", 37);
            blocksMapping.Add("11011010", 38);
            blocksMapping.Add("11011011", 39);
            blocksMapping.Add("11011110", 40);
            blocksMapping.Add("11011111", 41);
            blocksMapping.Add("11111000", 42);
            blocksMapping.Add("11111100", 42);
            blocksMapping.Add("11111001", 42);
            blocksMapping.Add("11111101", 42);
            blocksMapping.Add("11111010", 43);
            blocksMapping.Add("11111011", 44);
            blocksMapping.Add("11111110", 45);
            blocksMapping.Add("11111111", 46);
        }

        public static Block GetBlock(byte id,string textureNumber) {
            if (id==0) {
                if (textureNumber == "11111111") {
                    if (Random.Range(0, 10) > 7) {
                        return BlockList2.grassBlocks[(byte)Random.Range(0,14)];
                    } else {
                        return grassBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                    }
                } else {
                    return grassBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                }

                //if (blocksMapping.ContainsKey(textureNumber)) {
                //    if (textureNumber == "11111111") {
                //        return grassBlocks[63];
                //    } else {
                //        return sandBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                //    }
                //} else {
                //    return grassBlocks[46];
                //}
            } else if (id == 1) {
                return grassBlocks[63];

                //return BlockList2.blocks.ContainsKey(id) ? BlockList2.blocks[id] : null;
            } else if (id == 2) {
                return snowBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                //if (blocksMapping.ContainsKey(textureNumber)) {
                //    if (textureNumber == "11111111") {
                //        return grassBlocks[63];
                //    } else {
                //        return sandBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                //    }
                //} else {
                //    return grassBlocks[46];
                //}
            } else if (id == 3) {
                return grassBlocks[63];
            } else if (id == 4) {
                if (textureNumber== "11111111") {
                    return grassBlocks[63];
                } else {
                    return sandBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                }
            } else if (id == 5) {
                if (textureNumber == "11111111") {
                    return grassBlocks[63];
                } else {
                    return sandBlocks[(byte)blocksMapping.TryGet(textureNumber)];
                }
            } else {
                return grassBlocks[63];
            }
        }
    }
}