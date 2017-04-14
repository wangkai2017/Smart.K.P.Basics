using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SmartCommon.EnumHelper
{
    public class EnumItem
    {
        public string ItemText { get; set; }
        public string ItemValue { get; set; }
        public string ItemDes { get; set; }
        public const string ItemValueField = "ItemValue";
        public const string ItemTextField = "ItemText";
        public const string ItemDesField = "ItemDes";
    }
    public static class EnumExt
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, EnumItem>> EnumAbout
            = new ConcurrentDictionary<Type, Dictionary<string, EnumItem>>();
        /// <summary>得到备注</summary>
        public static string GetDes(this Enum value)
        {
            try
            {
                if (value == null) return "";
                FieldInfo field = value.GetType().GetField(value.ToString());
                return ((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))).Description;
            }
            catch (Exception ex)
            {
                return value.ToString();
            }
        }
        /// <summary>得到备注</summary>
        public static string GetDes(this Enum value, string eunmValue)
        {
            Dictionary<string, EnumItem> eItem = value.GetItemList();
            string desStr = "";
            foreach (var item in eunmValue.Split(','))
            {
                desStr += "," + (eItem.ContainsKey(item) ? eItem[item].ItemDes : "");
            }
            return desStr.TrimStart(',');
        }
        public static string GetDesNew(this Enum value, string eunmValue)
        {
            string desStr = "";
            try
            {
                Dictionary<string, EnumItem> eItem = value.GetItemList();
                foreach (var item in eunmValue.Split(','))
                {
                    desStr += "," + (eItem.ContainsKey(item) ? eItem[item].ItemDes : "");
                }
            }
            catch (Exception ex)
            {
                return "获取描述错误";
            }
            return desStr.TrimStart(',');
        }
        /// <summary>根据枚举常量得到备注</summary>
        public static string GetDesByTextField(this Enum value, string enumText)
        {
            if (value == null) return "";
            object enumValue = Enum.Parse(value.GetType(), enumText);
            FieldInfo field = value.GetType().GetField(enumValue.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))).Description;
        }
        public static Dictionary<string, EnumItem> GetItemList(this Enum value)
        {
            Type eType = value.GetType();
            if (!EnumAbout.ContainsKey(eType))
            {
                Type valueType = Enum.GetUnderlyingType(eType);
                var enums = Enum.GetValues(eType);
                Dictionary<string, EnumItem> tmpList = new Dictionary<string, EnumItem>();
                foreach (Enum e in enums)
                    tmpList.Add(Convert.ChangeType(e, valueType).ToString(), new EnumItem
                    {
                        ItemText = e.ToString(),
                        ItemValue = Convert.ChangeType(e, valueType).ToString(),
                        ItemDes = e.GetDes()
                    });
                EnumAbout.TryAdd(eType, tmpList);
            }
            return EnumAbout[eType];
        }
        public static SelectList GetSelectList(this Enum value, string emptyChoose = null, object selectedValue = null)
        {
            List<EnumItem> tempValue = value.GetItemList().Values.Where(z => !z.ItemDes.StartsWith("_")).ToList();
            if (emptyChoose != null)
            {
                string[] emptys = emptyChoose.Split(',');
                tempValue.Insert(0, new EnumItem { ItemText = "Choose", ItemValue = emptys[0], ItemDes = emptys.Length > 1 ? emptys[1] : "==请选择==" });
            }
            SelectList list = new SelectList(tempValue, EnumItem.ItemValueField, EnumItem.ItemDesField, selectedValue);
            return list;
        }
        
        #region 根据备注信息获取枚举值
        /// <summary>
        /// 根据备注信息获取枚举值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Enum GetEnumByDesc(System.Type enumType, string enumValue)
        {
            Enum result = null;
            foreach (object enumObject in Enum.GetValues(enumType))
            {
                Enum e = (Enum)enumObject;

                if (GetDes(e) == enumValue)
                {
                    result = e;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region 根据枚举值得到枚举
        /// <summary>
        /// 根据枚举值得到枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strEnum"></param>
        /// <returns></returns>
        public static T ToEnum<T>(int strEnum)
        {
            return (T)Enum.Parse(typeof(T), strEnum.ToString());
        }

        public static T ToEnum<T>(string strEnum)
        {
            return (T)Enum.Parse(typeof(T), strEnum);
        }
        #endregion

        #region 根据枚举和备注信息得到枚举的Value值
        /// <summary>
        /// 根据枚举和备注信息得到枚举的Value值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static int GetEnumValueByDesc(System.Type enumType, string description)
        {
            DataTable dt = GetEnumTable(enumType);
            int txtenumValue = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Text"].ToString() == description)
                {
                    txtenumValue = Convert.ToInt32(dt.Rows[i]["Value"].ToString());
                }
            }
            return txtenumValue;
        }
        #endregion

        #region 将含有描述信息的枚举组装datatable
        /// <summary>
        /// 将含有描述信息的枚举组装datatable(Table两个字段，text是描述，value是值(默认是int的值))
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable GetEnumTable(System.Type enumType)
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add("Text");
            dt.Columns.Add("Value");
            foreach (object enumValue in Enum.GetValues(enumType))
            {
                dr = dt.NewRow();
                Enum e = (Enum)enumValue;

                dr["Text"] = GetDes(e);
                dr["Value"] = Convert.ToInt32(enumValue).ToString();
                dt.Rows.Add(dr);

            }
            return dt;
        }
        #endregion
        
        #region 获取枚举的值字符串信息，根据传入的枚举值
        /// <summary>
        /// 获取枚举的值字符串信息，根据传入的枚举值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int GetEnumInt(Enum e)
        {
            return Convert.ToInt32(e);

        }
        #endregion

        #region 将指定枚举类型转换成List
        /// <summary>
        /// 将指定枚举类型转换成List，用来绑定ListControl
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="selectFeild">默认选择的项的值</param>
        /// <returns></returns>
        public static List<SelectListItem> ConvertEnumToList(Type enumType, string selectFeild)
        {
            if (enumType.IsEnum == false) { return null; }
            var typeDescription = typeof(DescriptionAttribute);
            var fields = enumType.GetFields();
            var selectListItems = new List<SelectListItem>();
            foreach (var field in fields.Where(field => !field.IsSpecialName))
            {
                var strValue = field.GetRawConstantValue().ToString();
                var arr = field.GetCustomAttributes(typeDescription, true);
                var strText = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : field.Name;

                var item = new SelectListItem { Text = strText, Value = strValue };

                if (strValue.Equals(selectFeild))
                    item.Selected = true;
                selectListItems.Add(item);
            }
            return selectListItems;
        }

        #endregion

        /// <summary>
        /// 根据值获取枚举描述
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDescByValue(System.Type enumType, int enumValue)
        {
            string dec = "";
            foreach (object enumObject in Enum.GetValues(enumType))
            {
                Enum e = (Enum)enumObject;
                int value = Convert.ToInt32(enumObject);
                if (value == enumValue)
                {
                    dec = GetDes(e);
                }
            }
            return dec;
        }
    }
}
