using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class ProductCommentModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CommentDateTime { get; set; }
        public string Content { get; set; }
        public int OrderNumber { get; set; }
        public string Description { get; set; }

        public long ProductId { get; set; }
    }
}
