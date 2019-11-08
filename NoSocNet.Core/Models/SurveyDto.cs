using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;

namespace NoSocNet.Core.Models
{
    public class SurveyDto
    {
        public SurveyDto()
        {
            this.Questions = new List<QuestionDto>();
            this.Instances = new List<SurveyInstanceDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public SurveyType Type { get; set; }

        public List<QuestionDto> Questions { get; set; }

        public List<SurveyInstanceDto> Instances { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public List<OptionDto> Options { get; set; }

        public SurveyDto Survey { get; set; }

        public QuestionDto()
        {
            this.Options = new List<OptionDto>();
        }
    }

    public class OptionDto
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int Position { get; set; }

        public string Text { get; set; }
    }

    public class SurveyInstanceDto
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public SurveyDto Survey { get; set; }

        public UserDto InitiatorUser { get; set; }

        public List<SurveyUserResultDto> UserResults { get; set; }

        public SurveyInstanceDto()
        {
            this.UserResults = new List<SurveyUserResultDto>();
        }
    }

    public class SurveyUserResultDto
    {
        public int Id { get; set; }

        public UserDto SurveyeeUser { get; set; }

        public SurveyInstanceDto SurveyInstance { get; set; }

        public List<QuestionResultDto> QuestionResults { get; set; }

        public SurveyUserResultDto()
        {
            this.QuestionResults = new List<QuestionResultDto>();
        }
    }

    public class QuestionResultDto
    {
        public int Id { get; set; }

        public DateTime DateGiven { get; set; }

        public QuestionDto Question { get; set; }

        public List<OptionDto> SelectedOptions { get; set; }
    }
}
