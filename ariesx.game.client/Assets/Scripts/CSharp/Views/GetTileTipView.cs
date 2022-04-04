using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
	public class GetTileTipView : BaseView {
		private GetTileTipViewModel viewModel;
		private GetTileTipViewPreference viewPref;
		/* UI Members*/

		/*************/

		void Awake() {
			this.viewModel = this.gameObject.GetComponent<GetTileTipViewModel>();
		}

		protected override void OnUIInit() {
			this.ui = UIManager.GetUI("UIGetTileTip");
            this.viewPref = this.ui.GetComponent<GetTileTipViewPreference>();
            /* Cache the ui components here */
            this.viewPref.btnGo.onClick.RemoveAllListeners();
            this.viewPref.btnGo.onClick.AddListener(this.OnBtnGo);
		}

		/* Propert change function */

		/***************************/

        private void OnBtnGo() {
            this.viewModel.GoTileReq();
        }

		protected override void OnVisible() {
		}

		protected override void OnInvisible() {
		}
	}
}
