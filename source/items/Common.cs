using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Tunneln.items.connections;

namespace Tunneln.items
{
    // https://www.paveway.info/entry/2019/04/08/csharp_encrypt
    class EncryptUtils
    {
        private static readonly string AesIV = Common.GetHashedTextString("AesIV").Substring(0,16); // 半角16文字のランダムな文字列にします。
        private static readonly string AesKey = Common.GetHashedTextString("AesKey").Substring(0, 32); // 半角32文字のランダムな文字列にします。
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;

        private EncryptUtils() { }

        public static string Encrypt(string value)
        {
            if (value is null){ value = ""; }
            var aes = GetAesManaged();
            var byteValue = Encoding.UTF8.GetBytes(value);
            var byteLength = byteValue.Length;
            var encryptor = aes.CreateEncryptor();
            var encryptValue = encryptor.TransformFinalBlock(byteValue, 0, byteLength);
            var base64Value = Convert.ToBase64String(encryptValue);
            return base64Value;
        }

        public static string Decrypt(string encryptValue)
        {
            string ret = null;
            try
            {
                var aes = GetAesManaged();
                var byteValue = Convert.FromBase64String(encryptValue);
                var byteLength = byteValue.Length;
                var decryptor = aes.CreateDecryptor();
                var decryptValue = decryptor.TransformFinalBlock(byteValue, 0, byteLength);
                ret = Encoding.UTF8.GetString(decryptValue);
            }
            catch
            {
            }
            return ret;
        }

        private static AesManaged GetAesManaged()
        {
            var aes = new AesManaged();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }

    class Common
    {
        public static void ButtonAdd(Button button)
        {
            button.Content = "✚";
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.FontSize = 15;
            button.Height = 20;
            button.Width = 20;
            button.Padding = new Thickness(0,0,0,0);
        }

        // 文字列のハッシュ値（SHA256）を計算・取得する
        public static string GetHashedTextString(string txt)
        {
            byte[] byteValues = Encoding.UTF8.GetBytes(txt);
            SHA256 crypto256 = new SHA256CryptoServiceProvider();
            byte[] hash256Value = crypto256.ComputeHash(byteValues);
            StringBuilder hashedText = new StringBuilder();
            for (int i = 0; i < hash256Value.Length; i++)
            {
                hashedText.AppendFormat("{0:X2}", hash256Value[i]);
            }
            return hashedText.ToString();
        }

        // IPv4文字列を2進数32桁の文字列へ変換する
        public static string IPv4ToBinary(string txt)
        {
            try
            {
                string ret = "";
                foreach (string _int_str in txt.Split('.'))
                {
                    ret += Convert.ToString(Int32.Parse(_int_str), 2).PadLeft(8, '0');

                }
                return ret;

            }
            catch
            {
                return null;
            }
        }

        // 2進数32桁の文字列をIPv4文字列へ変換する
        public static string BinaryToIPv4(string txt)
        {
            try
            {
                List<string> ret = new List<string>();
                for (int ii=0; ii<4; ii++)
                {
                    string _tmp = txt.Substring(ii * 8, 8);
                    ret.Add(Convert.ToInt32(_tmp, 2).ToString());
                }
                return string.Join('.', ret);

            }
            catch
            {
                return null;
            }
        }


        public static string GenerateRandomIPv4Address(string subnet = "127.0.0.0/8")
        {
            string _seg_ip   = subnet.Split('/')[0];
            string _seg_mask = subnet.Split('/')[1];
            if (Int32.TryParse(_seg_mask, out int host_mask))
            {
                string _ip_bin = Common.IPv4ToBinary(_seg_ip).Substring(0, host_mask);
                Random r = new System.Random();
                int no = r.Next(0, 2 << (32 - host_mask));
                _ip_bin += Convert.ToString(no, 2);
                _ip_bin = _ip_bin.PadRight(32,'0');
                return Common.BinaryToIPv4(_ip_bin);
            }
            return null;
        }

        public static Regex VALID_IP = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$", RegexOptions.Compiled);

        public static IPAddress GetIPv4AddressInLocal(string txt)
        {
            IPAddress _ip = null;
            if (Common.VALID_IP.IsMatch(txt))
            {
                /* IPv4 address */
                IPAddress.TryParse(txt, out _ip);
            }
            else if (txt.Length > 0)
            {
                /* Host Name ? */
                try
                {
                    string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
                    List<string> in_hosts = new List<string>() { "localhost" };
                    if (File.Exists(hostsFilePath))
                    {
                        foreach (string line in File.ReadAllLines(hostsFilePath))
                        {
                            if (!(line.StartsWith('#')) && (line.Length > 0))
                            {
                                string enable_line = Regex.Replace(line, @"\s*\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\s*", "");
                                Match hostname = Regex.Match(enable_line, @"[^\s]+");
                                in_hosts.Add(hostname.ToString());
                            }
                        }
                        if (in_hosts.Contains(txt))
                        {
                            IPHostEntry ipHostInfo = Dns.GetHostEntry(txt);
                            _ip = ipHostInfo.AddressList[0];
                        }
                    }

                }
                catch
                {

                }
            }
            return _ip;
        }

        public static IPAddress GetIPv4Addressl(string txt)
        {
            IPAddress _ip = null;
            if (Common.VALID_IP.IsMatch(txt))
            {
                /* IPv4 address */
                IPAddress.TryParse(txt, out _ip);
            }
            else if (txt.Length > 0)
            {
                /* Host Name ? */
                try
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(txt);
                    _ip = ipHostInfo.AddressList[0];
                }
                catch
                {
                }
            }
            return _ip;
        }

    }
}
