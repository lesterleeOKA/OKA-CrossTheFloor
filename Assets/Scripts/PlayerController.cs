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
    public CanvasGroup answerBoxCg;
    public Image answerBoxFrame;
    public float speed;
    private Transform characterTransform;
    private float limitMovingYOffsetPercentage = 0.7f;
    [HideInInspector]
    public Canvas characterCanvas = null;
    public Vector3 startPosition = Vector3.zero;
    public int characterOrder = 11;
    private CharacterAnimation characterAnimation = null;
    private TextMeshProUGUI answerBox = null;
    private bool isWalking = false;

    public void Init(CharacterSet characterSet = null, Sprite[] defaultAnswerBoxes = null)
    {
        float posX = UnityEngine.Random.Range(-850f, 850f);
        float posY = UnityEngine.Random.Range(-550f, -700f);
        this.startPosition = new Vector3(posX, posY);
        this.characterTransform = this.transform;
        this.characterTransform.localPosition = this.startPosition;
        this.characterCanvas = this.GetComponent<Canvas>();
        this.characterCanvas.sortingOrder = this.characterOrder;
        this.characterAnimation = this.GetComponent<CharacterAnimation>();
        this.characterAnimation.characterSet = characterSet;

        if(this.answerBoxCg != null ) {
            SetUI.Set(this.answerBoxCg, false);
            this.answerBox = this.answerBoxCg.GetComponentInChildren<TextMeshProUGUI>();
        }

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
                this.joystick.handle.GetComponent<Image>().color = _color;
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
            this.IsCheckedAnswer = true;
            var loader = LoaderConfig.Instance;
            var currentQuestion = QuestionController.Instance?.currentQuestion;
            int eachQAScore = currentQuestion.qa.score.full == 0 ? 10 : currentQuestion.qa.score.full;
            int currentScore = this.Score;
            this.answer = this.answerBox.text.ToLower();
            var lowerQIDAns = currentQuestion.correctAnswer.ToLower();
            int resultScore = this.scoring.score(this.answer, currentScore, lowerQIDAns, eachQAScore);
            this.Score = resultScore;

            StartCoroutine(this.showAnswerResult(this.scoring.correct,()=>
            {
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
            }));
        }
    }

    public IEnumerator showAnswerResult(bool correct, Action onCompleted = null)
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
        GameController.Instance?.UpdateNextQuestion();
        //this.playerReset();
        onCompleted?.Invoke();
        this.IsCheckedAnswer = false;
    }

    public void playerReset()
    {
        float posX = UnityEngine.Random.Range(-850f, 850f);
        float posY = UnityEngine.Random.Range(-550f, -700f);
        this.startPosition = new Vector3(posX, posY);
        this.characterCanvas.sortingOrder = this.characterOrder;
        this.characterTransform.localPosition = this.startPosition;
        this.setAnswer("");
    }

    public void FixedUpdate()
    {
        if(this.joystick == null) return;
        Vector2 direction = new Vector2(this.joystick.Horizontal, this.joystick.Vertical);

        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }

        Vector3 newPosition = this.characterTransform.position + (Vector3)direction * this.speed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize * Camera.main.aspect);
        newPosition.y = Mathf.Clamp(newPosition.y, -Camera.main.orthographicSize * this.limitMovingYOffsetPercentage, Camera.main.orthographicSize * (this.limitMovingYOffsetPercentage - 0.075f));

        this.characterTransform.position = newPosition;
        this.characterTransform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);
        this.answerBox.transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);

        if (direction.magnitude > 0.1f)
        {
            if (!this.isWalking)
            {
                this.isWalking = true; // Set walking state
                this.characterAnimation.PlayWalking(); // Start walking animation
            }
        }
        else
        {
            if (isWalking)
            {
                this.isWalking = false; // Reset walking state
                this.characterAnimation.setIdling(); // Switch to idling animation
            }
        }


        if (SortOrderController.Instance != null)
        {
            SortRoad highestRoad = null;

            foreach (var road in SortOrderController.Instance.roads)
            {
                // Check if the character is above the road
                if (this.characterTransform.position.y >= road.gameObject.transform.position.y)
                {
                    // Track the highest road that is below the character
                    if (highestRoad == null || road.gameObject.transform.position.y > highestRoad.gameObject.transform.position.y)
                    {
                        highestRoad = road;
                    }

                   // road.showRoadHint(true);
                }
                /*else
                {
                    road.showRoadHint(false);
                }*/
            }

            // If we found a road below the character, update the sorting order
            if (highestRoad != null && this.characterCanvas != null)
            {
                int newOrder = highestRoad.orderLayer;
                this.characterCanvas.sortingOrder = newOrder;
            }
            else
            {
                this.characterCanvas.sortingOrder = this.characterOrder;
            }
        }
    }

    public void setAnswer(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            this.answer = "";
            SetUI.Set(this.answerBoxCg, false);
        }
        else
        {
            if(content.Length > 1) { 
                this.answer = content;
            }
            else
            {
                this.answer += content;
            }
            SetUI.Set(this.answerBoxCg, true);
        }

        if(this.answerBox != null)
            this.answerBox.text = this.answer;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider has a specific tag, e.g., "Player"
        if (other.CompareTag("Word"))
        {
            var cell = other.GetComponent<Cell>();
            if (cell != null)
            {
                if (cell.isSelected)
                {
                    LogController.Instance.debug("Player has entered the trigger!" + other.name);
                    AudioController.Instance?.PlayAudio(9);
                    this.setAnswer(cell.content.text);
                    cell.SetTextStatus(false);
                }
            }
        }
        else if (other.CompareTag("MoveItem"))
        {
            if(this.characterCanvas.sortingOrder == other.GetComponent<MovingObject>().sortLayer)
            {
                AudioController.Instance?.PlayAudio(8);
                this.characterTransform.localPosition = this.startPosition;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            var cell = other.GetComponent<Cell>();
            if (cell != null)
            {
                if (cell.isSelected)
                {
                    LogController.Instance.debug("Player has exited the trigger!" + other.name);
                }
            }
        }
    }
}
