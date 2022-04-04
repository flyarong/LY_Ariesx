using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class AllianceEditViewModel : BaseViewModel, IViewModel, IAllianceEdit {
        private AllianceSubWindowsViewModel parent;
        private AllianceCreateOrJoinModel allianceCreateOrJoinModel;
        private AllianceEditView view;

        /* Model data get set */
        public Alliance Alliance {
            get {
                return RoleManager.GetAlliance();
            }
        }

        public List<string> ConditionList {
            get {
                return this.allianceCreateOrJoinModel.conditionList;
            }
        }

        public List<string> ConditionLocalList {
            get {
                return this.allianceCreateOrJoinModel.conditionLocalList;
            }
        }

        public List<string> InflunceList {
            get {
                return this.allianceCreateOrJoinModel.influnceList;
            }
        }

        public JoinConditionType? JoinCondition {
            get {
                return this.allianceCreateOrJoinModel.joinCondition;
            }
            set {
                this.allianceCreateOrJoinModel.joinCondition = (JoinConditionType)value;
            }
        }

        public int InflunceCondition {
            get {
                return this.allianceCreateOrJoinModel.influnceCondition;
            }
            set {
                this.allianceCreateOrJoinModel.influnceCondition = value;
            }
        }

        public string Description {
            get {
                return this.allianceCreateOrJoinModel.description;
            }
            set {
                if (this.allianceCreateOrJoinModel.description != value) {
                    this.allianceCreateOrJoinModel.description = value;
                }
            }
        }

        public int AllianceEmblem {
            get {
                return this.allianceCreateOrJoinModel.allianceEmblem;
            }
            set {
                this.view.OnLogoChange();
            }
        }

        public int Language {
            get {
                return this.allianceCreateOrJoinModel.language;
            }
            set {
                if(this.allianceCreateOrJoinModel.language != value) {
                    this.allianceCreateOrJoinModel.language = value;
                }
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
        /**********************/

        /* Other members */
        public bool NeedReFresh {
            get; set;
        }

        /*****************/

        private void Awake() {
            this.allianceCreateOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.parent = this.transform.parent.GetComponent<AllianceSubWindowsViewModel>();
            this.view = this.gameObject.AddComponent<AllianceEditView>();
            this.NeedReFresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedReFresh) {
                this.AllianceSettingDetail();
                this.view.SetInfo();
            }
        }


        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }
        
        private void AllianceSettingDetail() {
            this.JoinCondition = (JoinConditionType)this.Alliance.JoinCondition.Type;
            this.Description = this.Alliance.Description;
            this.InflunceCondition = this.Alliance.JoinCondition.ForceLimit;
        }

        public void HideWindow() {
            this.parent.Hide();
        }

        public void ShowLogoView() {
            this.parent.ShowSubWindowBlow(AllianceSubWindowType.Logo,
                AllianceSubWindowType.Setting);
        }

        public void SetAllianceReq() {
            //Debug.LogError("SetAllianceReq " + this.AllianceEmblem + " " +
            //    this.JoinCondition + " " + this.InflunceCondition);
            SetAllianceReq setAllianceReq = new SetAllianceReq() {
                Desc = this.Description,
                Emblem = this.AllianceEmblem,
                JoinType = (int)this.JoinCondition,
                ForceLimit = this.InflunceCondition,
                Language = this.Language
            };
            NetManager.SendMessage(setAllianceReq,
                typeof(SetAllianceAck).Name,
                this.SetAllianceAck
            );
        }

        private void SetAllianceAck(IExtensible message) {
            //SetAllianceAck setAllianceAck = message as SetAllianceAck;
            this.parent.ShowAllianeDetailByType(AllianceViewType.Alliance);
        }


        public void SetAllianceLanguage(AllianceLanguageConf language) {
            this.view.SetAllianceLanguage(language);
            this.Language = int.Parse(language.id);
        }
    }
}
