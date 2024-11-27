using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    public int playerId = -1;
    public TextMeshProUGUI content;
    private Image cellImage = null;
    public Sprite[] cellSprites;
    public Color32 defaultColor = Color.black;
    public Color32 selectedColor = Color.white;
    public int row;
    public int col;
    public bool isSelected = false;

    public void SetTextContent(string letter="", Color _color = default, Sprite gridSprite = null)
    {
        if(gridSprite != null) this.cellSprites[0] = gridSprite;
        if (this.cellImage == null) 
            this.cellImage = this.GetComponent<Image>();

        this.SetButtonColor(_color);
        this.cellImage.sprite = this.cellSprites[0];

        if (this.content != null) {
            this.content.text = letter;
            this.content.color = this.defaultColor;
        }
        this.isSelected = !string.IsNullOrEmpty(letter) ? true : false;
    }

    public void SetButtonColor(Color _color = default)
    {
        if (_color != default(Color))
            this.cellImage.color = _color;
        else
            this.cellImage.color = Color.white;
    }

    public void SetTextColor(Color _color = default)
    {
        if (this.content != null)
        {
            if (_color != default(Color))
                this.content.color = _color;
            else
                this.content.color = Color.black;
        }
    }


}
