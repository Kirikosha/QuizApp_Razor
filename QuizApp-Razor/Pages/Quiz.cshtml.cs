using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizApp_Razor.Database;
using QuizApp_Razor.Models;

namespace QuizApp_Razor.Pages;
public class QuizModel : PageModel {
    private IDbService _dbService;
    private ILogger _logger;
    public Quiz Quiz;
    public Dictionary<Question, IEnumerable<Answer>> QuestionAnswers;
    public QuizModel(IDbService dbService, ILogger<QuizModel> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet (int id){
        var data = await _dbService.GetQuiz(id);
        if(data.Item1 == null){
            return NotFound();
        }
        Quiz = data.Item1;
        QuestionAnswers = data.Item2;
        return new PageResult();
    }

    public async Task<ActionResult> OnPost(int id, [FromBody] List<string> answers)
    {
        int[] ids = answers.Select(int.Parse).ToArray();
        var idsJoined = string.Join(",", ids);
        List<Answer> answersList = await _dbService.GetAnswers(idsJoined);
        int correctGuesses = answersList.Count(a => a.IsCorrect);
        Console.WriteLine($"Your result is {correctGuesses}/{ids.Length}");
        return new StatusCodeResult(204);
    }
}