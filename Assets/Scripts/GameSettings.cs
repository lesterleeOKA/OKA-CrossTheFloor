using SimpleJSON;
using System;

[Serializable]
public class GameSettings : Settings
{
    public int playerNumber = 0;
    public string[] moveItemsImages;
    public string grid_image;
    //public string normal_color;
    //public string pressed_color;
}

public static class SetParams
{
    public static void setCustomParameters(GameSettings settings = null, JSONNode jsonNode= null)
    {
        if (settings != null && jsonNode != null)
        {
            ////////Game Customization params/////////
            var jsonArray = jsonNode["setting"]["move_item_images"].AsArray;

            if(jsonArray != null)
            {
                settings.moveItemsImages = new string[jsonArray.Count];
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    var movingItems = jsonArray[i].ToString().Replace("\"", "");
                    if (!movingItems.StartsWith("https://") || !movingItems.StartsWith(APIConstant.blobServerRelativePath))
                        settings.moveItemsImages[i] = APIConstant.blobServerRelativePath + movingItems;
                }
            }

            string grid_image = jsonNode["setting"]["grid_image"] != null ?
                jsonNode["setting"]["grid_image"].ToString().Replace("\"", "") : null;

            if(jsonNode["setting"]["player_number"] != null)
            {
                settings.playerNumber = jsonNode["setting"]["player_number"];
                LoaderConfig.Instance.gameSetup.playerNumber = settings.playerNumber;
            }

            if (grid_image != null)
            {
                if (!grid_image.StartsWith("https://") || !grid_image.StartsWith(APIConstant.blobServerRelativePath))
                    settings.grid_image = APIConstant.blobServerRelativePath + grid_image;
            }
        }
    }
}

