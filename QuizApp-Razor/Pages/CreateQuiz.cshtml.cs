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
        JsonElement questions = Parse("questions", "questions parsing", rootElement);
        if (questions.ValueKind == JsonValueKind.Undefined) return new Dictionary<Question, IEnumerable<Answer>>();

        foreach (var questionElement in questions.EnumerateArray()){
            Question? question = GetQuestion(questionElement);
            if(question == null) return new Dictionary<Question, IEnumerable<Answer>>();
            List<Answer> answers = GetAnswers(questionElement);
            result.Add(question, answers);
        }
        return result;
    }

    private Question? GetQuestion(JsonElement questionElement){
        JsonElement questionTitle = Parse("questionTitle", "question title parsing", questionElement);
        if (questionTitle.ValueKind == JsonValueKind.Undefined) return null;

        Question question = new Question{
            Text = questionTitle.GetString() ?? "Default question title"
        };
        return question;
    }

    private List<Answer> GetAnswers(JsonElement questionElement){
        List<Answer> answers = new List<Answer>();
        JsonElement answersInJson = Parse("answers", "answers parsing", questionElement);
        if(answersInJson.ValueKind == JsonValueKind.Undefined) return new List<Answer>();

        foreach(var answerElement in answersInJson.EnumerateArray()){
            JsonElement answerText = Parse("text", "parsing text", answerElement);
            if(answersInJson.ValueKind == JsonValueKind.Undefined) return new List<Answer>();

            JsonElement answerIsCorrect = Parse("isCorrect", "parsing correctness", answerElement);
            if(answerIsCorrect.ValueKind == JsonValueKind.Undefined) return new List<Answer>();

            Answer answer = new Answer{
                Text = answerText.GetString() ?? "Default answer",
                IsCorrect = answerIsCorrect.GetBoolean()
            };

            answers.Add(answer);
        }
        return answers;
    }

    private JsonElement Parse(string propertyName, string errorMessage, JsonElement parent){
        JsonElement result;
        bool isSuccess = parent.TryGetProperty(propertyName, out result);
        if (!isSuccess){
            _logger.LogError($"Error in {parent} {errorMessage}");
            return default;
        }
        return result;
    }
}