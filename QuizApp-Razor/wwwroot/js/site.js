const letters = ['B', 'C', 'D'];

document.getElementById("creationForm").addEventListener('submit', function(e){
    e.preventDefault();

    var title = document.getElementById("title").value.trim();
    if (!title){
        alert("Quiz title is required");
        return;
    }

    var questionsDiv = document.getElementById("questions");
    var questions = questionsDiv.querySelectorAll(".quiz-question");

    let isFormValid = true; // Flag to check if form is valid

    questions.forEach((question, qIndex) =>{
        if(!isFormValid){
            return;
        }
        const questionTitle = question.querySelector(`input[name="question_title_${qIndex + 1}"]`).value.trim();
        if (!questionTitle){
            alert(`Question ${qIndex + 1} title is required`);
            isFormValid = false;
            return;
        }

        var answers = question.querySelectorAll(".quiz-question-answer");
        var atLeastOneCorrect = 0;

        answers.forEach((answer, aIndex) => {
            const answerCorrect = answer.querySelector(`input[name="is_right_${qIndex + 1}_${aIndex + 1}"]`).checked;
            atLeastOneCorrect = answerCorrect ? atLeastOneCorrect + 1 : atLeastOneCorrect;

            const answerText = answer.querySelector(`input[name="answer_${qIndex + 1}_${aIndex + 1}"]`).value.trim();
            if(!answerText){
                alert(`Answer ${aIndex + 1} in question ${qIndex + 1} hasn't or has a wrong text`);
                isFormValid = false;
                return;
            }
        });

        if (atLeastOneCorrect == 0){
            alert(`At least one answer in question number ${qIndex + 1} should be correct`);
            isFormValid = false;
            return;
        }
    });

    if (isFormValid) {
        getFields(); // Only call this if the form is valid
    }
});
function createQuestion(){
        var newElementNumber = getNumberOfQuestions() + 1;
        var quizQuestionHTML = `
        <div class="quiz-question">
            <div class="quiz-question-main-content">
                <div class="quiz-question-title">
                    <p class="quiz-question-title-question-number question-title-font-size">Question ${newElementNumber}:</p>
                    <input type="text" class="quiz-question-title-input question-title-font-size" name="question_title_${newElementNumber}">
                </div>
                <div class="quiz-question-answers">
                    <div class="quiz-question-answer-correctness">
                        <p>Is right</p>
                    </div>
                    <div class="quiz-question-answer">
                        <input type="checkbox" name="is_right_${newElementNumber}_1">
                        <p class="quiz-question-answer-letter">A)</p>
                        <input class="quiz-question-answer-text" type="text" placeholder="Input your answer here" name="answer_${newElementNumber}_1">
                    </div>
                </div>
            </div>
            <div class="quiz-question-buttons">
                <button class="circle-button add-answer-button" onclick="createAnswer(this)" type="button"></button>
                <button class="circle-button delete-answer-button" onclick="deleteAnswer(this)" type="button"></button>
                <button class="delete-question-button" onclick="deleteQuestion(this)" type="button"></button>
            </div>
        </div>
        `;
        var quizQuestions = document.getElementById('questions');
        quizQuestions.insertAdjacentHTML('beforeend', quizQuestionHTML);
}

function getNumberOfQuestions(){
    var questionCollection = document.querySelector('.quiz-questions');
    if(questionCollection){
        return questionCollection.children.length;
    } else{
        return 0;
    }
}

function createAnswer(caller){
    var quizAnswersDiv = caller.parentNode.parentNode.children[0].children[1];
    var quizTitle = caller.parentNode.parentNode.children[0].children[0].children[1].name;
    var questionNumber = quizTitle.split('_')[2];
    var numberOfAnswers = quizAnswersDiv.children.length - 1;
    if(numberOfAnswers < 4){
        var quizAnswerHtml =`
        <div class="quiz-question-answer">
                        <input type="checkbox" name="is_right_${questionNumber}_${numberOfAnswers + 1}">
                        <p class="quiz-question-answer-letter">${letters[numberOfAnswers - 1]})</p>
                        <input class="quiz-question-answer-text" type="text" placeholder="Input your answer here" name="answer_${questionNumber}_${numberOfAnswers + 1}">
                        </div>`;
        quizAnswersDiv.insertAdjacentHTML('beforeend', quizAnswerHtml);
    }
}

function deleteAnswer(caller){
    var quizAnswersDiv = caller.parentNode.parentNode.children[0].children[1];
    var numberOfAnswers = quizAnswersDiv.children.length - 1;
    if(numberOfAnswers > 1){
        quizAnswersDiv.removeChild(quizAnswersDiv.children[quizAnswersDiv.children.length - 1]);
    }
    console.log(quizAnswersDiv.children);
}

function deleteQuestion(caller){
    var quizQuestionDiv = caller.parentNode.parentNode;
    var quizQuestionsDiv = quizQuestionDiv.parentNode;
    if(quizQuestionsDiv.children.length > 1 && window.confirm("Do you REALLY want to delete this question?")){
        quizQuestionsDiv.removeChild(quizQuestionDiv);
        renumberQuestions(quizQuestionsDiv);
    }
}

function renumberQuestions(quizQuestionsDiv) {
    var questionDivs = quizQuestionsDiv.getElementsByClassName('quiz-question');
    for (var i = 0; i < questionDivs.length; i++) {
        var questionNumber = i + 1; // Numbers start from 1
        var questionTitleNumber = questionDivs[i].querySelector('.quiz-question-title-question-number');
        questionTitleNumber.innerText = `Question ${questionNumber}:`;
        var questionTitleInputName = questionDivs[i].querySelector(".quiz-question-title-input");
        questionTitleInputName.name = `question_title_${questionNumber}`;

        var questionAnswersDiv = questionDivs[i].querySelectorAll(".quiz-question-answer");
        var len = questionAnswersDiv.length;
        for (var j = 0; j < questionAnswersDiv.length; j++){
            questionAnswersDiv[j].children[0].name = `is_right_${questionNumber}_${j + 1}`;
            questionAnswersDiv[j].children[2].name = `answer_${questionNumber}_${j + 1}`;
        }
    }
}

function getFields() {
    const form = document.getElementById('creationForm');
    const formData = new FormData(form);
    const jsonObject = {};

    var title = document.getElementById("title");
    jsonObject.title = title.value;
    const questions = document.querySelectorAll('.quiz-question');
    jsonObject.questions = [];

    questions.forEach((question, index) => {
        const questionNumber = index + 1;
        const questionData = {
            questionTitle: formData.get(`question_title_${questionNumber}`),
            answers: []
        };

        const answers = question.querySelectorAll('.quiz-question-answer');
        answers.forEach((_, answerIndex) => {
            const answerNumber = answerIndex + 1;
            questionData.answers.push({
                text: formData.get(`answer_${questionNumber}_${answerNumber}`),
                isCorrect: formData.get(`is_right_${questionNumber}_${answerNumber}`) !== null
            });
        });

        jsonObject.questions.push(questionData);
    });

    const json = JSON.stringify(jsonObject);

    $.ajax({
        type : "POST",
        url : "/CreateQuiz",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
            $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data : json,
        success : function(){
            console.log("success");
        }
    })

}

function success(){
    console.log("success");
}
