using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using QuizApp_Razor.Database;
using QuizApp_Razor.Models;
namespace QuizApp_Razor.Pages;

public class CreateQuizModel : PageModel{
    private IDbService _dbService;
    private ILogger _logger;
    public CreateQuizModel(IDbService dbService, ILogger<CreateQuizModel> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }
    public async Task<ActionResult> OnPost(){
        using(var reader = new StreamReader(Request.Body)){
            var jsonString = await reader.ReadToEndAsync();
            if (string.IsNullOrEmpty(jsonString)) return StatusCode(400);
            string? title = (string?)JObject.Parse(jsonString)["title"];
            if (string.IsNullOrEmpty(title)) title = "New Quiz";
            var result = GetMainData(jsonString);
            if (result.Count() > 0){
                _dbService.CreateQuiz(new Quiz{Title = title}, result);
            }
        }
        return StatusCode(202);
    }

    private Dictionary<Question, IEnumerable<Answer>> GetMainData(string jsonString){
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        Dictionary<Question, IEnumerable<Answer>> result = new Dictionary<Question, IEnumerable<Answer>>();
        JsonElement questions;
        var successfulQuestionsGet = rootElement.TryGetProperty("questions", out questions);
        if(!successfulQuestionsGet) {
            _logger.LogError($"Error in {rootElement} questions parsing");
            return new Dictionary<Question, IEnumerable<Answer>>();
        }

        foreach (var questionElement in questions.EnumerateArray()){
            JsonElement questionTitle;
            var successfulQuestionTitleGet = questionElement.TryGetProperty("questionTitle", out questionTitle);
            if (!successfulQuestionTitleGet){
                _logger.LogError($"Error in {questions} question title parsing");
                return new Dictionary<Question, IEnumerable<Answer>>();
            }

            Question question = new Question{
                Text = questionTitle.GetString() ?? "Default question title"
            };

            List<Answer> answers = new List<Answer>();

            JsonElement answersInJson;
            var successfulAnswersGet = questionElement.TryGetProperty("answers", out answersInJson);
            if(!successfulAnswersGet){
                _logger.LogError($"Error in {questions} answers parsing");
                return new Dictionary<Question, IEnumerable<Answer>>();
            }

            foreach(var answerElement in answersInJson.EnumerateArray()){
                JsonElement answerText;
                var successfulAnswerTextGet = answerElement.TryGetProperty("text", out answerText);
                if (!successfulAnswerTextGet){
                    _logger.LogError($"Error in {answerElement} parsing text");
                    return new Dictionary<Question, IEnumerable<Answer>>();
                }

                JsonElement answerIsCorrect;
                var successfulAnswerIsCorrectGet = answerElement.TryGetProperty("isCorrect", out answerIsCorrect);
                if (!successfulAnswerIsCorrectGet){
                    _logger.LogError($"Error in {answerElement} parsing correctness");
                    return new Dictionary<Question, IEnumerable<Answer>>();
                }

                Answer answer = new Answer{
                    Text = answerText.GetString() ?? "Default answer",
                    IsCorrect = answerIsCorrect.GetBoolean()
                };

                answers.Add(answer);
            }

            result.Add(question, answers);
        }
        return result;
    }
}