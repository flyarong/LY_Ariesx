using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class LotteryItemView: MonoBehaviour {
        #region ui element
        [SerializeField]
        private Image imgBoard;
        [SerializeField]
        private Transform pnlCount;
        [SerializeField]
        private Button btnDetail;
        [SerializeField]
        private CustomButton btnHeroPool;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private Image imgChest;
        [SerializeField]
        public Transform pnlPrice;
        [SerializeField]
        private TextMeshProUGUI txtPrice;
        [SerializeField]
        private TextMeshProUGUI txtCount;
        #endregion

        private bool isFree;
        public bool IsFree {
            get {
                return this.isFree;
            }
            set {
                this.isFree = value;
                this.OnFreeStatusChange();
            }
        }
        private int count = 0;
        public int Count {
            get {
                return this.count;
            }
            set {
                if (this.count != value) {
                    this.count = value;
                    this.OnFreeStatusChange();
                }
            }
        }



        private GachaGroupConf lotteryConf;
        public GachaGroupConf GachaGroupConf {
            get {
                return this.lotteryConf;
            }
            set {
                if (this.lotteryConf != value) {
                    this.lotteryConf = value;
                    this.OnLotterChange();
                }
            }
        }

        public UnityEvent OnHeroDetailClick {
            get {
                this.btnDetail.onClick.RemoveAllListeners();
                return this.btnDetail.onClick;
            }
        }

        public UnityEvent OnHeroPoolClick {
            get {
                this.btnHeroPool.onClick.RemoveAllListeners();
                return this.btnHeroPool.onClick;
            }
        }

        public void SetLotteryItem(GachaGroupConf lotteryConf, bool showDetail = false) {
            this.GachaGroupConf = lotteryConf;
            this.pnlPrice.gameObject.SetActiveSafe(!showDetail && (this.lotteryConf.price > 0)&&!this.isFree);
        }

        private void OnLotterChange() {
            this.pnlPrice.gameObject.SetActiveSafe((this.lotteryConf.price > 0) && !this.isFree);
            if (this.lotteryConf.price > 0) {
                if (RoleManager.GetResource(Resource.Gem) < this.lotteryConf.price) {
                    this.txtPrice.color = Color.red;
                } else {
                    this.txtPrice.color = Color.white;
                }
                this.txtPrice.text = lotteryConf.price.ToString();
            }
            this.imgChest.sprite =
                ArtPrefabConf.GetChestSprite(this.lotteryConf.chest);
            string chestName = LocalManager.GetValue(this.lotteryConf.chest);
            if (chestName.Contains(" ")) {
                string[] chestNameArr = chestName.CustomSplit(' ');
                chestName = string.Format(" <color=#fefa9cFF>{0}</color> {1}", chestNameArr[0], chestNameArr[1]);
            }
            this.txtName.text = chestName;
        }

        private void OnFreeStatusChange() {
            this.pnlPrice.gameObject.SetActiveSafe((this.lotteryConf.price > 0) && !this.isFree);
            this.imgBoard.sprite = ArtPrefabConf.GetChestSprite(
                    string.Concat(SpritePath.lotteryBackgroundPrefix, this.isFree ? "free" : "notfree"));

            if (this.isFree) {
                AnimationManager.Animate(this.gameObject, "Free");
            } else {
                AnimationManager.Stop(this.gameObject);
            }
            this.imgBoard.material = this.isFree ? PoolManager.GetMaterial(MaterialPath.matScan) : null;
            this.RefreshPrice(RoleManager.GetResource(Resource.Gem));
            this.pnlCount.gameObject.SetActiveSafe(this.isFree && this.count > 1);
            if (this.count > 1) {
                this.txtCount.text = this.Count.ToString();
            }
        }

        public void RefreshPrice(float cash) {
            this.txtPrice.color = (cash < this.lotteryConf.price) ?
                Color.red : Color.white;
        }

    }
}
