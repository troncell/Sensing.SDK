//-----------------------------------------------------------------------
// <copyright file="DynamicCreator.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-11-01</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using LogService;
using SensingBase.CSException;
using System;

namespace SensingBase.Utils
{
    //public class DynamicCreator
    //{
    //    private static DynamicCreator instance = new DynamicCreator();

    //    private static readonly IBizLogger logger =
    //      ServerLogFactory.GetLogger(typeof(DynamicCreator));
    //    private DynamicCreator()
    //    {

    //    }

    //    public static DynamicCreator Instance
    //    {
    //        get { return instance; }
    //    }

    //    public Object CreateObject(string dllName, string className)
    //    {
    //        object result = null;
    //        try
    //        {
    //            ObjectHandle handle = Activator.CreateInstance(dllName, className);
    //            result = handle.Unwrap();
    //        }
    //        catch (Exception e)
    //        {
    //            string errorString = string.Format("Reflect class failed. dllName={0}, className={1}", dllName, className);
    //            logger.Error(errorString,e);
    //            CoreException ex = new CoreException(errorString,e);
    //            throw ex;
    //        }
    //        return result;
    //    }

    //}
}