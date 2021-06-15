using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;

namespace Apzon.PosmanErp.Commons
{
    public class Function
    {
        /// <summary>
        /// hàm parse object to datetime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ParseDateTimes(object obj, string format = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(format))
                {
                    return DateTime.ParseExact(obj.ToString(), format, CultureInfo.InvariantCulture);
                }
                DateTime result;
                if (obj == null) return DateTime.Now.Date;
                //if (obj != null && DateTime.Parse(obj.ToString()).Year == 1899) return null;
                //if (obj != null && DateTime.Parse(obj.ToString()).Year == 1900) return null;
                if (DateTime.TryParse(obj.ToString(), out result))
                    return result;
                return DateTime.Now.Date;
            }
            catch (Exception)
            {
                return DateTime.Now.Date;
            }
        }

        /// <summary>
        /// hàm parse object to string
        /// exception return ""
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToString(object obj)
        {
            try
            {
                if (obj == null) return string.Empty;
                return obj.ToString().Trim();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// parse object to int
        /// exception return 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ParseInt(object obj)
        {
            try
            {
                int result;
                if (obj == null) return 0;
                if (int.TryParse(obj.ToString(), out result))
                    return result;
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal ParseDecimal(object obj)
        {
            try
            {
                decimal result;
                if (obj == null) return 0;
                var value = obj.ToString();
                if (decimal.TryParse(value, out result))
                    return result;
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                    return result;
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// hàm parrse object to datetime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTime(object obj, string format = "")
        {
            try
            {
                if (obj == null) return null;
                if (!string.IsNullOrEmpty(format))
                {
                    return DateTime.ParseExact(obj.ToString(), format, CultureInfo.InvariantCulture);
                }
                DateTime result;
                //if (obj != null && DateTime.Parse(obj.ToString()).Year == 1899) return null;
                //if (obj != null && DateTime.Parse(obj.ToString()).Year == 1900) return null;
                if (DateTime.TryParse(obj.ToString(), out result))
                    return result;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// hash md5 freetext
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string input)
        {
            try
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                var md5Hasher = MD5.Create();

                // Convert the input string to a byte array and compute the hash.
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return "";
            }
        }

        /// <summary>
        /// ham check system type
        /// </summary>
        /// <returns></returns>
        private static OperatingSystemType GetSystemType()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? OperatingSystemType.Linux
                : OperatingSystemType.Windows;
        }

        /// <summary>
        /// hàm get hardwarekey trên hệ thống
        /// </summary>
        /// <returns></returns>
        public static string GetHardwareKey()
        {
            var result = "";
            try
            {
                if(GetSystemType() == OperatingSystemType.Windows)
                {
                    #region lấy thông tin serial BIOS
                    var mc = new ManagementClass("Win32_BIOS");
                    var moc = mc.GetInstances();
                    foreach (var mo in moc)
                    {
                        if (result == "")
                        {
                            result = ToString(mo["SerialNumber"]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(result))
                            {
                                mo.Dispose();
                                break;
                            }
                        }
                        mo.Dispose();
                    }
                    #endregion

                    #region THông tin Processor ID
                    mc = new ManagementClass("Win32_Processor");
                    moc = mc.GetInstances();
                    foreach (var mo in moc)
                    {
                        var cpu = ToString(mo.Properties["processorID"].Value).Replace(" ", "") + ToString(mo.Properties["UniqueId"].Value).Replace(" ", "");
                        if (!string.IsNullOrEmpty(cpu))
                        {
                            result += cpu;
                            mo.Dispose();
                            break;
                        }
                        mo.Dispose();
                    }
                    #endregion

                    #region thông tin Serial Number base board
                    mc = new ManagementClass("Win32_BaseBoard");
                    moc = mc.GetInstances();
                    foreach (var mo in moc)
                    {
                        var baseBoard = ToString(mo.Properties["SerialNumber"].Value).Replace(" ", "");
                        if (!string.IsNullOrEmpty(baseBoard))
                        {
                            result += baseBoard;
                            mo.Dispose();
                            break;
                        }
                        mo.Dispose();
                    }
                    #endregion

                    #region computer name as number
                    var computerName = Function.ToString(Environment.MachineName).Replace(" ", "");
                    if (!string.IsNullOrEmpty(computerName))
                    {
                        var ar = computerName.ToCharArray();
                        result = ar.Aggregate(result, (current, t) => current + (int)t);
                    }
                    #endregion
                }
                else
                {
                    return "queiq46Nz8OGIg" + Environment.MachineName;
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                result = "";
            }
            return result;
        }


        private static int GetMaxDataLength()
        {
            if (Constants.RSA.fOAEP)
                return ((Constants.RSA.KEY_SIZE - 384) / 8) + 7;
            return ((Constants.RSA.KEY_SIZE - 384) / 8) + 37;
        }

        // kiem tra do dai cua key
        private static bool IsKeySizeValid()
        {
            return Constants.RSA.KEY_SIZE >= 384 &&
                   Constants.RSA.KEY_SIZE <= 16384 &&
                   Constants.RSA.KEY_SIZE % 8 == 0;
        }

        /// <summary>
        /// mã hóa bản rõ
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publickey"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string publickey = "")
        {
            if (string.IsNullOrEmpty(publickey))
            {
                publickey = Constants.RSA.PublicKey;
            }
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Can not encryt data");


            var maxLength = GetMaxDataLength();
            if (Encoding.Unicode.GetBytes(plainText).Length > maxLength)
                throw new ArgumentException("Can not encryt data");


            if (!IsKeySizeValid())
                throw new ArgumentException("Can not encryt data");


            if (string.IsNullOrWhiteSpace(publickey))
                throw new ArgumentException("Can not encryt data");


            string encryptedText;

            try
            {
                using (var rsaProvider = RSA.Create())
                {
                    rsaProvider.FromXmlString(publickey);
                    var plainBytes = Encoding.Unicode.GetBytes(plainText);
                    var encryptedBytes = rsaProvider.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);
                    encryptedText = Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                throw new Exception("Could not encryt data!");

            }
            return encryptedText;

        }

        /// <summary>
        /// hàm parser key as string to RSAParameters
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static RSAParameters ParseXmlString(string xml)
        {
            RSAParameters parameters = new RSAParameters();
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xml);
            if (!xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                throw new Exception("Invalid XML RSA key.");
            }
            foreach (System.Xml.XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                }
            }
            return parameters;
        }

        /// <summary>
        /// giải mã
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="privatekey"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string privatekey = "")
        {
            if (string.IsNullOrEmpty(privatekey))
            {
                privatekey = Constants.RSA.PrivateKey;
            }

            if (string.IsNullOrWhiteSpace(encryptedText))
                throw new ArgumentException("Can not Decrypt data");

            if (!IsKeySizeValid())
                throw new ArgumentException("Can not Decrypt data");

            if (string.IsNullOrWhiteSpace(privatekey))
                throw new ArgumentException("Can not Decrypt data");

            var plainText = "";


            try
            {
                using (var rsaProvider = RSA.Create())
                {
                    rsaProvider.FromXmlString(privatekey);
                    var encryptedBytes = Convert.FromBase64String(encryptedText);
                    var plainBytes = rsaProvider.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                    plainText = Encoding.Unicode.GetString(plainBytes);
                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                throw new Exception("Could not Decrypt data");


            }

            return plainText;

        }

        /// <summary>
        /// hàm đoc license file
        /// </summary>
        /// <param name="file">nội dung file license</param>
        /// <param name="bpCode">thông tin bp của license</param>
        /// <param name="installNumber">Tông tin installnumber của file license</param>
        /// <param name="hardwareKey">THông tin hardwarekey</param>
        /// <param name="fileId">Thông tin file ID</param>
        /// <param name="sysType">Thông tin license System Type</param>
        /// <param name="dateCreate">Thông tin ngày tạo license</param>
        /// <param name="version">Version IRS license</param>
        /// <param name="lstLicense">THông tin dữ liệu license của đọc từ file</param>
        public static void ReadLicense(string file, out string bpCode, out string installNumber,
                            out string hardwareKey, out string fileId, out string sysType, out string dateCreate, out string version, out List<LicenseInfoImpl> lstLicense)
        {
            //lưu danh sách license của file
            lstLicense = new List<LicenseInfoImpl>();

            using (var reader = new StringReader(file))
            {
                reader.ReadLine();
                //lấy thông tin BP Code
                bpCode = reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                //lấy thông tin hardwarekey
                hardwareKey = ReadLicensePath(reader.ReadLine(), reader);
                //thông tin installnumber
                installNumber = ReadLicensePath(reader.ReadLine(), reader);
                //thông tin fileId
                fileId = ReadLicensePath(reader.ReadLine(), reader);
                //thông tin system type
                sysType = ReadLicensePath(reader.ReadLine(), reader);
                //thông tin version
                version = ReadLicensePath(reader.ReadLine(), reader);
                //thông tin license create date
                dateCreate = ReadLicensePath(reader.ReadLine(), reader);
                //đọc dữ liệu config về license type bao gồm license code , license name, license rule, số lượng user, expired Date
                var plainInfo = ReadLicensePath(reader.ReadLine(), reader);
                //một file license có nhiều license type khác nhau
                //sử dụng vòng lặp để đọc cho tời khi kết thúc file license
                //dấu hiệu kết thúc license là ENDLICENSE
                while (!plainInfo.Equals("ENDLICENSE"))
                {
                    #region lấy dữ liệu license type
                    var arrInfo = plainInfo.Split(';');
                    var itemInfo = new LicenseInfoImpl();
                    itemInfo.type_code = arrInfo[0];
                    itemInfo.type_name = arrInfo[1];
                    itemInfo.amount = ParseInt(arrInfo[2]);
                    itemInfo.expiry_date = ParseDateTimes(arrInfo[3], "yyyyMMdd");
                    //license type dược chia làm 3 loại là: Super có thể truy cập tất cả các form 
                    //- Normal truy cập một số chức năng chuẩn theo danh sách được setting khi gen license, và các form UDO, UDT, query, report, user defined form....
                    //- service không có quyền truy cập bất cứ chức năng nào ở Client. Loại license này nhằm phục vụ cho các service ở API
                    itemInfo.license_rule = arrInfo[4].Equals("S") ? LicenseRule.Super : arrInfo[4].Equals("X") ? LicenseRule.Service : LicenseRule.Normal;
                    #endregion

                    #region lấy dữ liệu access form của license type
                    itemInfo.list_access_form = new List<string>();
                    //danh sách access form được mã hóa trên nhiều dòng
                    //mỗi dòng dồng nhiều form khác nhau phân cách nhau bằng dấu ;
                    var accessForm = ReadLicensePath(reader.ReadLine(), reader);
                    //sử dụng vòng lặp để đọc tới khi hết dữ liệu của license type
                    //dấu hiệu kết thúc license type là END
                    while (!accessForm.Equals("END"))
                    {
                        itemInfo.list_access_form.AddRange(accessForm.Split(';'));
                        accessForm = ReadLicensePath(reader.ReadLine(), reader);
                    }
                    lstLicense.Add(itemInfo);
                    #endregion
                    //đọc dữ liệu license type tiếp theo
                    plainInfo = ReadLicensePath(reader.ReadLine(), reader);
                }
            }
        }

        /// <summary>
        /// hàm đọc dữ liệu được mã hóa theo thuật toán xây dựng trước
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static string ReadLicensePath(string encryptText, StringReader reader)
        {
            //lấy số dòng dữ liệu nhiễu
            var length = ((int)Convert.ToChar(encryptText.Substring(15, 1))) % 5;
            //đọc hết thông tin ở các dòng gây nhiễu để đọc dữ liệu tiếp theo
            for (var i = 0; i < length; i++)
            {
                reader.ReadLine();
            }
            //giải mã dữ liệu license
            return Decrypt(encryptText + "==", Constants.RSA.PrivateKey);
        }

        /// <summary>
        /// hàm gen token trong trường hợp login với admin config user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public static string RandomSession(string userName, string dataBase)
        {
            var session = "";
            var arr = string.Format("{0}{1}", userName, dataBase).ToCharArray();
            for (var i = 0; i < arr.Length; i++)
            {
                session += (int)arr[i];
            }
            session += DateTime.Now.Year;
            session += DateTime.Now.Day;
            session += DateTime.Now.Month;
            session += DateTime.Now.Hour;
            session += DateTime.Now.Minute;
            session += DateTime.Now.Second;
            session += DateTime.Now.Millisecond;

            return session;
        }
    }

    public class LicenseInfoImpl
    {
        /// <summary>
        /// license type
        /// </summary>
        public string type_code { get; set; }

        /// <summary>
        /// license type name
        /// </summary>
        public string type_name { get; set; }

        public LicenseRule license_rule { get; set; }

        /// <summary>
        /// trạng thái valid cuar license
        /// false không có license
        /// </summary>
        public bool license_status { get; set; }

        /// <summary>
        /// granted license to user
        /// </summary>
        public bool is_mapping_user { get; set; }

        /// <summary>
        /// Số lượng user của licese
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// số lượng license đã được sử dụng
        /// </summary>
        public int used { get; set; }

        /// <summary>
        /// Thời gian hết hạn license
        /// </summary>
        public DateTime expiry_date { get; set; }

        public List<string> list_access_form { get; set; }
    }

    public enum LicenseRule
    {
        Normal = 0,
        Super = 1,
        Service = 2
    }

    public enum OperatingSystemType
    {
        Linux = 01,
        Windows = 02,
        MacOs = 03
    }
}
