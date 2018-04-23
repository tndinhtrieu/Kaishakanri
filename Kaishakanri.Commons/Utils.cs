using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Diagnostics;
using System.Net;
using System.Xml.Serialization;
using System.Management;

namespace Kaishakanri.Commons
{
    public class Utils
    {
        /// <summary>
        /// Read value from app.config
        /// </summary>
        /// <param name="pstrKey"></param>
        /// <param name="pstrType"></param>
        /// <returns></returns>
        public static object GetAppConfig(string pstrKey, string pstrType)
        {
            try
            {
                var objAppReader = new AppSettingsReader();
                return objAppReader.GetValue(pstrKey, Type.GetType(pstrType));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Function Open Help File
        /// </summary>
        /// <param name="sUrl"></param>
        public static void OpenLink(string sUrl)
        {
            try
            {
                System.Diagnostics.Process.Start(sUrl);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                        System.Diagnostics.Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                        LogMan.Instance.WriteToLog(exc2.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Read string value from app.config
        /// </summary>
        /// <param name="pstrKey"></param>
        /// <returns></returns>
        public static string GetAppConfig(string pstrKey)
        {
            try
            {
                var objAppReader = new AppSettingsReader();
                return objAppReader.GetValue(pstrKey, typeof(string)).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Get string from resource belong Culture
        /// </summary>
        /// <param name="iText"></param>
        /// <param name="currentLanguage"></param>
        /// <returns></returns>
        public static string GetRealString(string iText, string currentLanguage)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(currentLanguage);
            var rm = new ResourceManager(GetAppConfig("ResourceName"), Assembly.GetCallingAssembly());
            return rm.GetString(iText, culture);
        }

        public static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return Convert.ToBase64String(Results);
        }

        public static string DecryptString(string Message, string Passphrase)
        {
            if (string.IsNullOrEmpty(Message)) return string.Empty;
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();


            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return UTF8.GetString(Results);
        }

        public static int GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            int i = int.MaxValue;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    i = 0;
                    break;
                case DayOfWeek.Tuesday:
                    i = 1;
                    break;
                case DayOfWeek.Wednesday:
                    i = 2;
                    break;
                case DayOfWeek.Thursday:
                    i = 3;
                    break;
                case DayOfWeek.Friday:
                    i = 4;
                    break;
                case DayOfWeek.Saturday:
                    i = 5;
                    break;
                case DayOfWeek.Sunday:
                    i = 6;
                    break;
                default:
                    i = int.MaxValue;
                    break;
            }
            return i;
        }

        public static int WeekNumber(DateTime value)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFourDayWeek,
                                                                     DayOfWeek.Monday);
        }

        public static bool CompareWeek(DateTime param1, DateTime param2)
        {
            if (WeekNumber(param1) == WeekNumber(param2))
                return true;
            return false;
        }

