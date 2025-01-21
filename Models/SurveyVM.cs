namespace G_CustomerCommunication_API.Models
{
    public class SurveyQuestionsVM
    {
        public string? Question { get; set; }
        public int? AnswerType { get; set; }
        public long? SurveyTemplateId { get; set; }
    }

    public class SurveyAnswersVM
    {
        public long? UserId { get; set; }
        public long? SurveyTemplateId { get; set; }
        public string? Answers { get; set; }
    }

    public class SurveyFiltersVM
    {
        public long? StationId { get; set; }
        public long? NotificationLinkId { get; set; }
    }
}