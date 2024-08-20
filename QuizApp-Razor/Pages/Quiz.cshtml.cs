using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizApp_Razor.Database;
using QuizApp_Razor.Models;

namespace QuizApp_Razor.Pages;
public class QuizModel : PageModel {
    private IDbService _dbService;
    private ILogger _logger;
    [BindProperty]
    public Quiz Quiz { get; set; }
    public Dictionary<Question, IEnumerable<Answer>> QuestionAnswers;
    public QuizModel(IDbService dbService, ILogger<QuizModel> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet (int id){
        var data = await _dbService.GetQuiz(id);
        if(data.Item1 == null){
            _logger.LogError($"Quiz was not found. Maybe quiz id is broken or something went wrong {id}");
            return NotFound();
        }
        Quiz = data.Item1;
        TempData["quizTitle"] = Quiz.Title;
        QuestionAnswers = data.Item2;
        return new PageResult();
    }

    public async Task<IActionResult> OnPost(int id, [FromBody] List<string> answers)
    {
        int[] ids = answers.Select(int.Parse).ToArray();
        var idsJoined = string.Join(",", ids);
        List<Answer> answersList = await _dbService.GetAnswers(idsJoined);
        int correctGuesses = answersList.Count(a => a.IsCorrect);
        int maxGuesses = await _dbService.GetCorrectAnswerCount(id);
        if(maxGuesses == -1){
            _logger.LogError("Something went wrong when the max amount of points were requested from db");
            return new StatusCodeResult(400);
        }
        TempData["totalCorrect"] = maxGuesses;
        TempData["usersCorrect"] = correctGuesses;
        var redirectUrl = Url.Page("QuizResult");
        return new JsonResult(new { redirectUrl });
    }
}