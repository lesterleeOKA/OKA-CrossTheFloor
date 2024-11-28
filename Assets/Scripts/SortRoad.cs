using UnityEngine;
using UnityEngine.UI;

public class SortRoad : MonoBehaviour
{
    public int orderLayer = 0;
    private Image roadHintImage;
    private Canvas canvas;
    public MovingObject movingItem;
    // Start is called before the first frame update
    void Start()
    {
        if(this.canvas == null)
        {
            this.canvas = this.GetComponent<Canvas>();
            if(this.canvas != null ) {   
               this.canvas.sortingOrder = this.orderLayer;
            }
        }

        if(this.roadHintImage == null)
        {
            this.roadHintImage = this.GetComponent<Image>();
            this.showRoadHint(false);
        }

        if(this.movingItem == null)
        {
            this.movingItem = this.GetComponentInChildren<MovingObject>();
            if(this.movingItem != null) {
               this.movingItem.sortLayer = this.orderLayer;
            }
        }
    }

    public void setOrder(int newOrder)
    {
        if(this.canvas != null)
        {
            this.orderLayer = newOrder;
            this.canvas.sortingOrder = this.orderLayer;
        }
    }

    public void showRoadHint(bool status)
    {
        if(this.roadHintImage != null)
        {
            this.roadHintImage.enabled = status;
        }
    }
}
