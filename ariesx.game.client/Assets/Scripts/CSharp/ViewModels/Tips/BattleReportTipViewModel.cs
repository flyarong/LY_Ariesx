using UnityEngine;
using UnityEngine.Events;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class BattleReportTipViewModel : BaseViewModel, IViewModel {
        private BattleReportTipView view;
        private UnityAction clickCallback = null;
        private MapViewModel parent;
        private TroopModel troopModel;

        void Awake() {
            this.view = this.gameObject.AddComponent<BattleReportTipView>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.troopModel = ModelManager.GetModelData<TroopModel>();
        }

        public void Show(TroopArrivedNtf message, UnityAction clickCallback) {
            this.view.SetContent(message);
            this.clickCallback = clickCallback;
        }

        public void ShowDefend(TroopHasBeenAttackedNtf message, UnityAction clickCallback) {
            this.view.SetDefendContent(message);
            this.clickCallback = clickCallback;
        }

        public void ShowTopHUD() {
            this.parent.ShowTopHUD();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void PlayReport() {
            this.clickCallback.InvokeSafe();
        }

        public Troop GetTroopByArmyCampName(string armyCamp) {
            return this.troopModel.GetTroopByArmyCampName(armyCamp);
        }

    }
}
