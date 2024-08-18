using QuizApp_Razor.Models;

namespace QuizApp_Razor.Database;
public interface IDbService {
    public void CreateQuiz(Quiz quiz, Dictionary<Question, IEnumerable<Answer>> mainData);
    public Task<(Quiz?, Dictionary<Question, IEnumerable<Answer>>)> GetQuiz(int quizId);
    public Task<List<Answer>> GetAnswers(string ids);
}