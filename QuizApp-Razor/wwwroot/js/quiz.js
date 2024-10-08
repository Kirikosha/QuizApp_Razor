document.getElementById("completeQuiz").addEventListener('submit', function (e) {
    e.preventDefault();
    completeQuiz();
})

function completeQuiz() {
    var answers = getAnswers();
    if (answers == null) {
        return;
    }
    var quizId = document.getElementById("quizId").value;
    $.ajax({
        type: 'POST',
        url: `/Quiz/${quizId}`,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: answers,
        success: function (response) {
            if (response.redirectUrl) {
                // Perform the redirect
                window.location.href = response.redirectUrl;
            } else {
                console.log("Redirection URL not provided");
            }
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }
    });
}

function getAnswers() {
    var answers = document.querySelectorAll('.question-answer');
    var data = [];
    answers.forEach((answer, _) => {
        if (answer.checked) {
            var answerId = getAnswerId(answer);
            if (answerId == '-1') {
                alert("Can't get answer id");
                return null;
            }
            data.push(answerId);
        }
    });
    var json = JSON.stringify(data);
    return json;
}

function getAnswerId(answer) {
    var answerNameArr = answer.name.split('_');
    if (answerNameArr) {
        return answerNameArr[2];
    }
    return '-1';
}
