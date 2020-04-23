using AILinkFactoryAuto.Core;
using AILinkFactoryAuto.Task.Executer;
using AILinkFactoryAuto.Task.Property;
using AILinkFactoryAuto.Task.SmartBracelet.Property;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AILinkFactoryAuto.Task.SmartBracelet.Executer
{
    public class ReadMapExecuter : IExecuter
    {
        ReadMapProperties config;
        private GlobalVaribles configGv;
        public void Run(IProperties properties, GlobalDic<string, object> globalDic)//virtual
        {
            config = properties as ReadMapProperties;
            ILog log = globalDic[typeof(ILog).ToString()] as ILog;

            //int ret = -1;
            string mapFileString = "";
            try
            {
                mapFileString = Read(config.MapFilePath);
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("读取MAP文件失败,异常：{0}", ex.Message));
            }

            log.Info(string.Format("读取MAP文件成功：\r\n{0}", mapFileString));
            //更新MAP内存中的MAC地址
            string[] splits = { "\r\n" };
            string[] mapFileStringLines = mapFileString.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            //IMES获取的MAC***************
            configGv = globalDic[typeof(GlobalVaribles).ToString()] as GlobalVaribles;
            string macFromImes = configGv.Get("MAC");

            //更新MAP
            //byte[] bytesMac = strToToHexByte(macFromImes);
            //byte[] bytesReverseMac = bytesConvert(bytesMac);
            //string stringReverseMac = byteToHexStr(bytesReverseMac);
            //string reverseMacAddSpace = AddSpaceEachTwoBtye(stringReverseMac);


            string reverseMacAddSpace = AddSpaceEachTwoBtye(macFromImes);

            string mapFileStringMacUpdate = "";
            for (int i = 0; i < mapFileStringLines.Length; i++)
            {
                if (i == 17)
                {
                    //前面固定部分+反转并添加空格的MAC
                    mapFileStringLines[i] = mapFileStringLines[i].Substring(0, 25) + reverseMacAddSpace;
                }
                mapFileStringMacUpdate += mapFileStringLines[i] + "\r\n";
            }

            //存为byte数组应该会好些，这样可以定位地址，当MAC的偏移地址发生变化时,只需改地址值
            log.Info(string.Format("更新MAP文件中的MAC成功：\r\n{0}", mapFileStringMacUpdate));
            GlobalVaribles configGv1 = globalDic.Get<GlobalVaribles>();
            configGv1.Add("MapFileStringMacUpdate", mapFileStringMacUpdate);
            //globalDic.Add(typeof(OpenPhoneProperties).ToString(), config);

        }
        /// <summary>
        /// 字符串每两个字节添加空格
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string AddSpaceEachTwoBtye(string strValue)
        {
            for (int i = 4; i < strValue.Length; i = i + 4)
            {
                strValue = strValue.Insert(i, " ");
                i++;
            }
            return strValue;
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 字节数组高低字节互换
        /// </summary>
        /// <param name="bytesValue"></param>
        /// <returns></returns>
        public static byte[] bytesConvert(byte[] bytesValue)
        {
            byte[] data2 = new byte[bytesValue.Length];
            for (int i = 0; i < bytesValue.Length; i += 2)
            {
                data2[i] = bytesValue[i + 1];
                data2[i + 1] = bytesValue[i];
            }
            return data2;
        }

        /// <summary>
        /// 字符串高低位字节互换
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string strConvert(string strValue)
        {
            int intLength = strValue.Length;
            string res = string.Empty;
            for (int i = 0; i < intLength / 2; i++)
            {
                res += strValue.Substring(intLength - 2 * (i + 1), 2);
            }
            return res;
        }

        public string Read(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                String fileContent = "";
                while ((line = sr.ReadLine()) != null)
                {
                    fileContent += line + "\r\n";
                }
                sr.Dispose();
                sr.Close();
                return fileContent;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
