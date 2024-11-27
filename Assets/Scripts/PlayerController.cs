using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : UserData
{
    public FixedJoystick joystick;
    public Scoring scoring;
    public string answer = string.Empty;
    public bool IsCheckedAnswer = false;
    public TextMeshProUGUI answerBox;
    public Image answerBoxFrame;
    public float speed;
    private Transform characterTransform;
    private float limitMovingYOffsetPercentage = 0.7f;

    public void Init(string _word, Sprite[] defaultAnswerBoxes = null)
    {
        this.characterTransform = transform;
        if (this.joystick == null)
        {
            this.joystick = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "-controller").GetComponent<FixedJoystick>();
        }

        if (this.PlayerIcons[0] == null)
        {
            this.PlayerIcons[0] = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Icon").GetComponent<PlayerIcon>();
        }

        if (this.scoring.scoreTxt == null)
        {
            this.scoring.scoreTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Score").GetComponent<TextMeshProUGUI>();
        }

        if (this.scoring.resultScoreTxt == null)
        {
            this.scoring.resultScoreTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_ResultScore").GetComponent<TextMeshProUGUI>();
        }

        this.scoring.init();
    }

    public void updatePlayerIcon(bool _status = false, string _playerName = "", Sprite _icon = null, Color32 _color = default)
    {
        for (int i = 0; i < this.PlayerIcons.Length; i++)
        {
            if (this.PlayerIcons[i] != null)
            {
                this.PlayerColor = _color;
                this.PlayerIcons[i].playerColor = _color;
                this.PlayerIcons[i].SetStatus(_status, _playerName, _icon);
            }
        }

    }


    string CapitalizeFirstLetter(string str)
    {
        if (string.IsNullOrEmpty(str)) return str; // Return if the string is empty or null
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    public void checkAnswer(int currentTime, Action onCompleted = null)
    {
        if (!this.IsCheckedAnswer)
        {
            var loader = LoaderConfig.Instance;
            var currentQuestion = QuestionController.Instance?.currentQuestion;
            int eachQAScore = currentQuestion.qa.score.full == 0 ? 10 : currentQuestion.qa.score.full;
            int currentScore = this.Score;
            this.answer = this.answerBox.text.ToLower();
            var lowerQIDAns = currentQuestion.correctAnswer.ToLower();
            int resultScore = this.scoring.score(this.answer, currentScore, lowerQIDAns, eachQAScore);
            this.Score = resultScore;
            this.IsCheckedAnswer = true;

            StartCoroutine(this.showAnswerResult(this.scoring.correct));

            if (this.UserId == 0 && loader != null && loader.apiManager.IsLogined) // For first player
            {
                float currentQAPercent = 0f;
                int correctId = 0;
                float score = 0f;
                float answeredPercentage;
                int progress = (int)((float)currentQuestion.answeredQuestion / QuestionManager.Instance.totalItems * 100);

                if (this.answer == lowerQIDAns)
                {
                    if (this.CorrectedAnswerNumber < QuestionManager.Instance.totalItems)
                        this.CorrectedAnswerNumber += 1;

                    correctId = 2;
                    score = eachQAScore; // load from question settings score of each question

                    Debug.Log("Each QA Score!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + eachQAScore + "______answer" + this.answer);
                    currentQAPercent = 100f;
                }
                else
                {
                    if (this.CorrectedAnswerNumber > 0)
                    {
                        this.CorrectedAnswerNumber -= 1;
                    }
                }

                if (this.CorrectedAnswerNumber < QuestionManager.Instance.totalItems)
                {
                    answeredPercentage = this.AnsweredPercentage(QuestionManager.Instance.totalItems);
                }
                else
                {
                    answeredPercentage = 100f;
                }

                loader.SubmitAnswer(
                           currentTime,
                           this.Score,
                           answeredPercentage,
                           progress,
                           correctId,
                           currentTime,
                           currentQuestion.qa.qid,
                           currentQuestion.correctAnswerId,
                           this.CapitalizeFirstLetter(this.answer),
                           currentQuestion.correctAnswer,
                           score,
                           currentQAPercent,
                           onCompleted
                           );
            }
            else
            {
                onCompleted?.Invoke();
            }
        }
    }

    public IEnumerator showAnswerResult(bool correct)
    {
        float delay = 2f;
        if (correct)
        {
            LogController.Instance?.debug("Add marks" + this.Score);
            GameController.Instance?.setGetScorePopup(true);
            AudioController.Instance?.PlayAudio(1);
            yield return new WaitForSeconds(delay);
            GameController.Instance?.setGetScorePopup(false);
        }
        else
        {
            GameController.Instance?.setWrongPopup(true);
            AudioController.Instance?.PlayAudio(2);
            yield return new WaitForSeconds(delay);
            GameController.Instance?.setWrongPopup(false);
        }
        this.scoring.correct = false;
        this.IsCheckedAnswer = false;
        GameController.Instance?.UpdateNextQuestion();
    }

    private void resetWord()
    {
        if (this.answerBox != null) this.answerBox.text = "";
    }


    public void FixedUpdate()
    {
        if(this.joystick == null) return;
        // Get the joystick input
        Vector2 direction = new Vector2(this.joystick.Horizontal, this.joystick.Vertical);

        // Normalize the direction to prevent faster diagonal movement
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        Vector3 newPosition = this.characterTransform.position + (Vector3)direction * this.speed * Time.fixedDeltaTime;

        // Clamp the position within the screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize * Camera.main.aspect);
        newPosition.y = Mathf.Clamp(newPosition.y, -Camera.main.orthographicSize * this.limitMovingYOffsetPercentage, Camera.main.orthographicSize * (this.limitMovingYOffsetPercentage - 0.075f));

        // Update the character's position
        this.characterTransform.position = newPosition;

        this.characterTransform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);

    }
}
