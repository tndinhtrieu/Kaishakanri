using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.ServiceProcess;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Kaishakanri.Commons
{

    /// <summary>
    /// Thực hiện các thao tác chuyên dùng như convert dữ liệu .....
    /// </summary>
    public class CollectionHelper
    {
        static char[] hexDigits = {
         '0', '1', '2', '3', '4', '5', '6', '7',
         '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static void ConvertHashtableToDataRow(Hashtable hashtable, ref DataRow row)
        {
            if (hashtable == null || hashtable.Count == 0)
            {
                row = null;
            }
            else
            {
                foreach (DictionaryEntry objDictionaryEntry in hashtable)
                {
                    row.SetField(objDictionaryEntry.Key.ToString(), objDictionaryEntry.Value);
                }
            }
        }
        public static void ConvertItemToDataRow<T>(T entity, ref DataRow row)
        {
            IList<PropertyInfo> Properties = entity.GetType().GetProperties().ToList();
            if (Properties.Count == 0)
            {
                row = null;
                return;
            }
            foreach (PropertyInfo objPropertyInfo in Properties)
            {
                string Name = objPropertyInfo.Name;
                var Value = objPropertyInfo.GetValue(entity, null);
                row.SetField(Name, Value);
            }
        }
        public static dynamic ConvertDataRowToItem<T>(DataRow row)
        {
            if (row == null)
            {
                return null;
            }
            else
            {
                T entity = Activator.CreateInstance<T>();
                IList<PropertyInfo> Properties = entity.GetType().GetProperties().ToList();
                if (Properties.Count == 0)
                {
                    return null;
                }
                foreach (PropertyInfo objPropertyInfo in Properties)
                {
                    string Name = objPropertyInfo.Name;
                    var Value = row.Field<dynamic>(Name);
                    objPropertyInfo.SetValue(entity, Value, null);
                }
                return entity;
            }
        }
        public static Hashtable ConvertItemToHashTable<T>(T entity)
        {
            IList<PropertyInfo> Properties = entity.GetType().GetProperties().ToList();
            Hashtable hashtable = new Hashtable();
            foreach (PropertyInfo objPropertyInfo in Properties)
            {
                string Name = objPropertyInfo.Name;
                var Value = objPropertyInfo.GetValue(entity, null);
                hashtable.Add(Name, Value);
            }
            return hashtable;
        }
        public static Color StringHexToColor(string Hexa)
        {
            return System.Drawing.ColorTranslator.FromHtml("#" + Hexa);
        }
        public static string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }
        public static Color ContrastColor(Color color)
        {
            try
            {
                byte[] bytes = new byte[3];
                bytes[0] = (byte)(byte.MaxValue - color.R);
                bytes[1] = (byte)(byte.MaxValue - color.G);
                bytes[2] = (byte)(byte.MaxValue - color.B);
                return Color.FromArgb(bytes[0], bytes[1], bytes[2]);
            }
            catch
            {
                Random r = new Random();
                return Color.FromArgb((byte)r.Next(0, 225), (byte)r.Next(0, 225), (byte)r.Next(0, 225));
            }
        }
        #region Control win services
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns>if result is fale then service is runing else services  stoped </returns>
        public static bool StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return true;
            }
            catch (Exception ex)
            {
                Kaishakanri.Commons.LogMan.Instance.WriteErrorToLog(ex);
                return false;
            }
            finally
            {
                if (service != null)
                {
                    service.Close();
                    service.Dispose();
                    service = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch (Exception ex)
            {
                Kaishakanri.Commons.LogMan.Instance.WriteErrorToLog(ex);
            }
            finally
            {
                if (service != null)
                {
                    service.Close();
                    service.Dispose();
                    service = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            StopService(serviceName, timeoutMilliseconds);
            StartService(serviceName, timeoutMilliseconds);

        }
        /// <summary>
        /// Function Check Window Service Installed.
        /// </summary>
        /// <returns></returns>

        public static bool CheckStatusServices(string serviceName, ServiceControllerStatus status)
        {
            ServiceController service = new ServiceController(serviceName);
            if (service == null) return false;
            if (service.Status == status)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInstalledService(string ServicesName)
        {
            ServiceController ctl = ServiceController.GetServices().Where(s => s.ServiceName == ServicesName).FirstOrDefault();
            if (ctl == null)
                return false;
            else
                return true;
        }
        #endregion
        public static void SaveXml(string path, object manager)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(manager.GetType());
                serializer.Serialize(stream, manager);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(stream!=null)
                stream.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic LoadXml(string Path, Type type)
        {
            FileStream stream=null;
            try
            {
                if (File.Exists(Path))
                {
                    dynamic _dynamic = Activator.CreateInstance(type);
                    stream = new FileStream(Path, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(type);
                    _dynamic = serializer.Deserialize(stream);
                    return _dynamic;
                }
                else
                {
                    return null;
                }
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
        /// <summary>
        /// Compare 2 row. first row have to has struct same second row. 
        /// RowTarget and RowSource are not null
        /// </summary>
        /// <param name="rowTarget"></param>
        /// <param name="rowSource"></param>
        /// <returns></returns>
        public static bool CompareRows(DataRow rowTarget, DataRow rowSource)
        {
            List<string> FieldList = new List<string>();
            foreach (DataColumn column in rowTarget.Table.Columns)
                FieldList.Add(column.ColumnName);
            if (FieldList.Count == 0) return false;
            return CompareRows(rowTarget, rowSource, FieldList);
        }
        /// <summary>
        /// rowTarget and RowSource are not null
        /// </summary>
        /// <param name="rowTarget"></param>
        /// <param name="rowSource"></param>
        /// <param name="FieldList">Collection columnName need to compare</param>
        /// <returns></returns>
        public static bool CompareRows(DataRow rowTarget, DataRow rowSource, List<string> FieldList)
        {
            if (FieldList.Count == 0) return false;
            foreach (string ColumnName in FieldList)
            {
                if (!rowTarget.Table.Columns.Contains(ColumnName)) return false;
                if (!rowSource.Table.Columns.Contains(ColumnName)) return false;
                if (rowSource[ColumnName] == null && rowTarget[ColumnName] != null) return false;
                if (rowSource[ColumnName] != null && rowTarget[ColumnName] == null) return false;
                if (!rowSource[ColumnName].Equals(rowTarget[ColumnName]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
