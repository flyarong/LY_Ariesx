using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ChestSetting : MonoBehaviour {
        public Image imgChest;

        public void SetChestContent(string chestName) {
            if (string.IsNullOrEmpty(chestName)) {
                return;
            }

            this.imgChest.sprite =
                    ArtPrefabConf.GetChestSprite(chestName);
        }
    }
}