        public static bool IsInstalledService(string ServicesName)
        {
            ServiceController ctl=null;
            try
            {
                ctl = ServiceController.GetServices().Where(s => s.ServiceName == ServicesName).FirstOrDefault();
                if (ctl == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ctl != null)
                {
                    ctl.Dispose();
                    ctl = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                
            }
        }
        public static void InstallService(string ExeFilename)
        {
            string[] commandLineOptions = new string[1] { "/LogFile=install.log" };
            AssemblyInstaller Installer = new AssemblyInstaller(ExeFilename, commandLineOptions);
            try
            {
                Installer.UseNewContext = true;
                Installer.Install(null);
                Installer.Commit(null);
            }
            catch (Exception ex)
            {
                Kaishakanri.Commons.LogMan.Instance.WriteErrorToLog(ex);
            }
            finally
            {
                Installer.Dispose();
            }
        }

        public static void UninstallService(string ExeFilename)
        {
            string[] commandLineOptions = new string[1] { "/LogFile=uninstall.log" };
            AssemblyInstaller Installer = new AssemblyInstaller(ExeFilename, commandLineOptions);
            try
            {
                Installer.UseNewContext = true;
                Installer.Uninstall(null);
            }
            catch (Exception ex)
            {
                Kaishakanri.Commons.LogMan.Instance.WriteErrorToLog(ex);
            }
            finally
            {
                Installer.Dispose();
            }
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string convertToUnSign2(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            sb = sb.Replace("  ", " ");
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        public unsafe static string converToUnsign1(string s)
        {
            string[] pattern = {"(á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ)",
                   "đ",
                   "(é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ)",
                   "(í|ì|ỉ|ĩ|ị)",
                   "(ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ)",
                   "(ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự)",
                   "(ý|ỳ|ỷ|ỹ|ỵ)"};
            char[] replaceChar = { 'a', 'd', 'e', 'i', 'o', 'u','y',
    'A', 'D', 'E', 'I', 'O', 'U', 'Y'};

            fixed (char* ptrChar = replaceChar)
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    MatchCollection matchs = Regex.Matches(s, pattern[i], RegexOptions.IgnoreCase);
                    foreach (Match m in matchs)
                    {
                        char ch = char.IsLower(m.Value[0]) ? *(ptrChar + i) : *(ptrChar + i + 7);
                        s = s.Replace(m.Value[0], ch);
                    }
                }
            }
            return s;
        }

        public static List<string> GetMAC()
        {
            List<string> ListMACAddress = new List<string>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
              string   checkMAC = nic.GetPhysicalAddress().ToString();
              if (!string.IsNullOrEmpty(checkMAC))
              {
                  if (!ListMACAddress.Contains(checkMAC))
                  {
                      ListMACAddress.Add(checkMAC);
                  }
              }
            }
            return ListMACAddress;

        }
        public static string GetPasswordRandom()
        {
           return Guid.NewGuid().ToString("d").Substring(1, 8);
        }

        public static T GetObject<T>(object Source)
        {
            T objTarget = Activator.CreateInstance<T>();
            PropertyInfo[] Properties = objTarget.GetType().GetProperties();
            foreach (PropertyInfo propertiy in Properties)
            {
                try
                {
                    PropertyInfo propertySource = Source.GetType().GetProperty(propertiy.Name);
                    if (propertySource != null)
                    {
                        object value = propertySource.GetValue(Source, null);
                        propertiy.SetValue(objTarget, value, null);
                    }
                }
                catch
                {
                }
            }
            return objTarget;
        }

        public static void email_send(string FromAddress, string FromPass, string ToAddresss, dynamic Info,string Replace)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(FromAddress);
            mail.To.Add(ToAddresss);
            mail.Subject = "Thông tin tài khoản đăng nhập hệ thống quản lý file trực tuyến";
            mail.Body = "Tên đăng nhập: " + Info.UserName.Replace(Replace, string.Empty) + "\r\n" +
                        "Mật khẩu:  "+ Utils.DecryptString(Info.Password,"tndinhtrieu");

            //System.Net.Mail.Attachment attachment;
            //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            //mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(FromAddress,FromPass);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

        }

        public static void ProcessActive(string FullFileName)
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = FullFileName;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
        }

