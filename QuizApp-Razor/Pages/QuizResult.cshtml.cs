using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizApp_Razor.Models;

public class QuizResultModel : PageModel{
    public string quizTitle {get; set;}
    public int TotalAmountOfCorrectAnswers {get; set;}
    public int UsersAmountOfCorrectAnswers {get; set;}
    public async Task<ActionResult> OnGet(){
        if(TempData["totalCorrect"] != null && TempData["usersCorrect"] != null){
            TotalAmountOfCorrectAnswers = (int)TempData["totalCorrect"];
            UsersAmountOfCorrectAnswers = (int)TempData["usersCorrect"];
        }
        if (TempData["quizTitle"] != null){
            quizTitle = (string)TempData["quizTitle"];
        }
        return new PageResult();
    }
}