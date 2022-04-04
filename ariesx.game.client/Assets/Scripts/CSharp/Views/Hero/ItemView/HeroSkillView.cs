using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using Protocol;
using System.Collections.Generic;
using TMPro;

namespace Poukoute {
    public class HeroSkillView : BaseView {
        public class SkillEvent : UnityEvent<RectTransform> { }
        public SkillEvent onBtnDetailClick = new SkillEvent();
        
        //private GameObject ui;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtDesc;
        public Button btnDetail;
        public Transform skillDetail;
        public CanvasGroup skillDetailCG;
        //private bool isShowDetail = false;

        private SkillConf skill;
        public SkillConf SkillConf {
            get {
                return this.skill;
            }
            set {
                if (this.skill != value) {
                    this.skill = value;
                    this.OnSkillChange();
                }
            }
        }

        //private UnityEvent onBtnClick = new UnityEvent();
        public UnityEvent OnBtnDetailsClick {
            get {
                this.btnDetail.onClick.RemoveAllListeners();
                return this.btnDetail.onClick;
            }
        }

        private List<Protocol.Skill> skillList;
        public List<Protocol.Skill> SkillList {
            get {
                return this.skillList;
            }
            set {
                this.skillList = value;
                this.OnSkillListChange();
            }
        }

        //void Awake() {
            //this.btnDetail.onClick.AddListener(this.OnBtnDetailClick);
        //}

        private void OnBtnDetailClick() {
            //if (!isShowDetail) {
            //    AnimationManager.Animate(this.skillDetail.gameObject, "Show");
            //    UIManager.SetUICanvasGroupVisible(this.skillDetailCG, true);
            //    this.isShowDetail = true;
            //} else {
            //    UIManager.SetUICanvasGroupVisible(this.skillDetailCG, false);
            //    AnimationManager.Animate(this.skillDetail.gameObject, "Hide");                
            //    this.isShowDetail = false;
            //}
            //this.onBtnDetailClick.Invoke(this.GetComponent<RectTransform>());
        }

        private void OnSkillChange() {
            this.txtName.text = LocalManager.GetValue(this.skill.name);
        }

        private void OnSkillListChange() {
            this.txtDesc.text = string.Format(
                LocalManager.GetValue(this.skill.description), skillList[0].Args.ToArray());
        }
    }
}
