using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class MarchConfirmViewModel : BaseViewModel, IViewModel {
        private TroopInfoViewModel parent;
        private MarchConfirmView view;
        /* Model data get set */

        /**********************/

        /* Other members */
        private BattleSimulationResult result;
        public BattleSimulationResult Result {
            get {
                return this.result;
            }
            set {
                if (this.result != value) {
                    this.result = value;
                    this.view.SetMarchConfirm();
                }
            }
        }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<TroopInfoViewModel>();
            this.view = this.gameObject.AddComponent<MarchConfirmView>();
        }

        public void Show(BattleSimulationResult result) {
            this.view.PlayShow();
            this.Result = result;
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void HideTroop(bool needHideTileInfo = true) {
            this.parent.Hide(needHideTileInfo);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                if (FteManager.Instance.curStep == string.Empty) {
                    FteManager.HideFteMask();
                }
            });
        }

        /* Net*/

        public void MarchReq() {
            this.parent.MarchReq();
            this.HideTroop();
        }

        public void Recruit() {
            if (this.result == BattleSimulationResult.Hard) {
                FteManager.SetCurrentTroop(this.parent.CurrentTroop, Vector2.zero, string.Empty);
                FteManager.StartFte(GameConst.RECRUIT, 2, true);
            } else {
                this.view.afterHideCallback = () => this.parent.ShowHero(HeroSubViewType.All);
            }
            this.HideTroop();
        }

        /***********************************/
    }
}
