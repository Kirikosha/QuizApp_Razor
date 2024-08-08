namespace QuizApp_Razor.Models;
public class Answer{
    public int AnswerId{get; set;}
    public string Text {get; set;} = string.Empty;
    public bool IsCorrect {get; set;}
    public int QuestionId {get; set;}
}