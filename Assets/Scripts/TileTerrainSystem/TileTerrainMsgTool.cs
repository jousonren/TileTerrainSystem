using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public static class TileTerrainMsgTool {
    //Convert.ToString(12,2); // 将12转为2进制字符串，结果 “1100”
    //Convert.ToInt("1100",2); // 将2进制字符串转为整数，结果 12

    public static string HeightIntBits;
    public static string HeightFloatBits;
    public static string TextureBits;
    /// <summary>
    /// 一个格子的信息转换成两个byte后的数据
    /// </summary>
    private static byte[] tileByteMsg = new byte[2];
    /// <summary>
    /// 存储临时数据
    /// </summary>
    private static string byteString;
    /// <summary>
    /// 将一个格子的信息转换成长度为2的byte数组
    /// </summary>
    /// <param name="heightIntNumber">高度整数部分代号</param>
    /// <param name="heightFloatNumber">高度浮点数部分代号</param>
    /// <param name="TextureNumber">贴图代号</param>
    /// <returns></returns>
    public static byte[] TileMsgToByte(int heightIntNumber, int heightFloatNumber, int TextureNumber) {
        HeightIntBits = Convert.ToString(heightIntNumber, 2).PadLeft(5, '0');
        HeightFloatBits = Convert.ToString(heightFloatNumber, 2).PadLeft(7, '0');
        TextureBits = Convert.ToString(TextureNumber, 2).PadLeft(3, '0');
        byteString = HeightIntBits + HeightFloatBits.Substring(0, 3);
        tileByteMsg[0] = DecodeBinaryString(byteString);
        byteString = HeightFloatBits.Substring(3, 4) + TextureBits + "0";
        tileByteMsg[1] = DecodeBinaryString(byteString);
        return tileByteMsg;
    }

    /// <summary>
    /// 将长度为2的byte数组转换成一个格子的信息
    /// </summary>
    /// <param name="byteMsg"></param>
    /// <param name="heightIntNumber"></param>
    /// <param name="heightFloatNumber"></param>
    /// <param name="TextureNumber"></param>
    public static void ByteToTileMsg(byte[] byteMsg,out int heightIntNumber,out int heightFloatNumber,out int TextureNumber) {
        tileByteMsg[0] = byteMsg[0];
        tileByteMsg[1] = byteMsg[1];
        byteString = Convert.ToString(tileByteMsg[0], 2).PadLeft(8, '0') + Convert.ToString(tileByteMsg[1], 2).PadLeft(8, '0');
        heightIntNumber = Convert.ToInt16(byteString.Substring(0,5), 2);
        heightFloatNumber = Convert.ToInt16(byteString.Substring(5, 7), 2);
        TextureNumber = Convert.ToInt16(byteString.Substring(12, 3), 2);
    }
    /// <summary>
    /// bit字符串转byte
    /// </summary>
    /// <param name="byteStr"></param>
    /// <returns></returns>
    private static byte DecodeBinaryString(String byteStr) {
        int re, len;
        if (null == byteStr) {
            return 0;
        }
        len = byteStr.Length;
        if (len != 4 && len != 8) {
            return 0;
        }
        if (len == 8) {// 8 bit处理  
            if (byteStr.ToCharArray()[0] == '0') {// 正数  
                re = Convert.ToInt32(byteStr, 2);
            } else {// 负数  
                re = Convert.ToInt32(byteStr, 2) - 256;
            }
        } else {// 4 bit处理  
            re = Convert.ToInt32(byteStr, 2);
        }
        return (byte)re;
    }
}
