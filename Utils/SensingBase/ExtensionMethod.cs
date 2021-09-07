//-----------------------------------------------------------------------
// <copyright file="ExtensionMethod.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-15</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using SensingBase.CSException;
using LogService;

namespace SensingBase
{
    public static class ExtensionMethod
    {
        private static readonly IBizLogger Logger = ServerLogFactory.GetLogger(typeof(ExtensionMethod));
        /// <summary>
        /// if obj is null return string.Empty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringEx(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString();
            }
        }

        public static T ToValue<T>(this string str)
        {
            T result = default(T);
            Type tType = typeof (T);
            if (tType == typeof (double))
            {
                double value;
                if (double.TryParse(str, out value))
                {
                    result = (T) (object) value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to double error!", str));
                }
            }
            else if (tType == typeof (int))
            {
                int value;
                if (int.TryParse(str, out value))
                {
                    result = (T) (object) value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to int error!", str));
                }
            }
            else if (tType == typeof (bool))
            {
                bool value;
                if (bool.TryParse(str, out value))
                {
                    result = (T) (object) value;
                }
                else if (str == "1" || str == "0")
                {
                    result = (T)(object)(str == "1");
                }
                else
                {
                    Logger.Error(string.Format("String {0} to bool error!", str));
                }
            }
            else if (tType == typeof (long))
            {
                long value;
                if (long.TryParse(str, out value))
                {
                    result = (T) (object) value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to long error!", str));
                }
            }
            else if (tType == typeof (string))
            {
                result = (T)(Object)str;
            }
            else if (tType == typeof (DateTime))
            {
                DateTime value;
                if (DateTime.TryParse(str, out value))
                {
                    result = (T) (Object) value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to DateTime error!", str));
                }
            }
            else if (tType == typeof (Guid))
            {
                Guid value;
                try
                {
                    value = new Guid(str);
                    result = (T) (object) value;
                }
                catch (Exception ex)
                {
                    throw new CoreException("Cann't create the GUID", ex);
                }
            }
            else if (tType == typeof (TimeSpan))
            {
                TimeSpan value;


                //value = TimeSpan.ParseExact(keyValue, "HH:mm:ss", null);
                if (TimeSpan.TryParse(str, out value))
                {
                    result = (T) (object) value;
                }
                else
                {
                    throw new CoreException("Can not Trypars timespan");
                }
            }
            return result;
        }


        public static T ToValue<T>(this string str,T defaultValue)
        {
            T result = defaultValue;
            Type tType = typeof(T);
            if (tType == typeof(double))
            {
                double value;
                if (double.TryParse(str, out value))
                {
                    result = (T)(object)value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to double error!", str));
                }
            }
            else if (tType == typeof(int))
            {
                int value;
                if (int.TryParse(str, out value))
                {
                    result = (T)(object)value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to int error!", str));
                }
            }
            else if (tType == typeof(bool))
            {
                bool value;
                if (bool.TryParse(str, out value))
                {
                    result = (T)(object)value;
                }
                else if (str == "1" || str == "0")
                {
                    result = (T)(object)(str == "1");
                }
                else
                {
                    Logger.Error(string.Format("String {0} to bool error!", str));
                }
            }
            else if (tType == typeof(long))
            {
                long value;
                if (long.TryParse(str, out value))
                {
                    result = (T)(object)value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to long error!", str));
                }
            }
            else if (tType == typeof(string))
            {
                result = (T)(Object)str;
            }
            else if (tType == typeof(DateTime))
            {
                DateTime value;
                if (DateTime.TryParse(str, out value))
                {
                    result = (T)(Object)value;
                }
                else
                {
                    Logger.Error(string.Format("String {0} to DateTime error!", str));
                }
            }
            else if (tType == typeof(Guid))
            {
                Guid value;
                try
                {
                    value = new Guid(str);
                    result = (T)(object)value;
                }
                catch (Exception ex)
                {
                    throw new CoreException("Cann't create the GUID", ex);
                }
            }
            else if (tType == typeof(TimeSpan))
            {
                TimeSpan value;


                //value = TimeSpan.ParseExact(keyValue, "HH:mm:ss", null);
                if (TimeSpan.TryParse(str, out value))
                {
                    result = (T)(object)value;
                }
                else
                {
                    throw new CoreException("Can not Trypars timespan");
                }
            }
            return result;
        }
    }
}