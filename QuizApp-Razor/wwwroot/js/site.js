const letters = ['B', 'C', 'D'];

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
    var question_collection = document.querySelector('.quiz-questions');
    if(question_collection){
        return question_collection.children.length;
    } else{
        return 0;
    }
}

function createAnswer(caller){
    var quiz_answers_div = caller.parentNode.parentNode.children[0].children[1];
    var quiz_title = caller.parentNode.parentNode.children[0].children[0].children[1].name;
    var question_number = quiz_title.split('_')[2];
    var number_of_answers = quiz_answers_div.children.length - 1;
    if(number_of_answers < 4){
        var quiz_answer_html =`
        <div class="quiz-question-answer">
                        <input type="checkbox" name="is_right_${question_number}_${number_of_answers + 1}">
                        <p class="quiz-question-answer-letter">${letters[number_of_answers - 1]})</p>
                        <input class="quiz-question-answer-text" type="text" placeholder="Input your answer here" name="answer_${question_number}_${number_of_answers + 1}">
                        </div>`;
        quiz_answers_div.insertAdjacentHTML('beforeend', quiz_answer_html);
    }
}

function deleteAnswer(caller){
    var quiz_answers_div = caller.parentNode.parentNode.children[0].children[1];
    var number_of_answers = quiz_answers_div.children.length - 1;
    if(number_of_answers > 1){
        quiz_answers_div.removeChild(quiz_answers_div.children[quiz_answers_div.children.length - 1]);
    }
    console.log(quiz_answers_div.children);
}

function deleteQuestion(caller){
    var quiz_question_div = caller.parentNode.parentNode;
    var quiz_questions_div = quiz_question_div.parentNode;
    if(quiz_questions_div.children.length > 1 && window.confirm("Do you REALLY want to delete this question?")){
        quiz_questions_div.removeChild(quiz_question_div);
        renumberQuestions(quiz_questions_div);
    }
}

function renumberQuestions(quiz_questions_div) {
    var question_divs = quiz_questions_div.getElementsByClassName('quiz-question');
    for (var i = 0; i < question_divs.length; i++) {
        var question_number = i + 1; // Numbers start from 1
        var question_title_number = question_divs[i].querySelector('.quiz-question-title-question-number');
        question_title_number.innerText = `Question ${question_number}:`;
        var question_title_input_name = question_divs[i].querySelector(".quiz-question-title-input");
        question_title_input_name.name = `question_title_${question_number}`;

        var question_answers_div = question_divs[i].querySelectorAll(".quiz-question-answer");
        var len = question_answers_div.length;
        for (var j = 0; j < question_answers_div.length; j++){
            question_answers_div[j].children[0].name = `is_right_${question_number}_${j + 1}`;
            question_answers_div[j].children[2].name = `answer_${question_number}_${j + 1}`;
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
        answers.forEach((answer, answerIndex) => {
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