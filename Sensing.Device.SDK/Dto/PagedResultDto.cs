using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public IList<T> Items { get; set; }
        public PagedResultDto()
        {

        }
        public PagedResultDto(int totalCount, IList<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}
