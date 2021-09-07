using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{

    public class PaperAnswerReportDto
    {
        public int QuestionItemId { get; set; }
        public string QuestionItemName { get; set; }
        public string Prefix { get; set; }
        public int Count { get; set; }
    }


}
