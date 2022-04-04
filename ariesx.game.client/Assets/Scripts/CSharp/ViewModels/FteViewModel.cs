using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class FteViewModel : BaseViewModel, IViewModel {
        private FteModel model;
        private FteView view;
        //bool isNonEnforceFte = false;
        /* Model data get set */
        public List<string> FTEList {
            get {
                return this.model.fteList;
            }
        }

        public string CurrentTroop {
            get {
                return this.model.curTroop;
            }
            set {
                if (this.model.curTroop != value) {
                    this.model.curTroop = value;
                }
            }
        }

        public string CurrentTroopName {
            get {
                return this.model.curTroopName;
            }
            set {
                if (this.model.curTroopName != value) {
                    this.model.curTroopName = value;
                }
            }
        }

        public Vector2 CurrentTarget {
            get {
                return this.model.curTargetCoord;
            }
            set {
                if (this.model.curTargetCoord != value) {
                    this.model.curTargetCoord = value;
                }
            }
        }

        public int CurrentTaskId {
            get {
                return this.model.curTaskId;
            }
            set {
                if (this.model.curTaskId != value) {
                    this.model.curTaskId = value;
                }
            }
        }

        public string CurrentName {
            get {
                return this.model.curTargetName;
            }
            set {
                if (this.model.curTargetName != value) {
                    this.model.curTargetName = value;
                }
            }
        }

        public string CurrentLotteryGroup {
            get {
                return this.model.curLotteryGroup;
            }
            set {
                if (this.model.curLotteryGroup != value) {
                    this.model.curLotteryGroup = value;
                }
            }
        }

        public string CurrentBuild {
            get {
                return this.model.curBuild;
            }
            set {
                if (this.model.curBuild != value) {
                    this.model.curBuild = value;
                }
            }
        }

        public string CurrentHero {
            get {
                return this.model.curHero;
            }
            set {
                if (this.model.curHero != value) {
                    this.model.curHero = value;
                }
            }
        }
        /**********************/

        /* Other members */

        /*****************/

        void Awake() {
            this.model = ModelManager.GetModelData<FteModel>();
            this.view = this.gameObject.AddComponent<FteView>();

            FteManager.SetStartCallback(GameConst.NORMAL, 2, (index) => { StartCoroutine(this.OnFteStep2Start()); });
            FteManager.SetStartCallback(GameConst.NORMAL, 11, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 21, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 31, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 71, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 81, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 91, (index) => this.ShowChat(FteChatRole.None));
            //FteManager.SetStartCallback(GameConst.NORMAL, 101, (index) => FteManager.StartFte(GameConst.NORMAL,131));
            FteManager.SetStartCallback(GameConst.NORMAL, 131, (index) => this.ShowChat(FteChatRole.None));
            //FteManager.SetStartCallback(GameConst.NORMAL, 19, (index) => this.ShowChat());
            FteManager.SetStartCallback(GameConst.NORMAL, 151, (index) => this.ShowChat(FteChatRole.None));
            FteManager.SetStartCallback(GameConst.NORMAL, 151, (index) => { FteManager.FteOver = true; });
            FteManager.SetStartCallback(GameConst.NORMAL, 161, (index) => { TriggerManager.Invoke(Trigger.DramaArrow); });
            FteManager.SetStartCallback(GameConst.NORMAL, 121,
                (index) => this.ShowChat(FteChatRole.None, transparent: true));
            FteManager.SetStartCallback(GameConst.NORMAL, 141, (index) => this.ShowChat(FteChatRole.None));
            
            NetHandler.AddNtfHandler(typeof(EventBuildNtf).Name, this.EventBuildNtf);
        }

        public void Show() {
            this.view.Show();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void ShowFteMask() {
            this.view.ShowFteMask();
        }

        public void HideFteMask() {
            this.view.HideFteMask();
        }

        public void EndFte() {
            this.view.EndFte();
        }

        public void ShowComic(int step) {
            this.view.ShowComic(step);
        }

        public void HideComic() {
        }

        public void ShowBanner(UnityAction callback) {
            this.view.ShowBanner(callback);
        }

        public void HideBanner(UnityAction callback, bool immediatly = false) {
            this.view.HideBanner(callback, immediatly);
        }

        private IEnumerator OnFteStep2Start() {
            yield return YieldManager.GetWaitForSeconds(2.5f);
            FteManager.EndFte(true);
        }

        public void ShowChat(FteChatRole imgRole, bool transparent = false, float offset = 0) {
            this.view.SetImageOther(imgRole);
            FteManager.SetChat(offset, transparent: transparent);
        }

        public void SetLeftChat(string text,bool isHigh,UnityAction afterCallBack) {
            this.view.SetLeftChat(text, isHigh,afterCallBack);
        }

        public void SetRightChat(string text, UnityAction afterCallBack) {
            this.view.SetRightChat(text, afterCallBack);
        }

        public void HideRightChat() {
            this.view.HideRightChat();
        }

        public void HideChat(UnityAction callback,bool isLastChat) {
            this.view.HideChat(callback, isLastChat);
        }

        // Temp

        public void ShowBattle() {
            BattleModel battleModel = ModelManager.GetModelData<BattleModel>();
            battleModel.isFte = true;
            ModelManager.LoadScene("Scene3DBattle", true);
        }



        public void SetDragMask(Transform from, Transform to, CustomDrag fromDrag, CustomDrop toDrop) {
            this.view.SetDragMask(from, to, fromDrag, toDrop);
        }


        public void SetMask(Transform nextTrans, Transform prevTrans, bool hasArrow, bool isButton, bool autoNext,
            bool isEnforce, bool isHighlight, Transform arrowParent, Vector2 offset, float rotation,UnityAction afterCallBack) {
            this.view.SetMask(nextTrans, prevTrans, hasArrow, isButton, autoNext,
                isEnforce, isHighlight, arrowParent, offset, rotation,afterCallBack);
        }

        public void RemoveMask() {
            this.view.RemoveMask();
        }

        public void SetArrow(Transform rectTrans, Transform arrowParent,
            bool isEnforce, Vector2 offset, float rotation) {
            FteRect arrowRect = new FteRect(
                MapUtils.WorldToUIPoint(rectTrans.position, this.view.fteCamera),
                rectTrans.GetComponent<RectTransform>().rect
            );
            if (isEnforce) {
                this.view.ShowFteMask();
            } else {
                this.view.HideFteMask();
            }
            this.view.SetArrow(arrowRect, arrowParent ?? rectTrans, offset: offset, rotation: rotation);
        }

        public bool HasArrow() {
            return this.view.HasArrow();
        }

        public void HideArrow() {
            this.view.HideArrow();
        }

        public void SetChat(float offset, string text, int subIndex,
            bool needButton, bool needBackground, int direction, bool transparent) {
            this.view.SetChat(offset, text, subIndex, needButton, needBackground, direction, transparent);
        }
        /* Add 'NetMessageAck' function here*/

        private void EventBuildNtf(IExtensible message) {
            EventBuildNtf eventBuildNtf = message as EventBuildNtf;
            EventBuild eventBuild = eventBuildNtf.EventBuild;
            if (eventBuild.BuildingName.CustomEquals(this.CurrentBuild) &&
                eventBuildNtf.Method.CustomEquals("del")) {
                FteManager.EndFte(true, false);
                this.CurrentBuild = string.Empty;
            }
        }

        /***********************************/
    }
}
