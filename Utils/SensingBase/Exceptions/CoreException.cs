//-----------------------------------------------------------------------
// <copyright file="CoreException.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-11-01</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace SensingBase.CSException
{
    [Serializable]
    public class CoreException:SensingExceptionBase
    {
         public CoreException(string message)
            : base (message)
        {

        }
        public CoreException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        protected CoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}