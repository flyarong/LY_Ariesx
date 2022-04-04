using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class MiniMapTileViewModel : BaseViewModel {
        private MiniMapViewModel parent;
        private MiniMapTileView view;

        /* Model data get set */
        public List<Point> TileList {
            get {
                return RoleManager.GetPointList();
            }
        }
        /**********************/

        public Coord CurrentCoord {
            get; set;
        }

        public bool IsFold { get; set; }

        /* Other members */
        public bool IsSelectItem { get; set; }
        private bool NeedRefresh { get; set; }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MiniMapViewModel>();
            this.view = this.gameObject.AddComponent<MiniMapTileView>();

            TriggerManager.Regist(Trigger.RefreshPlayerPoint, () => {
                if (this.view.IsVisible) {
                    this.view.Refresh();
                } else {
                    NeedRefresh = true;
                }
            });

            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            this.parent.resetChoseAction = this.view.ResetItemChosen;
            if (this.NeedRefresh) {
                this.view.ResetItems(this.TileList.Count);
                this.NeedRefresh = false;
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public void SetStatus(bool isFold) {
            this.IsFold = isFold;
        }

        public void BubbleMoveTo(Coord coordinate) {
            this.parent.Coordinate = new Vector2(coordinate.X, coordinate.Y);
            if (this.IsFold) {
                this.parent.Jump();
            }
        }
    }
}
