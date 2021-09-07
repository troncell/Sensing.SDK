
namespace Sensing.SDK.Contract
{

    public class PagedAndSortedInputDto : PagedInputDto
    {
        public string Sorting { get; set; }

        public PagedAndSortedInputDto()
        {
            MaxResultCount = AppConsts.DefaultPageSize;
        }
    }
}