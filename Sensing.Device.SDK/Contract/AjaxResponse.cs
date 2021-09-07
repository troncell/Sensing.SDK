using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class AjaxResponse<TResult>
    {
        public TResult Result { get; set; }
        public string TargetUrl { get; set; }

        public bool Success { get; set; }

        public ErrorInfo Error { get; set; }

        public bool UnAuthorizedRequest { get; set; }

        public bool __abp { get; } = true;
    }

    public class ErrorInfo
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public class ErrorAjaxResponse
    {
        public string TargetUrl { get; set; }

        public bool Success { get; set; }

        public ErrorInfo Error { get; set; }

        public bool UnAuthorizedRequest { get; set; }

        public bool __abp { get; } = true;
    }

    //public class ApiStatus
    //{
    //    public static string OK = "OK";
    //    public static string NG = "NG";
    //}
    //public class WebApiResult<T>
    //{
    //    public string status { get; set; }
    //    public string message { get; set; }
    //    public string code { get; set; }
    //    public T data { get; set; }
    //}
}
