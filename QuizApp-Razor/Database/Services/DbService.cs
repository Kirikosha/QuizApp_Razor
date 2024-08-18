using Dapper;
using QuizApp_Razor.Models;
namespace QuizApp_Razor.Database;

public class DbService : IDbService {
    private const string INSERT_QUIZ_QUERY = "INSERT INTO quiz (title) VALUES (@Title); SELECT LAST_INSERT_ID()";
    private const string INSERT_QUESTION_QUERY = "INSERT INTO question (text, `quiz-id`) VALUES (@Text, @QuizId); SELECT LAST_INSERT_ID()";
    private const string INSERT_ANSWER_QUERY = "INSERT INTO answer (text, `is-correct`, `question-id`) VALUES (@Text, @IsCorrect, @QuestionId)";
    private const string GET_QUIZ_QUERY = "SELECT `quiz-id` AS QuizId, title AS Title FROM quiz_app_razor.quiz WHERE `quiz-id` = ";
    private const string GET_QUESTION_QUERY = "SELECT `question-id` AS QuestionId, text AS TEXT, `quiz-id` as QuizId FROM quiz_app_razor.question WHERE `quiz-id` =";
    private const string GET_ANSWER_QUERY = "SELECT `answer-id` AS AnswerId, text AS Text, `is-correct` AS IsCorrect, `question-id` AS QuestionId FROM quiz_app_razor.answer WHERE `question-id` =";
    private const string GET_ANSWERS_BY_IDS = "SELECT `answer-id` AS AnswerId, `is-correct` AS IsCorrect FROM quiz_app_razor.answer WHERE `answer-id` IN";
    IConnector _connector;
    ILogger<DbService> _logger;
    public DbService(IConnector connector, ILogger<DbService> logger)
    {
        _connector = connector;
        _logger = logger;
    }

    #region QuizCreate
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
    #endregion

    #region QuizGet
    public async Task<(Quiz?, Dictionary<Question, IEnumerable<Answer>>)> GetQuiz(int quizId){
        Quiz? quiz = await TryGetQuiz(quizId);
        if (quiz == null){
            return (null, new Dictionary<Question, IEnumerable<Answer>>());
        }
        Dictionary<Question, IEnumerable<Answer>> data = await TryGetQuestionsAnswers(quiz.QuizId);
        if(data.Count() == 0){
            return (null, data);
        }

        return (quiz, data);

    }

    private async Task<Quiz?> TryGetQuiz(int quizId){
        string getQuizQuery = string.Concat(GET_QUIZ_QUERY, quizId);
        Quiz? quiz = await _connector.GetConnection().QueryFirstAsync<Quiz>(getQuizQuery);
        if(CheckForQuizCorruption(quiz, quizId)) return null;
        return quiz;
    }

    private async Task<Dictionary<Question, IEnumerable<Answer>>> TryGetQuestionsAnswers(int quizId){
        string getQuestionQuery = string.Concat(GET_QUESTION_QUERY, quizId);
        IEnumerable<Question> questions = await _connector.GetConnection().QueryAsync<Question>(getQuestionQuery);
        if(questions.Count() == 0){
            _logger.LogError($"The quiz is corrupted because the request returned 0 questions. Quiz-id:{quizId}");
            return new Dictionary<Question, IEnumerable<Answer>>();
        }

        Dictionary<Question, IEnumerable<Answer>> data = new Dictionary<Question, IEnumerable<Answer>>();
        foreach (var question in questions){
            if(CheckForQuestionCorruption(question, quizId)) return new Dictionary<Question, IEnumerable<Answer>>();

            string getAnswerQuery = string.Concat(GET_ANSWER_QUERY, question.QuestionId);
            IEnumerable<Answer> answers = await _connector.GetConnection().QueryAsync<Answer>(getAnswerQuery);
            if(answers.Count() == 0){
                _logger.LogError($"The question is corrupted because the request returned 0 answers. Question-id:{question.QuestionId}");
                return new Dictionary<Question, IEnumerable<Answer>>();
            }
            if(CheckForAnswerCorruption(answers, question.QuestionId)) return new Dictionary<Question, IEnumerable<Answer>>();
            data.Add(question, answers);
        }
        return data;
    }

    private bool CheckForQuizCorruption(Quiz? quiz, int quizId){
        if (quiz == null){
            _logger.LogError($"Can't find specified quiz {quizId}");
            return true;
        }

        if (string.IsNullOrEmpty(quiz.Title)){
            _logger.LogError($"The quiz is corrupted or broken. Quiz id is {quizId}");
            return true;
        }
        return false;
    }
    private bool CheckForQuestionCorruption(Question question, int quizId){
        if(question == null){
            _logger.LogError($"Question inside of a quiz with id {quizId} is corrupted.");
            return true;
        }
        else if (string.IsNullOrEmpty(question.Text)){
            _logger.LogError($"Question with an id of {question.QuestionId} is corrupted");
            return true;
        }
        return false;
    }

    private bool CheckForAnswerCorruption(IEnumerable<Answer> answers, int questionId){
        foreach(var answer in answers){
            if(answer == null){
                _logger.LogError($"Answer inside of a question with id {questionId} is corrupted");
                return true;
            }
            else if(string.IsNullOrEmpty(answer.Text)){
                _logger.LogError($"Answer with an id of {answer.AnswerId} is corrupted");
                return true;
            }
        }
        return false;
    }
    #endregion
    
    #region AnswersGet
    public async Task<List<Answer>> GetAnswers(string ids){
        string getAnswersQuery = string.Concat(GET_ANSWERS_BY_IDS, $"({ids})");
        IEnumerable<Answer> answers = await _connector.GetConnection().QueryAsync<Answer>(getAnswersQuery);
        return answers.ToList();
    }
    #endregion
}