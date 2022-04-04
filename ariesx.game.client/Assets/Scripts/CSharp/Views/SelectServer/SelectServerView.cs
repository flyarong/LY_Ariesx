using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Poukoute {
    public class SelectServerView : BaseView {
        private SelectServerViewModel viewModel;
        private SelectServerViewPreference viewPref;
        private Dictionary<string, GameObject> selfServerDict
            = new Dictionary<string, GameObject>();
        public int togglePage = 0;


        void Awake() {
            this.viewModel = this.GetComponent<SelectServerViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UISelectServer");
            this.viewPref = this.ui.transform.GetComponent<SelectServerViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.background.onClick.AddListener(this.OnBtnCloseClick);
        }

        public void ClearDetailList() {
            GameHelper.ClearChildren(this.viewPref.PnlDetailList);
        }

        public void ClearGroupList() {
            GameHelper.ClearChildren(this.viewPref.PnlGroupList);
        }

        public void ResetToggle() {
            this.viewPref.toggleGroup.SetAllTogglesOff();
            this.togglePage = 0;
        }

        public void SetSelfServerItem(List<object> roleListOld, List<object> worldList) {
            this.selfServerDict.Clear();
            List<object> roleList = (from temp in roleListOld
                                     orderby
            int.Parse(((System.Object)((Dictionary<string, object>)temp)
            ["force"]).ToString())
                                     select temp).ToList<object>();
            foreach (Dictionary<string, object> role in roleList) {
                foreach (Dictionary<string, object> world in worldList) {
                    string worldId = (string)world["world_id"];
                    string worldNum = ((System.Object)world["world_number"]).ToString();
                    if (worldId.Equals((string)role["world_id"])) {

                        GameObject itemObj =
                            PoolManager.GetObject(PrefabPath.pnlServerItem,
                                this.viewPref.PnlDetailList);
                        string name = (string)world["name"];
                        long time = (long)world["open_date"];
                        bool maintenance = (bool)world["maintenance"];
                        int serviceState = int.Parse((world["world_state"]).ToString());
                        serviceState = maintenance ? 2 : serviceState;
                        string force = ((System.Object)role["force"]).ToString();
                        string level = ((System.Object)role["level"]).ToString();

                        ServerItemView itemView = itemObj.GetComponent<ServerItemView>();
                        itemView.SetItemView(worldId,
                            name, worldNum, time,
                            serviceState, true, force, level);
                        this.selfServerDict.Add(worldId, itemObj);

                        if (worldId.Equals(RoleManager.WorldId)) {
                            this.viewPref.txtName.text = name;
                            this.viewPref.txtID.text = string.Concat(worldNum, LocalManager.GetValue(
                            LocalHashConst.change_server_server)); //worldNum.PadLeft(5,'0');
                            //this.viewPref.Time.text = GameHelper.DateFormat(time);
                            string path = string.Empty;
                            switch (serviceState) {
                                case 0:
                                    path = "point_busy";
                                    break;
                                case 1:
                                    path = "point_smooth";
                                    break;
                                case 2:
                                    path = "point_maintenance";
                                    break;
                            }
                            this.viewPref.imgSmallLight.sprite
                                = ArtPrefabConf.GetSprite(path);
                            if (!force.Equals(string.Empty)) {
                                this.viewPref.force.text = GameHelper.GetFormatNum(long.Parse(force), maxLength: 3, decLength: 2);
                            }
                            this.viewPref.level.text =
                                string.Format(LocalManager.GetValue(LocalHashConst.level), level);
                            this.viewPref.pnlMyServer.gameObject.SetActiveSafe(true);
                        }
                    }
                }
            }
            foreach (var item in this.selfServerDict) {
                if (item.Key.Equals(RoleManager.WorldId)) {
                    item.Value.transform.SetSiblingIndex(0);
                    item.Value.transform.
                        GetComponent<ServerItemView>().SetNowServer();
                }
            }
        }

        public void SetNormalServerItem(List<object> worldListOld) {
            List<object> roleList = new List<object>();
            List<object> useless = new List<object>();
            this.viewModel.GetSelfServer(ref roleList, ref useless);
            List<object> worldList = (from temp in worldListOld
                                      orderby
                                        int.Parse(((System.Object)((Dictionary<string, object>)temp)
                                     ["world_number"]).ToString())
                                      select temp).ToList<object>();
            foreach (Dictionary<string, object> world in worldList) {
                bool canShow = true;
                string worldId = (string)world["world_id"];
                string worldNum = ((System.Object)world["world_number"]).ToString();
                string name = (string)world["name"];
                long time = (long)world["open_date"];
                bool maintenance = (bool)world["maintenance"];
                int serviceState = int.Parse((world["world_state"]).ToString());
                serviceState = maintenance ? 2 : serviceState;
                foreach (Dictionary<string, object> role in roleList) {
                    if (worldId.Equals((string)role["world_id"])) {
                        canShow = false;
                    }
                }
                if (canShow) {
                    GameObject itemObj =
                        PoolManager.GetObject(PrefabPath.pnlServerItem,
                            this.viewPref.PnlDetailList);
                    ServerItemView itemView = itemObj.GetComponent<ServerItemView>();
                    itemView.SetItemView(worldId, name,
                        worldNum, time, serviceState, false, string.Empty, string.Empty);
                }
            }
        }

        public void SetNormalServerItem(Dictionary<string, object> world) {
            string worldNum = ((System.Object)world["world_number"]).ToString();
            GameObject itemObj =
                    PoolManager.GetObject(PrefabPath.pnlServerItem,
                        this.viewPref.PnlDetailList);
            string worldId = (string)world["world_id"];
            string name = (string)world["name"];
            long time = (long)world["open_date"];
            bool maintenance = (bool)world["maintenance"];
            int serviceState = int.Parse((world["world_state"]).ToString());
            serviceState = maintenance ? 2 : serviceState;
            ServerItemView itemView = itemObj.GetComponent<ServerItemView>();
            string level = string.Empty;
            string force = string.Empty;
            List<object> roleList = new List<object>();
            List<object> useless = new List<object>();
            this.viewModel.GetSelfServer(ref roleList, ref useless);
            bool canShow = false;
            foreach (Dictionary<string, object> role in roleList) {
                if (worldId.Equals((string)role["world_id"])) {
                    canShow = true;
                    force = ((System.Object)role["force"]).ToString();
                    level = ((System.Object)role["level"]).ToString();
                }
            }
            itemView.SetItemView(worldId, name,
                worldNum, time, serviceState, canShow, force, level, isNew: true);
            if (worldId.Equals(RoleManager.WorldId)) {
                Debug.Log("SetNowServer");
                itemView.SetNowServer();
            }
        }

        public void SetToggleGroup(int worldsCount) {
            GameObject itemObj = PoolManager.GetObject(
                PrefabPath.myToggle,
                this.viewPref.PnlGroupList
            );
            itemObj.GetComponent<Toggle>().group = this.viewPref.toggleGroup;
            ServerToggleItemView itemView = itemObj.GetComponent<ServerToggleItemView>();
            itemView.SetItemView("0");
            itemView.toogleAction = OnToggleChange;
            itemObj = PoolManager.GetObject(
                PrefabPath.toggle,
                this.viewPref.PnlGroupList
            );
            itemObj.GetComponent<Toggle>().group = this.viewPref.toggleGroup;
            itemView = itemObj.GetComponent<ServerToggleItemView>();
            itemView.SetItemView(
                LocalManager.GetValue(
                    LocalHashConst.change_server_suggest
                )
            );
            itemView.toogleAction = OnToggleChange;
            for (int i = 0; i < worldsCount / 10; i++) {
                itemObj = PoolManager.GetObject(
                    PrefabPath.toggle,
                    this.viewPref.PnlGroupList
                );
                itemObj.GetComponent<Toggle>().group = this.viewPref.toggleGroup;
                itemView = itemObj.GetComponent<ServerToggleItemView>();
                itemView.SetItemView(string.Concat(
                    (i * 10 + 1).ToString(), "-", (i * 10 + 10).ToString(),
                    LocalManager.GetValue(LocalHashConst.change_server_server)
                ));
                itemView.toogleAction = OnToggleChange;
            }
            if (worldsCount % 10 > 0) {
                itemObj =
               PoolManager.GetObject(PrefabPath.toggle, this.viewPref.PnlGroupList);
                itemObj.GetComponent<Toggle>().group = this.viewPref.toggleGroup;
                itemView = itemObj.GetComponent<ServerToggleItemView>();
                itemView.SetItemView(string.Concat(
                    (worldsCount / 10 * 10 + 1).ToString(), "-", (worldsCount).ToString(),
                     LocalManager.GetValue(LocalHashConst.change_server_server)
                ));
                itemView.toogleAction = OnToggleChange;
            }
        }

        private void OnToggleChange(int value) {
            if (value == 0) {
                this.viewModel.MyWorldReq();
                this.togglePage = 0;
            } else if (value == -1) {
                this.viewModel.RecommendWorldReq();
                this.togglePage = -1;
            } else {
                this.viewModel.PageWorldReq(value / 10 + 1);
                this.togglePage = value / 10 + 1;
            }
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
