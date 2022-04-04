using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class SmartGiftBagViewModel : BaseViewModel {
        private PayViewModel parent;
        private SmartGiftBagView view;
        /* Model data get set */
        public HeroItem heroConf;
        public float currentFragmentCount;
        public float targetFragmentCount;

        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<PayViewModel>();
            this.view = this.gameObject.AddComponent<SmartGiftBagView>();
        }

        public void Show(HeroItem heroConf) {
            SetHeroData(heroConf);
            this.view.PlayShow();
            LYGameData.OnOtherClickBuyButton(
                heroConf.HeroName,
                heroConf.FragmentCount.ToString(),
                heroConf.GoldPrice.ToString()
            );
        }

        private void SetHeroData(HeroItem heroConf) {
            this.heroConf = heroConf;
            if (HeroModel.Instance.heroDict.ContainsKey(this.heroConf.HeroName)) {
                currentFragmentCount = HeroModel.Instance.heroDict[this.heroConf.HeroName].FragmentCount;
            } else {
                currentFragmentCount = 0;
            }
            targetFragmentCount = this.heroConf.FragmentCount + currentFragmentCount;
        }

        public void Hide() {
            this.view.PlayHide();
        }

        /* Add 'NetMessageAck' function here*/
        //private bool isGetDailyShop = false;
        public void GetDailyShopReq() {
            GetDailyShopReq getDailyShopReq = new GetDailyShopReq() {
                Fetch = this.heroConf.Fence,
                Catagory = 0
            };
            NetManager.SendMessage(getDailyShopReq,
                                    typeof(GetDailyShopAck).Name,
                                    this.GetDailyShopAck,
                                    this.ErrorGetDailyShopAck);
            //this.isGetDailyShop = true;
        }

        private void GetDailyShopAck(IExtensible message) {
            GetDailyShopAck getDailyShopAck = message as GetDailyShopAck;
            if (HeroModel.Instance.heroDict.ContainsKey(getDailyShopAck.Hero.Name))
                HeroModel.Instance.heroDict[getDailyShopAck.Hero.Name] = getDailyShopAck.Hero;
            else {
                HeroModel.Instance.heroDict.Add(getDailyShopAck.Hero.Name, getDailyShopAck.Hero);
                HeroModel.Instance.unlockHeroDict.Remove(getDailyShopAck.Hero.Name);
                HeroModel.Instance.NewHeroCount++;
            }
            LYGameData.OnOtherBuyItemSuccess(
               heroConf.HeroName,
               heroConf.FragmentCount.ToString(),
               heroConf.GoldPrice.ToString()
           );
            targetFragmentCount = getDailyShopAck.Hero.FragmentCount;
            this.heroConf.IsCollect = true;
            this.parent.RefreshSmartGiftBagHeroItem(this.heroConf);
            this.view.PayedSmartGiftBag();
            //this.isGetDailyShop = false;
        }

        private void ErrorGetDailyShopAck(IExtensible message) {
            UIManager.ShowTip(LocalManager.GetValue("ErrorGetDailyShopAck"), TipType.Error);
            //this.isGetDailyShop = false;
        }

        /***********************************/
    }
}
