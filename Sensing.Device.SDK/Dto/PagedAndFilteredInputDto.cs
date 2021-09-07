using System.ComponentModel.DataAnnotations;

namespace Sensing.SDK.Contract
{
    public class AppConsts
    {
        public const int DefaultPageSize = 100;
        public const int MaxPageSize = 1000;
    }
    public class PagedAndFilteredInputDto
    {

        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Filter { get; set; }

        public PagedAndFilteredInputDto()
        {
            MaxResultCount = AppConsts.DefaultPageSize;
        }
    }
}