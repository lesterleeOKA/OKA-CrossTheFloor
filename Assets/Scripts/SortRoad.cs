using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SortRoad : MonoBehaviour
{
    public Direction direction = Direction.toRight;
    public int orderLayer = 0;
    public GameObject movingObjectPrefab; // Reference to the MovingObject prefab
    public int maxMovingItems = 2; // Maximum number of moving items allowed
    private Image roadHintImage;
    private Canvas canvas;
    public MovingObject[] movingItems;
    public float minSpeed = 3f; // Minimum speed
    public float maxSpeed = 6f; // Maximum speed

    public enum Direction { none, toLeft, toRight};

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

        if(this.direction == Direction.none) return;

        // Initialize the moving items array
        this.maxMovingItems = LoaderConfig.Instance.gameSetup.maximumObjectsEachRoad;
        this.movingItems = new MovingObject[this.maxMovingItems];
        this.minSpeed = (LoaderConfig.Instance.gameSetup.objectAverageSpeed * 2.5f) - 1f;
        this.maxSpeed = (LoaderConfig.Instance.gameSetup.objectAverageSpeed * 2.5f) + 1f;   
    }

    public void InitRoad()
    {
        float speed = Random.Range(this.minSpeed, this.maxSpeed);
        bool toLeft = this.direction == Direction.toLeft;
        for (int i = 0; i < this.maxMovingItems; i++)
        {
            GameObject movingItemObject = Instantiate(this.movingObjectPrefab, this.transform);
            this.movingItems[i] = movingItemObject.GetComponent<MovingObject>();
            if (this.movingItems[i] != null)
            {
                this.movingItems[i].speed = speed;
                this.movingItems[i].startPosX = toLeft ? 1550f : -1550f;
                this.movingItems[i].transform.localScale = new Vector3(toLeft ? -1f : 1f, 1f, 1f);
                this.movingItems[i].SortLayer = this.orderLayer + 1;
            }
        }
    }


    public void startMovingItems(int roadId)
    {    
        StartCoroutine(this.delayNextItem(3f, roadId));
    }

    private IEnumerator delayNextItem(float delay = 1f, int roadId = -1)
    {
        foreach (var movingItem in this.movingItems)
        {
            if (movingItem != null && this.direction != Direction.none)
            {
                LogController.Instance.debug("roadId" + roadId);
                movingItem.StartNewMovement(roadId);
                yield return new WaitForSeconds(delay);
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
