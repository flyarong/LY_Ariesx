using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class AllianceCreateViewModel : BaseViewModel, IViewModel, IAllianceEdit {
        private AllianceCreateOrJoinViewModel parent;
        private AllianceCreateOrJoinModel model;
        private AllianceCreateView view;

        /* Model data get set */
        public string InputAllianceName {
            get {
                return this.model.inputAllianceName;
            }
            set {
                this.model.inputAllianceName = value;
            }
        }

        public JoinConditionType? JoinCondition {
            get {
                return this.model.joinCondition;
            }
            set {
                this.model.joinCondition = (JoinConditionType)value;
            }
        }

        public string Description {
            get {
                return this.model.description;
            }
            set {
                this.model.description = value;
            }
        }

        public int ConditionForce {
            get {
                return this.model.influnceCondition;
            }
            set {
                this.model.influnceCondition = value;
            }
        }

        public int Language {
            get {
                return this.model.language;
            }
            set {
                this.model.language = value;
            }
        }

        public List<string> ConditionList {
            get {
                return this.model.conditionList;
            }
        }

        public List<string> ConditionLocalList {
            get {
                return this.model.conditionLocalList;
            }
        }

        public List<string> InflunceList {
            get {
                return this.model.influnceList;
            }
        }

        public int InflunceCondition {
            get {
                return this.model.influnceCondition;
            }
            set {
                this.model.influnceCondition = value;
            }
        }

        public int AllianceEmblem {
            get {
                return this.model.allianceEmblem;
            }
            set {
                this.model.allianceEmblem = value;
                if (this.view.IsVisible) {
                    this.view.OnLogoChange();
                }
            }
        }

        public long RejoinAllianceFinishAt {
            get {
                return this.model.rejoinAllianceFinishAt;
            }
        }

        private AllianceLangChooseViewModel allianceLangChooseViewModel;
        public AllianceLangChooseViewModel AllianceLangChooseViewModel {
            get {
                return allianceLangChooseViewModel ??
                    (this.allianceLangChooseViewModel = PoolManager.GetObject<AllianceLangChooseViewModel>(this.transform));
            }

            set {
                allianceLangChooseViewModel = value;
            }
        }
        /*****************/

        /* Other members */
        public bool NeedRefresh {
            get; set;
        }

        /**********************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceCreateOrJoinViewModel>();
            this.model = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.view = this.gameObject.AddComponent<AllianceCreateView>();
            this.NeedRefresh = true;
            this.Hide();
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.view.SetInfo();
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void ShowLogoView() {
            this.parent.ShowSubWindowByType(AllianceSubWindowType.Logo);
        }

        public void CreateAlliance() {
            if (this.InputAllianceName.Length < 2 || this.InputAllianceName.Length > 20|| 
                this.InputAllianceName.CustomStartsWith(" ")) {
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.create_name_illegal)
                   , btnInfoLabel:LocalManager.GetValue(LocalHashConst.button_confirm));
                return;
            }
            CreateAllianceReq createAlliance = new CreateAllianceReq() {
                Name = this.InputAllianceName,
                Desc = this.Description,
                Emblem = this.AllianceEmblem,
                JoinType = (int)this.JoinCondition.Value,
                ForceLimit = this.ConditionForce,
                Language = this.Language
            };
            NetManager.SendMessage(createAlliance, string.Empty, null);
        }


        public void SetGameLanguage(AllianceLanguageConf language) {
            this.view.SetGameLanguage(language);
            this.model.language = int.Parse(language.id);
        }
    }
}
