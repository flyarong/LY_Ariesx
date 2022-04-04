using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
	public class DailyRewardNoRewardViewModel : BaseViewModel {
        private MapViewModel parent;
        private DailyRewardNoRewardView view;
        internal Dictionary<Resource, int> resourceDict;

        /* Model data get set */

        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<DailyRewardNoRewardView>();
		}

		public void Show(LoginRewardConf rewardConf) {
            this.view.PlayShow();
            this.view.SetRewardViewInfo(rewardConf);
        }

		public void Hide() {
            this.view.PlayHide();
        }

        public void ShowHeroInfo(string heroName) {
            this.parent.ShowHeroInfo(heroName,infoType:HeroInfoType.Unlock,isSubWindow:true);
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
