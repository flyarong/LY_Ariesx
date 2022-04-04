using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public enum NoticeType {
        BuildLevelUp = 0,
        //HeroLevelUp = 1,
        HeroRecruitDone = 1
    };

    public class MapRightHUDViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        //private MapModel model;
        private HeroModel heroModel;
        private BuildModel buildModel;
        private MapRightHUDView view;
        /* Model data get set */
        public List<string> noticesItemName = new List<string>();
        private const int NOTICE_ITEM_COUNT = 3;
        /**********************/

        /* Other members */
        public Hero CurrentHero {
            get; set;
        }

        public ElementBuilding CurrentBuilding {
            get; set;
        }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            //this.model = ModelManager.GetModelData<MapModel>();
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.view = this.gameObject.AddComponent<MapRightHUDView>();

            FteManager.SetEndCallback(GameConst.NORMAL, 7, this.OnFteStep7End);
        }

        public void ShowNoticesItemInCache() {
            //Debug.LogError("ShowNoticesItemInCache");
            this.view.Show();
            if ((this.noticesItemName.Count > 0) &&
                (this.view.GetNoticeItemCount() < NOTICE_ITEM_COUNT)) {
                string[] noticeInfo = this.noticesItemName[0].CustomSplit(","[0]);
#if UNITY_EDITOR
                if (noticeInfo.Length < 2) {
                    Debug.LogError("noticeInfo must contain name and notice type" + noticeInfo.Length);
                    return;
                }
#endif

                string noticeName = noticeInfo[0];
                NoticeType noticeType = (NoticeType)(int.Parse(noticeInfo[1]));
                AudioManager.Play(AudioPath.showPrefix + "tip_warning",
                    AudioType.Show, AudioVolumn.High);
                switch (noticeType) {
                    case NoticeType.BuildLevelUp:
                        this.CurrentBuilding = this.buildModel.buildingDict[noticeName];
                        this.view.ShowBuildingLevelUp();
                        SetQueueIsFoldByBuildName(noticeName);
                        break;
                    //case NoticeType.HeroLevelUp:
                    //    this.CurrentHero = this.heroModel.heroDict[noticeName];
                    //    this.view.ShowMapNotice(true);
                    //    break;
                    case NoticeType.HeroRecruitDone:
                        this.CurrentHero = this.heroModel.heroDict[noticeName];
                        this.view.ShowMapRecruitDoneNotice(false);
                        break;
                    default:
                        Debug.LogError("NOTICE!!could not reach herer!");
                        break;
                }

                this.noticesItemName.RemoveAt(0);
            }
        }

        private void SetQueueIsFoldByBuildName(string noticeName) {
            if (noticeName.Contains("armycamp"))
                this.parent.SetQueueIsFold(false);
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

        public void ShowHero(string heroName) {
            this.parent.ShowHeroInfo(heroName);
        }

        public void MoveToBuilding(string buildingName) {
            Vector2 coordinate = this.buildModel.GetBuildCoordinateByName(buildingName);
            this.parent.MoveWithClick(coordinate);
        }

        public void BuildingLevelUpHandler(string buildingName) {
            this.noticesItemName.Add(
                string.Format(
                    GameConst.TWO_PART_WITH_COMMA, buildingName, (int)NoticeType.BuildLevelUp));
            this.ShowNoticesItemInCache();
        }

        public void HeroRecruitDoneHandler(string troopName) {
            this.noticesItemName.Add(
                string.Format(
                    GameConst.TWO_PART_WITH_COMMA, troopName, (int)NoticeType.HeroRecruitDone));
            this.ShowNoticesItemInCache();
        }
        /***********************************/

        #region FTE

        private void OnFteStep7End() {
            this.view.OnFteStep7End();
        }

        #endregion
    }
}
