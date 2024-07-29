const letters = ['B', 'C', 'D'];

function createQuestion(){
        var newElementNumber = getNumberOfQuestions() + 1;
        var quizQuestionHTML = `
            <div class="quiz-question">
                <div class="quiz-question-main-content">
                    <div class="quiz-question-title">
                        <p class="quiz-question-title-question-number question-title-font-size">Question ${newElementNumber}:</p>
                        <input type="text" class="quiz-question-title-input question-title-font-size">
                    </div>
                    <div class="quiz-question-answers">
                        <div class="quiz-question-answer-correctness">
                            <p>Is right</p>
                        </div>
                        <div class="quiz-question-answer">
                            <input type="checkbox">
                            <p class="quiz-question-answer-letter">A)</p>
                            <input class="quiz-question-answer-text" type="text" placeholder="Input your answer here">
                        </div>
                    </div>
                </div>
                <div class="quiz-question-buttons">
                    <button class="circle-button add-answer-button" onclick="createAnswer(this)"></button>
                    <button class="circle-button delete-answer-button" onclick="deleteAnswer(this)"></button>
                    <button class="delete-question-button" onclick="deleteQuestion(this)"></button>
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
    var number_of_answers = quiz_answers_div.children.length - 1;
    if(number_of_answers < 4){
        var quiz_answer_html =`
        <div class="quiz-question-answer">
                        <input type="checkbox">
                        <p class="quiz-question-answer-letter">${letters[number_of_answers - 1]})</p>
                        <input class="quiz-question-answer-text" type="text" placeholder="Input your answer here">
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
        var question_title = question_divs[i].querySelector('.quiz-question-title-question-number');
        question_title.innerText = `Question ${question_number}:`;
    }
}