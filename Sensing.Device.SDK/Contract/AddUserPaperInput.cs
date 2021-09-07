using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{

    public class AddUserPaperInput
    {
        public string subkey { get; set; }
        public int paperId { get; set; }
        public int userActionId { get; set; }
        public DateTime examStartTime { get; set; }
        public DateTime examEndTime { get; set; }
        public int answeredQuestionCount { get; set; }
        public int correctCount { get; set; }
        public int totalScore { get; set; }
        public string examResult { get; set; }
        public PaperQuestion[] paperQuestions { get; set; }
    }

    public class PaperQuestion
    {
        public int questionId { get; set; }
        public string answer { get; set; }
        public string comments { get; set; }
        public int[] questionItemIds { get; set; }
    }

}
