using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameController : GameBaseController
{
    public static GameController Instance = null;
    public GridManager gridManager;
    public Cell[,] grid;
    public GameObject playerPrefab;
    public Transform parent;
    public Color[] playersColor;
    public Sprite[] defaultAnswerBox;
    public List<PlayerController> playerControllers = new List<PlayerController>();
    private bool showWordHints = false;

    protected override void Awake()
    {
        if (Instance == null) Instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator InitialQuestion()
    {
        QuestionController.Instance?.nextQuestion();
        string word = QuestionController.Instance.currentQuestion.correctAnswer;

        yield return new WaitForEndOfFrame();

        Sprite gridTexture = LoaderConfig.Instance.gameSetup.gridTexture != null ?
            SetUI.ConvertTextureToSprite(LoaderConfig.Instance.gameSetup.gridTexture as Texture2D) : null;

        this.grid = gridManager.CreateGrid(word, gridTexture);


         for (int i = 0; i < this.playerNumber; i++)
         {
             var playerController = GameObject.Instantiate(this.playerPrefab, this.parent).GetComponent<PlayerController>();
             playerController.gameObject.name = "Player_" + i;
             playerController.UserId = i;
             this.playerControllers.Add(playerController);
             this.playerControllers[i].Init(word, this.defaultAnswerBox);

             if (i == 0 && LoaderConfig.Instance != null && LoaderConfig.Instance.apiManager.peopleIcon != null)
             {
                 var _playerName = LoaderConfig.Instance?.apiManager.loginName;
                 var icon = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);
                 this.playerControllers[i].UserName = _playerName;
                 this.playerControllers[i].updatePlayerIcon(true, _playerName, icon, this.playersColor[i]);
             }
             else
             {
                 this.playerControllers[i].updatePlayerIcon(true, null, null, this.playersColor[i]);
             }
         }
    }


    public override void enterGame()
    {
        base.enterGame();
        StartCoroutine(this.InitialQuestion());
    }

    public override void endGame()
    {
        bool showSuccess = false;
        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            if(i < this.playerNumber)
            {
                var playerController = this.playerControllers[i];
                if (playerController != null)
                {
                    if (playerController.Score >= 30)
                    {
                        showSuccess = true;
                    }
                    this.endGamePage.updateFinalScore(i, playerController.Score);
                }
            }
        }
        this.endGamePage.setStatus(true, showSuccess);

        base.endGame();
    }

    public void UpdateNextQuestion()
    {
        LogController.Instance?.debug("Next Question");
        QuestionController.Instance?.nextQuestion();
        string word = QuestionController.Instance.currentQuestion.correctAnswer;
        this.gridManager.UpdateGridWithWord(word);
    }

   

    private void Update()
    {
        if(!this.playing) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.UpdateNextQuestion();
        }
       
    }

    
}
