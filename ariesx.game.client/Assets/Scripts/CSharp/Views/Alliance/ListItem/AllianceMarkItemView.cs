using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceMarkItemView : BaseItemViewsHolder {
        #region ui element
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private TextMeshProUGUI txtCoordinate;
        [SerializeField]
        private Button btnDelete;
        [SerializeField]
        private Transform pnlNotice;
        [SerializeField]
        private Transform pnlChoosen;
        [SerializeField]
        private Button btnItemInfo;
        //[SerializeField]
        //private Image imgIcon;
        #endregion

        public MapMark mark = new MapMark();
        public Vector2 coordinate;
        private UnityAction OnInfoClick = null;
        [HideInInspector]
        public int Index = -1;

        private void Start() {
            this.btnItemInfo.onClick.RemoveAllListeners();
            this.btnItemInfo.onClick.AddListener(() => {
                this.OnInfoClick.InvokeSafe();
                this.FocusOnItemView(true);
            });
        }

        public void SetMark(MapMark mapMark,
            UnityAction deleteCallback, UnityAction callback) {
            if (this.mark.mark != mapMark.mark) {
                this.mark = mapMark;
                this.coordinate = new Vector2(this.mark.mark.Coord.X, this.mark.mark.Coord.Y);
                this.OnMarkChange();
                this.btnDelete.onClick.RemoveAllListeners();
                this.btnDelete.onClick.AddListener(deleteCallback);
                this.OnInfoClick = callback;
                this.FocusOnItemView(false);
            }
        }

        public void FocusOnItemView(bool isChoosen) {
            this.pnlChoosen.gameObject.SetActiveSafe(isChoosen);
            if (isChoosen && this.mark.isNew) {
                this.mark.isNew = false;
                this.pnlNotice.gameObject.SetActiveSafe(false);
            }
        }

        private void OnMarkChange() {
            bool showDelete = false;
            this.pnlNotice.gameObject.SetActiveSafe(this.mark.isNew);
            if (this.mark.type == MapMarkType.Alliance) {
                showDelete = ((int)RoleManager.GetAllianceRole() > (int)AllianceRole.Elder);
                //this.imgIcon.sprite =
                //    ArtPrefabConf.GetSprite(SpritePath.markIconPrefix, "alliance");
            } else {
                if (this.mark.type == MapMarkType.Others) {
                    showDelete = true;
                }
                //this.imgIcon.sprite =
                //    ArtPrefabConf.GetSprite(SpritePath.markIconPrefix, "other");
            }
            this.btnDelete.gameObject.SetActiveSafe(showDelete);

            this.txtName.text = this.mark.mark.Name;
            this.txtCoordinate.text = string.Concat("(",
                this.mark.mark.Coord.X, ",", this.mark.mark.Coord.Y, ")");
        }
    }
}
