using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
            this.SetTextStatus(true);
            this.content.text = letter;
            this.content.color = this.defaultColor;
        }
        this.isSelected = !string.IsNullOrEmpty(letter) ? true : false;
        this.setCellStatus(false);
    }

    public void SetTextStatus(bool show, float duration=0.5f)
    {
        if (this.content != null)
        {
            this.content.transform.DOScale(show ? 1f : 0f, duration).SetEase(Ease.InOutSine);
            this.isSelected = show? true : false;
        }
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

    public void setCellStatus(bool show=false)
    {
        if(this.cellImage != null)
        {
            this.cellImage.enabled = show;
        }
    }


}
