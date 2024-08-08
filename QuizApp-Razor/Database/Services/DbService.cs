using Dapper;
using QuizApp_Razor.Models;
namespace QuizApp_Razor.Database;

public class DbService : IDbService {
    private const string INSERT_QUIZ_QUERY = "INSERT INTO quiz (title) VALUES (@Title); SELECT LAST_INSERT_ID()";
    private const string INSERT_QUESTION_QUERY = "INSERT INTO question (text, `quiz-id`) VALUES (@Text, @QuizId); SELECT LAST_INSERT_ID()";
    private const string INSERT_ANSWER_QUERY = "INSERT INTO answer (text, `is-correct`, `question-id`) VALUES (@Text, @IsCorrect, @QuestionId)";
    IConnector _connector;
    ILogger<DbService> _logger;
    public DbService(IConnector connector, ILogger<DbService> logger)
    {
        _connector = connector;
        _logger = logger;
    }

    public void CreateQuiz(Quiz quiz, Dictionary<Question, IEnumerable<Answer>> mainData){
        int quizId = InsertQuiz(quiz);
        bool result = InsertMainData(quizId, mainData);
    }

    private int InsertQuiz(Quiz quiz){
        int id = -1;
        try{
            id = _connector.GetConnection().ExecuteScalar<int>(INSERT_QUIZ_QUERY, quiz);
        }
        catch (Exception ex){
            _logger.LogError(ex.Message);
            id = -1;
        }
        return id;
    }

    private bool InsertMainData(int quizId, Dictionary<Question, IEnumerable<Answer>> mainData){
        bool result = true;
        foreach(KeyValuePair<Question, IEnumerable<Answer>> pair in mainData){
            pair.Key.QuizId = quizId;
            int id = -1;
            try{
                id = _connector.GetConnection().ExecuteScalar<int>(INSERT_QUESTION_QUERY, pair.Key);
                foreach (var answer in pair.Value){
                    answer.QuestionId = id;
                    _connector.GetConnection().Query(INSERT_ANSWER_QUERY, answer);
                }
            }
            catch (Exception ex){
                _logger.LogError(ex.Message);
                result = false;
                break;
            }
        }
        return result;
    }
}