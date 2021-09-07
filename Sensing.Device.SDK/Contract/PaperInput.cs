using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class PaperInput : PagedSortedAndFilteredInputDto
    {
        public int SoftwareId { get; set; }
    }

    public class QuestionInput : PagedSortedAndFilteredInputDto
    {
        public int PaperId { get; set; }
    }

    public class PaperDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LogoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string FromType { get; set; }
        public string ExtensionData { get; set; }
        public string OuterId { get; set; }
        public int QuestionsCount { get; set; }
        public int ActualQuestionsCount { get; set; }
        public string CompositionType { get; set; }
        public int RandomCount { get; set; }
        public List<QuestionDto> Questions { get; set; }

    }


    public class QuestionDto
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public double Score { get; set; }
        public int OrderNo { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool Enabled { get; set; }
        public string OuterId { get; set; }
        public string QuestionScoreType { get; set; }
        public Questionitem[] QuestionItems { get; set; }
    }

    public class Questionitem
    {
        public int Id { get; set; }
        public int OrderNo { get; set; }
        public string Prefix { get; set; }
        public string Content { get; set; }
        public string LogoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ExtensionData { get; set; }
        public bool IsAnswer { get; set; }
        public double Score { get; set; }
        public string Description { get; set; }
        public string OuterId { get; set; }
    }

}
