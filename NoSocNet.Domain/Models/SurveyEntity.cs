using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Models
{
    public enum SurveyType
    {
        Private,
        Public
    }

    public class SurveyEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        #endregion

        #region Fields

        public string Name { get; set; }

        public string Description { get; set; }

        public SurveyType Type { get; set; }

        #endregion

        #region Navigational properties

        public virtual ICollection<QuestionEntity> Questions { get; set; }

        public virtual ICollection<SurveyInstanceEntity> Instances { get; set; }
        #endregion
    }

    public class QuestionEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        public int SurveyId { get; set; }
        #endregion

        #region Fields
        public string Text { get; set; }

        #endregion

        #region Navigational properties
        public SurveyEntity Survey { get; set; }

        public virtual ICollection<OptionEntity> Options { get; set; }

        #endregion
    }

    public class OptionEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }
        #endregion

        #region FK
        public int QuestionId { get; set; }
        #endregion

        #region Fields
        public string Text { get; set; }

        public int Position { get; set; }

        public string Value { get; set; }
        #endregion

        #region Navigational properties
       
        public QuestionEntity Question { get; set; }
        #endregion
    }

    public class SurveyInstanceEntity : IIdentifiable<int>
    {
        #region PK

        public int Id { get; set; }

        #endregion

        #region FK
        public string InitiatorUserId { get; set; }
        public int SurveyId { get; set; }
        #endregion

        #region Fields

        public DateTime CreatedDate { get; set; }

        public Nullable<DateTime> BeginningDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }
        #endregion

        #region Navigational properties

        public SurveyEntity Survey { get; set; }

        public UserEntity InitiatorUser { get; set; }
        public virtual ICollection<SurveyUserResultEntity> UserResults { get; set; }
        #endregion
    }

    public class SurveyUserResultEntity : IIdentifiable<int>
    {
        #region PK
        public int Id { get; set; }
        #endregion

        #region FK
        public int SurveyInstanceId { get; set; }
        public string SurveyeeUserId { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Navigational properties
        public UserEntity SurveyeeUser { get; set; }
        public SurveyInstanceEntity SurveyInstance { get; set; }
        public virtual ICollection<QuestionResultEntity> QuestionResults { get; set; }
        #endregion
        public SurveyUserResultEntity()
        {
            this.QuestionResults = new HashSet<QuestionResultEntity>();
        }
    }

    public class QuestionResultEntity : IIdentifiable<int>
    {
        #region PK
        public int Id { get; set; }
        #endregion

        #region FK
        public int SurveyUserResultId { get; set; }
        public int QuestionId { get; set; }
        #endregion

        #region Fields

        public DateTime CreatedDate { get; set; }

        #endregion

        #region Navigational properties
        public QuestionEntity Question { get; set; }

        public SurveyUserResultEntity SurveyUserResult { get; set; }

        public virtual ICollection<SelectedOptionEntity> SelectedOptions { get; set; }
        #endregion
    }

    public class SelectedOptionEntity
    {
        #region PK,FK
        public int QuestionResultId { get; set; }

        public int OptionId { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Navigational properties

        public OptionEntity Option { get; set; }

        public QuestionResultEntity QuestionResult { get; set; }
        #endregion
    }
}