        private static Stream GetServerVersion(string url)
        {
            WebClient request = new WebClient();
            request.Credentials = new NetworkCredential("uservbcv", "vbcvpass");
            try
            {
                byte[] newFileData = request.DownloadData(new Uri(url));
                Stream stream = new MemoryStream(newFileData);
                return stream;
            }
            catch (WebException e)
            {
            }
            return null;
        }
        public static dynamic LoadXmlByUrl(string url, Type type)
        {
            Stream stream = null;
            try
            {
                stream = GetServerVersion(url);
                //XmlTextReader xmltextreader = new XmlTextReader(url);
                dynamic _dynamic = Activator.CreateInstance(type);
                XmlSerializer serializer = new XmlSerializer(type);
                _dynamic = serializer.Deserialize(stream);
                return _dynamic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

        }

        public static bool CheckUpdate(string CurrentVersion, string NewVersion)
        {
            try
            {
                string[] arrCur = CurrentVersion.Split('.');
                string[] arrNew = NewVersion.Split('.');
                int Length = arrCur.Length < arrNew.Length ? arrCur.Length : arrNew.Length;
                for (int i = 0; i < Length; i++)
                {
                    if (int.Parse(arrCur[i]) < int.Parse(arrNew[i]))
                    {
                        return true;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
            }
            return false;
        }
    }

    public class Parser
    {
        public const string FormatString = "{0:#,0.##}";
        public const string ExactString = "{0:#,0.00}";
        public const string Dash = "\u2212";

        public static string ToString(object o)
        {
            string s = null;
            if (o == null)
            {
                s = "";
            }
            else
            {
                s = o.ToString();
            }

            return s;
        }

        public static int ToInt(string s)
        {
            int val;
            int.TryParse(s, out val);
            return val;
        }
        public static int ToInt(object s)
        {
            int val;
            int.TryParse(ToString(s), out val);

            return val;
        }
        public static long ToLong(string s)
        {
            long val;
            long.TryParse(s, out val);

            return val;
        }

        public static DateTime ToDateTime(string s)
        {
            DateTime val;
            DateTime.TryParse(s, out val);

            return val;
        }
        public static float ToFloat(string s)
        {
            float val;
            float.TryParse(s, out val);

            return val;
        }
        public static double ToDouble(string s)
        {
            double val;
            double.TryParse(s, out val);

            return val;
        }
        public static double ToDoubleFromCurrency(string currency)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return 0;
            }
            else
            {
                for (int i = currency.Length - 1; i >= 0; i--)
                {
                    if ((currency[i] < '0' || currency[i] > '9') && currency[i] != '.')
                    {
                        currency = currency.Remove(i, 1);
                    }
                }
                double val;
                double.TryParse(currency, out val);
                return val;
            }
        }
        public static float ToFloatFromCurrency(string currency)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return 0;
            }
            else
            {
                for (int i = currency.Length - 1; i >= 0; i--)
                {
                    if ((currency[i] < '0' || currency[i] > '9') && currency[i] != '.' && currency[i] != '-')
                    {
                        currency = currency.Remove(i, 1);
                    }
                }
                float val;
                float.TryParse(currency, out val);
                return val;
            }
        }

        public static bool ToBool(string _Value)
        {
            if (_Value == "True" || _Value == "true")
                return true;
            return false;
        }

        public static string ToFormatNumber(double number)
        {
            if (Math.Round(number, 2) == 0)
            {
                return "0";
            }
            else
            {

                string str = string.Empty;
                if (number < 0)
                {
                    str = String.Format(FormatString, -number);
                    str = Dash + str;
                }
                else
                {
                    str = String.Format(FormatString, number);
                }
                return str;
            }
        }

        public static string ToExactNumber(double number)
        {
            if (Math.Round(number, 2) == 0)
            {
                return "0";
            }
            else
            {

                string str = string.Empty;
                if (number < 0)
                {
                    str = String.Format(ExactString, -number);
                    str = Dash + str;
                }
                else
                {
                    str = String.Format(ExactString, number);
                }
                return str;
            }
        }

        public static DateTime ConvertToJPDateFromString(string idate)
        {
            try
            {
                DateTimeFormatInfo dtif = new DateTimeFormatInfo();
                dtif.ShortDatePattern = "yyyyMMdd";
                dtif.DateSeparator = "";
                DateTime obj;
                DateTime.TryParse(idate, dtif, DateTimeStyles.None, out obj);
                return obj;
            }
            catch (Exception ex)
            {
                LogMan.Instance.WriteToLog(ex.ToString());
                return DateTime.MinValue;
            }
        }

        public static int GetYear(int dDate)
        {
            return dDate / 10000;
        }

        public static int GetMonth(int dDate)
        {
            int dMonth = dDate - GetYear(dDate) * 10000;
            return dMonth / 100;
        }

        public static int GetDay(int dDate)
        {
            int dMonth = dDate - GetYear(dDate) * 10000;
            return dMonth % 100;
        }

        public static int GetHour(int dTime)
        {
            return dTime / 10000;
        }

        public static int GetMinute(int dTime)
        {
            int dMinute = dTime - GetHour(dTime) * 10000;
            return dMinute % 100;
        }

        public static int GetRHour(int dTime)
        {
            return dTime / 100;
        }

        public static int GetRMinute(int dTime)
        {
            int dMinute = dTime - GetRHour(dTime) * 100;
            return dMinute % 100;
        }

        public static string StrGetRHour(int dTime)
        {
            return (dTime / 100).ToString();
        }

        public static string StrGetRMinute(int dTime)
        {
            string strminute = (dTime % 100).ToString();
            if (strminute.Length == 1)
            {
                strminute = "0" + strminute;
            }
            return strminute;
        }

       
    }

    public class XMLProcess
    {
        #region Singlton
        /// <summary>
        /// Get the Instance of XMLProcess class
        /// </summary>
        public static readonly XMLProcess Instance = new XMLProcess();
        #endregion

        #region Private Varial

        /// <summary>
        /// Varial store file name of XML file.
        /// </summary>
        private string _FileName = string.Empty;
        /// <summary>
        /// Varial store root node name of XML file.
        /// </summary>
        private string _RootName = string.Empty;
        /// <summary>
        /// Varial store element name of XML node.
        /// </summary>
        private string _ElementName = string.Empty;
        /// <summary>
        /// Varial store attribute of XML node.
        /// </summary>
        private string _AttributeName = string.Empty;
        /// <summary>
        /// Varial store list name of attribute in a node.
        /// </summary>
        private List<string> _NodeFieldName = new List<string>();
        /// <summary>
        /// Varial store list value of note or attribute.
        /// </summary>
        private Dictionary<string, string> _NodeAttritbuteArray = new Dictionary<string, string>();
        /// <summary>
        /// Varial store path to XML folder.
        /// </summary>
        private string _Path = Application.StartupPath + @"\App_Data\";
        /// <summary>
        /// Constain define XML type.
        /// </summary>
        private const string _Extention = ".xml";

        #endregion Private Varial

        #region Private Function

        /// <summary>
        /// Function create new XML file. 
        /// </summary>
        /// <param name="Name">File name of XML file.</param>
        /// <param name="RootName">Root name of XML file.</param>
        /// <returns>True if create successfull, false.</returns>
        private bool _CreateXMLFile(string Name, string RootName)
        {
            string PathToFile = _Path + Name + _Extention;
            if (!File.Exists(PathToFile))
            {
                Directory.CreateDirectory(_Path);
                XDocument _xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
                XElement _rootItem = new XElement(RootName, string.Empty);
                _xml.Add(_rootItem);
                StreamWriter sw = new StreamWriter(PathToFile, false, Encoding.UTF8, 512);
                _xml.Save(sw);
                sw.Close();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function insert a new node into XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file.</param>
        /// <param name="NodeName">Node name to add into XML file.</param>
        /// <param name="NodeAttritbuteArray">Attribute array  of new node.</param>
        /// <returns>True if add successfull, other return false.</returns>
        private bool _AddNewNode(string FileName, string NodeName, Dictionary<string, string> NodeAttritbuteArray)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xNode = xDoc.Elements().FirstOrDefault();
                XElement NewNode = new XElement(NodeName);
                foreach (KeyValuePair<string, string> kvp in NodeAttritbuteArray)
                {
                    XAttribute _Att = new XAttribute(kvp.Key, kvp.Value);
                    NewNode.Add(_Att);
                }
                xNode.Add(NewNode);
                xRead.Close();
                xDoc.Save(PathToFile);
                return true;
            }
            return false;
        }

        private bool _AddNewNode(string FileName, string NodeName, Dictionary<string, string> NodeAttritbuteArray, string KeyAttribute)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xNode = xDoc.Elements().FirstOrDefault();
                XElement NewNode = new XElement(NodeName);
                foreach (KeyValuePair<string, string> kvp in NodeAttritbuteArray)
                {
                    XAttribute _Att = new XAttribute(kvp.Key, kvp.Value);
                    NewNode.Add(_Att);
                }
                xNode.Add(NewNode);
                xRead.Close();
                xDoc.Save(PathToFile);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check new element is exits ?
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="RootName"></param>
        /// <param name="iNode"></param>
        /// <returns></returns>
        private bool CheckExits(string FileName, string RootName, XElement iNode)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                var q = from c in xDoc.Elements(RootName).Elements(iNode.Name.ToString())
                        where c == iNode
                        select c;
                xRead.Close();
                if (q.Count() > 0)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Function to modified a node in XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file.</param>
        /// <param name="NodeName">Node name need modified.</param>
        /// <param name="AttributeName">Attribute name need modified in node.</param>
        /// <param name="ConditionValue">Condition to decied modified node.</param>
        /// <param name="ModifiedValue">Modified value to save into this attribute.</param>
        /// <returns>True if modified successful, other return false.</returns>
        private bool _ModifiedNode(string FileName, string NodeName, string AttributeName, string ConditionValue, string ModifiedValue)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xRootName = xDoc.Elements().FirstOrDefault();
                XElement xNode = xDoc.Descendants(NodeName).Where(c => c.Attribute(AttributeName).Value.Equals(ConditionValue)).FirstOrDefault();
                if (xNode != null)
                {
                    xNode.SetAttributeValue(AttributeName, ModifiedValue);
                    xRead.Close();
                    xDoc.Save(PathToFile);
                    xNode = null;
                    xRootName = null;
                    xDoc = null;
                    //xDoc.Save(PathToFile);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function to remove a node from XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file</param>
        /// <param name="NodeName">Node name need remove</param>
        /// <param name="AttributeKey">Attribute key to decied node.</param>
        /// <param name="ConditionValue">Condition value to search node</param>
        /// <returns>True if remove successfull, else return false</returns>
        private bool _RemoveNode(string FileName, string NodeName, string AttributeKey, string ConditionValue)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xNode = xDoc.Descendants(NodeName).Where(c => c.Attribute(AttributeKey).Value.Equals(ConditionValue)).FirstOrDefault();
                if (xNode != null)
                {
                    xRead.Close();
                    xNode.Remove();
                    xDoc.Save(PathToFile);
                }
                return true;
            }
            return false;
        }

        #endregion Private Function

        #region Public Function
        /// <summary>
        /// Function create new XML file. 
        /// </summary>
        /// <param name="Name">File name of XML file.</param>
        /// <param name="NodeRootName">Root name of XML file.</param>
        /// <returns>True if create successfull, false.</returns>
        public void CreateXMLFile(string FileName, string NodeRootName)
        {
            _CreateXMLFile(FileName, NodeRootName);
        }
        /// <summary>
        /// Function insert a new node into XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file.</param>
        /// <param name="NodeName">Node name to add into XML file.</param>
        /// <param name="NodeAttritbuteArray">Attribute array  of new node.</param>
        /// <returns>True if add successfull, other return false.</returns>
        public bool AddNewNode(string FileName, string NodeName, Dictionary<string, string> NodeAttritbuteArray)
        {
            return _AddNewNode(FileName, NodeName, NodeAttritbuteArray);
        }
        /// <summary>
        /// Function to modified a node in XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file.</param>
        /// <param name="NodeName">Node name need modified.</param>
        /// <param name="AttributeName">Attribute name need modified in node.</param>
        /// <param name="ConditionValue">Condition to decied modified node.</param>
        /// <param name="ModifiedValue">Modified value to save into this attribute.</param>
        /// <returns>True if modified successful, other return false.</returns>
        public bool ModifiedNode(string FileName, string NodeName, string AttributeName, string ConditionValue, string ModifiedValue)
        {
            return _ModifiedNode(FileName, NodeName, AttributeName, ConditionValue, ModifiedValue);
        }
        /// <summary>
        /// Function to remove a node from XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file</param>
        /// <param name="NodeName">Node name need remove</param>
        /// <param name="AttributeKey">Attribute key to decied node.</param>
        /// <param name="ConditionValue">Condition value to search node</param>
        /// <returns>True if remove successfull, else return false</returns>
        public bool RemoveNode(string FileName, string NodeName, string AttributeKey, string ConditionValue)
        {
            return _RemoveNode(FileName, NodeName, AttributeKey, ConditionValue);
        }
        /// <summary>
        /// Return content of last node in XML file.
        /// </summary>
        /// <param name="FileName">File name of XML file.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetLastNode(string FileName)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xRootName = xDoc.Elements().FirstOrDefault();
                if (xRootName != null)
                {
                    XElement xNode = xRootName.Elements().LastOrDefault();
                    if (xNode != null)
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        foreach (XAttribute aName in xNode.Attributes())
                        {
                            result.Add(aName.Name.LocalName, aName.Value);
                        }
                        xRead.Close();
                        return result;
                    }
                }
                xRead.Close();
            }
            return null;
        }
        /// <summary>
        /// Funtion get key value of a element.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="NodeName"></param>
        /// <param name="AttributeKeyName"></param>
        /// <param name="ConditionName"></param>
        /// <param name="ConditionValue"></param>
        /// <returns></returns>
        public string GetAttributeValue(string FileName, string NodeName, string AttributeKeyName, string ConditionName, string ConditionValue)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xRootName = xDoc.Elements().FirstOrDefault();
                if (xRootName != null)
                {
                    var q = from c in xDoc.Elements(xRootName.Name.ToString()).Elements(NodeName)
                            where c.Attribute((XName)ConditionName).Value.Equals(ConditionValue)
                            select c;
                    if (q.Count() > 0)
                    {
                        xRead.Close();
                        return q.FirstOrDefault().Attribute((XName)AttributeKeyName).Value;
                    }
                }
                xRead.Close();
            }
            return string.Empty;
        }

        public XElement GetLastElement(string FileName, string NodeName, string ConditionName, string ConditionValue)
        {
            string PathToFile = _Path + FileName + _Extention;
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xRootName = xDoc.Elements().FirstOrDefault();
                if (xRootName != null)
                {
                    var q = from c in xDoc.Elements(xRootName.Name.ToString()).Elements(NodeName)
                            where c.Attribute((XName)ConditionName).Value.Equals(ConditionValue)
                            select c;
                    if (q.Count() > 0)
                    {
                        xRead.Close();
                        return q.LastOrDefault();
                    }
                }
                xRead.Close();
            }
            return null;
        }
        /// <summary>
        /// Function to read all data from XML file to DataTable
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public DataTable GetAllADataFromFile(string FileName, string NodeName, string KeyAttribute, int SCode)
        {
            string PathToFile = _Path + FileName + _Extention;
            DataTable dtReturn = new DataTable();
            if (File.Exists(PathToFile))
            {
                XmlReader xRead = XmlReader.Create(PathToFile);
                XDocument xDoc = XDocument.Load(xRead);
                XElement xRootName = xDoc.Elements().FirstOrDefault();
                if (xRootName != null)
                {
                    XElement NodeItem = xDoc.Elements(xRootName.Name.ToString()).Elements(NodeName).FirstOrDefault();
                    foreach (var XAt in NodeItem.Attributes())
                    {
                        dtReturn.Columns.Add(XAt.Name.ToString(), typeof(string));
                    }
                    var q = (from c in xDoc.Elements(xRootName.Name.ToString()).Elements(NodeName)
                             where c.Attribute(KeyAttribute).Value.Equals(SCode.ToString())
                             select c).Distinct();
                    if (q != null)
                    {
                        foreach (var item in q)
                        {
                            DataRow dr = dtReturn.NewRow();
                            foreach (var xAt in item.Attributes())
                            {
                                dr[xAt.Name.ToString()] = xAt.Value;
                            }
                            dtReturn.Rows.Add(dr);
                        }
                    }
                }
                xRead.Close();
            }
            return dtReturn;
        }

        #endregion End Public Function
    }

    public enum ModeFormAction
    {
        Insert,
        Update,
        Delete,
        Load
    }
}
