namespace QuizApp_Razor.Models;
public class Question{
    public int QuestionId {get; set;}
    public string Text {get; set;} = string.Empty;
    public int QuizId {get; set;}
}