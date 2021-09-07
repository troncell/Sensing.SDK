//-----------------------------------------------------------------------
// <copyright file="XElementDecoder.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-15</date>
// <summary>XElementDecoder is an adapter class for XElement.</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using LogService;

namespace SensingBase.Utils
{
    public class XElementDecoder
    {
        private static readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(XElementDecoder));

        private static XElementDecoder EmptyXElementDecoder = new XElementDecoder();
        private static IEnumerable<XElementDecoder> EmptyList = new List<XElementDecoder>();
        private XElement elemItem;

        public XElement ElemItem
        {
            get { return elemItem; }
        }

        private XElementDecoder()
        {

        }
        
        /// <summary>
        /// 通过 XElement构造
        /// 参数可以为
        /// </summary>
        /// <param name="xelem"></param>
        public XElementDecoder(XElement xelem)
        {
            elemItem = xelem;
        }

        /// <summary>
        /// 转换XmlElement 为 XElementDecoder
        /// </summary>
        /// <param name="xelem"></param>
        /// <returns></returns>
        public static XElementDecoder Parse(XmlElement xelem)
        {
            XElementDecoder result = new XElementDecoder();
            if (xelem == null)
            {
                result.elemItem = null;
            }
            else
            {
                result.elemItem = XElement.Parse(xelem.OuterXml);
            }

            return result;
        }
        
        public static bool IsNullorEmptyXElementDecoder(XElementDecoder xDecoder)
        {
            return (xDecoder == null || xDecoder == EmptyXElementDecoder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem">可以为 NULL</param>
        /// <returns></returns>
        public XElementDecoder Element(string elem)
        {
            XElementDecoder result;
            if (elemItem == null || elem == null)
            {
                logger.Debug("Element elemItem or elemPara is NULL ---- elemPara=" + elem);
                result = EmptyXElementDecoder;
            }
            else
            {
                try
                {
                    XElement xelem = elemItem.Element(elem);
                    if (xelem != null)
                    {
                        result = new XElementDecoder(xelem);
                    }
                    else
                    {
                        logger.Debug("this Elem will be empty with " + elem);
                        result = EmptyXElementDecoder;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Element Error in XElementDecoder", ex);
                    result = EmptyXElementDecoder;
                }
            }
            return result;
        }

        public IEnumerable<XElementDecoder> Elements(string elem)
        {
            if (elemItem == null)
            {
                logger.Debug("Elements Method SelfElement is NULL ---- " + elem);
                yield break;
            }

            foreach (var i in elemItem.Elements(elem))
            {
                yield return new XElementDecoder(i);
            }
        }

        /// <summary>
        /// 通过反射使用T对象的public属性取出xml对应的元素的值
        /// 默认值为该对象属性的Get方法返回的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValueByElement<T>(T defaultValue)
            where T : class
        {
            return DecodeHelper.GetValueByElement<T>(elemItem, defaultValue);
        }

        /// <summary>
        /// 通过反射使用T对象的public属性取出xml对应的属性的值
        /// 默认值为该对象属性的Get方法返回的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValueByProperty<T>()
            where T : class
        {
            return DecodeHelper.GetValueByProperty<T>(elemItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">double,int,long,datetime,string,bool(True,False / 1,0)</typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(T defaultValue)
        {
            T result = defaultValue;
            if (elemItem != null)
            {
                DecodeHelper.TryParseValue<T>(elemItem.Name.ToString(),elemItem.Value.Trim(), ref result);
            }
            else
            {
                logger.Warning("Get Value Elem is null ");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">double,int,long,datetime,string,bool(True,False / 1,0)</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName, T defaultValue)
        {
            T result = defaultValue;
            if (elemItem != null)
            {
                result = DecodeHelper.GetValueByProperty<T>(elemItem, propertyName, defaultValue);
            }
            else
            {
                logger.Warning("Get Property Elem is null,Property is " + propertyName);
            }
            return result;
        }

    }
}