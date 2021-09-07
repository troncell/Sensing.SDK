//-----------------------------------------------------------------------
// <copyright file="DecodeHelper.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>01/11/2012</date>
// <summary>XElement Decode Helper Class is for parsing the XML Element.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using SensingBase.CSException;

namespace SensingBase.Utils
{
    public static class DecodeHelper
    {
        public static List<string> GetItems(XElement config, string elementName)
        {
            List<string> result = new List<string>();
            if (config == null || config.Element(elementName) == null)
            {
                return result;
            }
            foreach (var i in config.Element(elementName).Elements("Item"))
            {
                result.Add(DecodeHelper.GetValueByProperty(i, "Name", string.Empty));
            }
            return result;
        }

        public static Position GetPosition(XElement config)
        {
            return null;
        }

        public static T GetValue<T>(XElement configElement, bool isProperty)
            where T : class
        {
            Type tType = typeof(T);
            T result = Activator.CreateInstance(tType) as T;
            if (configElement == null)
            {
                return result;
            }
            PropertyInfo[] p = tType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo i in p)
            {
                object getValue = i.GetValue(result, null);
                object value = GetValue(i.PropertyType, configElement, i.Name, getValue, isProperty);
                i.SetValue(result, value, null);
            }

            return result;
        }

        private static object GetValue(Type type, XElement configElement, string keyName, object defaultValue, bool isProperty)
        {
            Type typeofClassWithGenericStaticMethod = typeof(DecodeHelper);
            MethodInfo methodInfo = typeofClassWithGenericStaticMethod.GetMethod("GetGenericValue", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(type);
            object returnValue = genericMethodInfo.Invoke(null, new object[] { configElement, keyName, defaultValue, isProperty });
            return returnValue;
        }

        private static T GetGenericValue<T>(XElement configElement, string keyName, T defaultValue, bool isProperty)
        {
            if (configElement == null)
            {
                return defaultValue;
            }
            T result = defaultValue;
            XObject configValue;
            if (!string.IsNullOrEmpty(keyName))
            {
                configValue = isProperty ? configElement.Attribute(keyName) : configElement.Element(keyName) as XObject;
            }
            else
            {
                configValue = configElement as XObject;
            }
            if (configValue != null)
            {
                string keyValue = isProperty ? (configValue as XAttribute).Value : (configValue as XElement).Value.Trim();
                TryParseValue<T>(keyValue, ref result);
            }

            return result;
        }

        public static void TryParseValue<T>(string keyValue, ref T result)
        {
            TryParseValue<T>(string.Empty, keyValue, ref result);
        }

        /// <summary>
        /// 通过反射使用T对象的public属性取出xml对应的属性的值
        /// 默认值为该对象属性的Get方法返回的值
        /// </summary>
        /// <typeparam name="T">
        ///     Should  inherite class
        /// </typeparam>
        /// <param name="configElement"></param>
        /// <returns></returns>
        public static T GetValueByProperty<T>(XElement configElement)
            where T : class
        {
            return GetValue<T>(configElement, true);
        }

        /// <summary>
        /// 通过反射使用T对象的public属性取出xml对应的元素的值
        /// 默认值为该对象属性的Get方法返回的值
        /// </summary>
        /// <typeparam name="T">
        ///     Should  inherite class
        /// </typeparam>
        /// <param name="configElement"></param>
        /// <returns></returns>
        public static T GetValueByElement<T>(XElement configElement,T defaultValue)
            where T : class
        {
            //return GetValue<T>(configElement, false);
            return GetGenericValue<T>(configElement,null,defaultValue, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">double,int,long,datetime,string,bool(True,false / 1,0),timespan</typeparam>
        /// <param name="configElement"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueByProperty<T>(XElement configElement, string propertyName, T defaultValue)
        {
            return GetGenericValue<T>(configElement, propertyName, defaultValue, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">keyName is used for updating the result T if this param is in{top,height,left,width}</param>
        /// <param name="keyValue"></param>
        /// <param name="result"></param>
        public static void TryParseValue<T>(string keyName, string keyValue, ref T result)
        {
            Type tType = typeof(T);
            if (tType.Equals(typeof(double)))
            {
                double value;
                if (double.TryParse(keyValue, out value))
                {
                    result = (T)(object)value;
                }
                //处理 Left,Top,Width,Height
                //result = (T)FilterPosition(keyName, result);
            }
            else if (tType.Equals(typeof(int)))
            {
                int value;
                if (int.TryParse(keyValue, out value))
                {
                    result = (T)(object)value;
                }
            }
            else if (tType.Equals(typeof(bool)))
            {
                bool value;
                if (bool.TryParse(keyValue, out value))
                {
                    result = (T)(object)value;
                }
                else if (keyValue == "1" || keyValue == "0")
                {
                    result = (T)(object)(keyValue == "1");
                }
            }
            else if (tType.Equals(typeof(long)))
            {
                long value;
                if (long.TryParse(keyValue, out value))
                {
                    result = (T)(object)value;
                }
            }
            else if (tType.Equals(typeof(string)))
            {
                result = (T)(Object)keyValue;
            }
            else if (tType.Equals(typeof(DateTime)))
            {
                DateTime value;
                if (DateTime.TryParse(keyValue, out value))
                {
                    result = (T)(Object)value;
                }
            }
            else if (tType.Equals(typeof(Guid)))
            {
                Guid value;
                try
                {
                    value = new Guid(keyValue);
                    result = (T)(object)value;
                }
                catch (Exception ex)
                {
                    throw new CoreException("Cann't create the GUID",ex);
                }
            }
            else if (tType.Equals(typeof(TimeSpan)))
            {
                TimeSpan value;


                //value = TimeSpan.ParseExact(keyValue, "HH:mm:ss", null);
                if (TimeSpan.TryParse(keyValue, out value))
                {
                    result = (T)(object)value;
                }
                else
                {
                    throw new CoreException("Can not Trypars timespan");
                }


            }


        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">double,int,long,datetime,string,bool(True,false / 1,0)
        /// timespan
        /// </typeparam>
        /// <param name="configElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueByElement<T>(XElement configElement, string elementName, T defaultValue)
        {
            return GetGenericValue<T>(configElement, elementName, defaultValue, false);
        }


        /// <summary>
        /// e.g
        /// <Args Book="Spiration">
        ///     <Arg Name="Description" Value="Ambition"></Arg>
        /// </Args>
        /// </summary>
        /// <param name="elementXml">the element contains attributes and sub elements.</param>
        /// <param name="elementName">the element's name</param>
        /// <param name="childElementName">the child element's name</param>
        /// <returns></returns>
        public static void AddKeyValueFromAttributes(XElement elementXml,string childElementName,Dictionary<string,string> addedDictionary)
        {
            XElement varsElement = elementXml;
            if (varsElement != null)
            {
                foreach (XAttribute attribute in varsElement.Attributes())
                {
                    addedDictionary.Add(attribute.Name.ToString(), attribute.Value);
                }
                if (!string.IsNullOrEmpty(childElementName))
                {
                    foreach (var arg in varsElement.Elements(childElementName))
                    {
                        string name = GetValueByProperty<string>(arg, "Name", null);
                        string value = GetValueByProperty<string>(arg, "Value", null);
                        if (!string.IsNullOrEmpty(name))
                        {
                            addedDictionary.Add(name, value);
                        }
                    }
                }
            }
        }
    }
}