using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace RegistrationTrialPeriod
{
    public class RegistrationTrialPeriod
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string ToEncrypt(string encryptKey, string str)
        {
            try
            {
                byte[] P_byte_key = //将密钥字符串转换为字节序列
                    Encoding.Unicode.GetBytes(encryptKey);
                byte[] P_byte_data = //将字符串转换为字节序列
                    Encoding.Unicode.GetBytes(str);
                MemoryStream P_Stream_MS = //创建内存流对象
                    new MemoryStream();
                CryptoStream P_CryptStream_Stream = //创建加密流对象
                    new CryptoStream(P_Stream_MS, new DESCryptoServiceProvider().
                   CreateEncryptor(P_byte_key, P_byte_key), CryptoStreamMode.Write);
                P_CryptStream_Stream.Write(//向加密流中写入字节序列
                    P_byte_data, 0, P_byte_data.Length);
                P_CryptStream_Stream.FlushFinalBlock();//将数据压入基础流
                byte[] P_bt_temp =//从内存流中获取字节序列
                    P_Stream_MS.ToArray();
                P_CryptStream_Stream.Close();//关闭加密流
                P_Stream_MS.Close();//关闭内存流
                return //方法返回加密后的字符串
                    Convert.ToBase64String(P_bt_temp);
            }
            catch (CryptographicException ce)
            {
                throw new Exception(ce.Message);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string ToDecrypt(string encryptKey, string str)
        {
            try
            {
                byte[] P_byte_key = //将密钥字符串转换为字节序列
                    Encoding.Unicode.GetBytes(encryptKey);
                byte[] P_byte_data = //将加密后的字符串转换为字节序列
                    Convert.FromBase64String(str);
                MemoryStream P_Stream_MS =//创建内存流对象并写入数据
                    new MemoryStream(P_byte_data);
                CryptoStream P_CryptStream_Stream = //创建加密流对象
                    new CryptoStream(P_Stream_MS, new DESCryptoServiceProvider().
                    CreateDecryptor(P_byte_key, P_byte_key), CryptoStreamMode.Read);
                byte[] P_bt_temp = new byte[200];//创建字节序列对象
                MemoryStream P_MemoryStream_temp =//创建内存流对象
                    new MemoryStream();
                int i = 0;//创建记数器
                while ((i = P_CryptStream_Stream.Read(//使用while循环得到解密数据
                    P_bt_temp, 0, P_bt_temp.Length)) > 0)
                {
                    P_MemoryStream_temp.Write(//将解密后的数据放入内存流
                        P_bt_temp, 0, i);
                }
                return //方法返回解密后的字符串
                    Encoding.Unicode.GetString(P_MemoryStream_temp.ToArray());
            }
            catch (CryptographicException ce)
            {
                throw new Exception(ce.Message);
            }
        }


        /// <summary>
        /// 注册试用期
        /// </summary>
        public static bool Register()
        {
            try
            {
                string file_Regist = Application.StartupPath + "\\RegistrationTrialPeriod.rgtp";

                //文件不存在
                if (!File.Exists(file_Regist))
                {
                    string usetime = System.DateTime.Now.AddDays(90).ToLongDateString();
                    string jiami = ToEncrypt("youc", usetime);
                    File.WriteAllText(file_Regist, jiami);
                    File.SetAttributes(file_Regist, FileAttributes.Hidden);//设置添加隐藏文件
                    MessageBox.Show("您可以免费试用软件90天！", "感谢您首次使用");
                }
                else
                {
                    string jiemi = ToDecrypt("youc", File.ReadAllText(file_Regist));
                    DateTime usetime = Convert.ToDateTime(jiemi);
                    DateTime daytime = DateTime.Parse(System.DateTime.Now.ToLongDateString());
                    TimeSpan ts = usetime - daytime;
                    int day = ts.Days;
                    if (day <= 0)
                    {
                        if (MessageBox.Show("软件试用期已到，请注册后再使用！", "提示", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("本软件的试用期还有" + day.ToString() + "天！", "提示");
                    }
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("软件试用期已到，请注册后再使用！", "提示", MessageBoxButtons.OK);
                return false;
            }
        } 
    }
}
