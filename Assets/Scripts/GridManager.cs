using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GridManager
{
    public GameObject cellPrefab;
    public Transform parent;
    public int gridRow = 4;
    public int gridColumn = 4;
    public int maxRetries = 10;
    private Cell[,] cells;
    public List<int> showCellIdList = new List<int>();
    HashSet<Vector2Int> usedPositions = null;
    List<Vector2Int> availablePositions = null;
    HashSet<char> usedLetters = null;
    public bool showQuestionWordPosition = false;

    public Cell[,] CreateGrid(string initialWord, Sprite cellSprite = null)
    {
        this.usedPositions = new HashSet<Vector2Int>();
        this.usedLetters = new HashSet<char>();
        this.cells = new Cell[this.gridRow, this.gridColumn];
        this.availablePositions = new List<Vector2Int>();
        var letters = this.ShuffleStringToCharArray(initialWord);
        for (int i = 0; i < this.gridRow; i++)
        {
            for (int j = 0; j < this.gridColumn; j++)
            {
                this.availablePositions.Add(new Vector2Int(i, j));
            }
        }
        System.Random random = new System.Random();
        this.availablePositions = this.availablePositions.OrderBy(x => random.Next()).ToList();

        for (int i = 0; i < this.gridRow; i++)
        {
            for (int j = 0; j < this.gridColumn; j++)
            {
                GameObject cellObject = GameObject.Instantiate(cellPrefab, this.parent != null ? this.parent : null);
                cellObject.name = "Cell_" + i + "_" + j;
                Cell cell = cellObject.GetComponent<Cell>();
                cell.SetTextContent("");
                cell.row = i;
                cell.col = j;
                this.cells[i, j] = cell;
            }
        }

        this.showCellIdList = this.GenerateUniqueRandomIntegers(letters.Length, 0, cells.Length);
        for (int i=0; i < this.showCellIdList.Count; i++)
        {
            Vector2Int position =  this.availablePositions[this.showCellIdList[i]];
            this.cells[position.x, position.y].SetTextContent(letters[i].ToString(), default, cellSprite);
        }
        
        return cells;
    }

    public List<int> GenerateUniqueRandomIntegers(int count, int minValue, int maxValue)
    {
        HashSet<int> uniqueIntegers = new HashSet<int>(); System.Random random = new System.Random(); while (uniqueIntegers.Count < count) { int randomNumber = random.Next(minValue, maxValue); uniqueIntegers.Add(randomNumber); }
        return new List<int>(uniqueIntegers);
    }

    char[] ShuffleStringToCharArray(string input)
    {
        char[] letters = input.ToCharArray();
        System.Random random = new System.Random();
        letters = letters.OrderBy(x => random.Next()).ToArray();

        return letters;
    }
    public void UpdateGridWithWord(string newWord)
    {
       this.PlaceWordInGrid(newWord);
    }

    /*public void setLetterHint(bool status)
    {
        foreach (var wordCellPos in this.questionCells)
        {
            this.cells[wordCellPos.x, wordCellPos.y].SetButtonColor(status? Color.yellow : Color.white);
        }
    }*/

    /*public void setFirstLetterHint(bool status)
    {
        for(int i=0; i< this.questionCells.Count; i++)
        {
            if (i == 0)
            {
                this.cells[this.questionCells[i].x, this.questionCells[i].y].SetTextColor(status ? Color.yellow : Color.black);
            }
        }
    }*/


    void PlaceWordInGrid(string _word)
    {
        System.Random random = new System.Random();
        this.availablePositions = this.availablePositions.OrderBy(x => random.Next()).ToList();

        for (int i = 0; i < this.gridRow; i++)
        {
            for (int j = 0; j < this.gridColumn; j++)
            {
                this.cells[i, j].SetTextContent("");
            }
        }

        var letters = this.ShuffleStringToCharArray(_word);
        this.showCellIdList = this.GenerateUniqueRandomIntegers(letters.Length, 0, cells.Length);

        for (int i = 0; i < this.showCellIdList.Count; i++)
        {
            Vector2Int position = availablePositions[this.showCellIdList[i]];
            this.cells[position.x, position.y].SetTextContent(letters[i].ToString());
        }
    }



}
