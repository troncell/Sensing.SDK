//-----------------------------------------------------------------------
// <copyright file="SensingExceptionBase.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-15</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace SensingBase.CSException
{
    [Serializable]
    public abstract class SensingExceptionBase: ApplicationException
    {

        protected SensingExceptionBase(SerializationInfo info, StreamingContext context)
            : base
                (info, context)
        {

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        
        public SensingExceptionBase(string message)
            : base (message)
        {

        }
        public SensingExceptionBase(string message,Exception innerException):base
            (message, innerException)
        {

        }

    }
}