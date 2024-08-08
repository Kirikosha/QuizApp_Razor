using QuizApp_Razor.Models;

namespace QuizApp_Razor.Database;
public interface IDbService {
    public void CreateQuiz(Quiz quiz, Dictionary<Question, IEnumerable<Answer>> mainData);
}