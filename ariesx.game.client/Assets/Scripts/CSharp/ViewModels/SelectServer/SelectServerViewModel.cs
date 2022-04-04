using Protocol;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Poukoute {
    public class SelectServerViewModel : BaseViewModel, IViewModel {
        private SelectServerView view;
        private MapViewModel parent;
        private LoginModel loginModel;
        private int worldcount = 0;
        public List<object> roleList = new List<object>();

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<SelectServerView>();
            this.loginModel = ModelManager.GetModelData<LoginModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.PlayShow(() => {
                    this.parent.OnAddViewAboveMap(this);
                },needHideBack:true);
                this.view.afterShowCallback += () => {
                    this.view.ResetToggle();
                    this.PageWorldReq(0);
                    this.MyWorldReq();
                };
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide(
                    this.OnHideCallback
                   );
            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        private void OnHideCallback() {
            this.parent.OnRemoveViewAboveMap(this);
        }

        public void GetSelfServer(ref List<object> roleList
            , ref List<object> worldList) {
            roleList = this.loginModel.roleList;
            worldList = this.loginModel.allWorldList;
        }

        public void MyWorldReq() {
            this.view.ClearDetailList();
            string url = string.Concat(VersionConst.url, "api/client/worlds/my_worlds");
            StartCoroutine(NetManager.SendHttpMessage(url, this.MyWorldAck,
               new string[] { "client_agent", VersionConst.netGate.ToString(),
                   "client_locale", VersionConst.language,
                   "login_token",RoleManager.LoginToken}));
        }

        public void MyWorldAck(WWW www) {
            if (www.error != null) {
                Debug.LogError("MyWorldAck");
                return;
            }
            Dictionary<string, object> dict =
                 (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
            List<object> worldList = (List<System.Object>)dict["worlds"];
            List<object> roleList = (List<System.Object>)dict["roles"];
            this.roleList = roleList;
            if (this.view.togglePage == 0) {
                this.view.ClearDetailList();
                this.view.SetSelfServerItem(roleList, worldList);
            }
        }

        public void PageWorldReq(int page) {
            if (page != 0) {
                this.view.ClearDetailList();
            } 
            string url = string.Concat(VersionConst.url, "api/client/worlds/page_world");
            StartCoroutine(NetManager.SendHttpMessage(url, this.PageWorldAck,
               new string[] { "client_agent", VersionConst.netGate.ToString(),
                   "client_locale", VersionConst.language, "page",page.ToString()}));
        }

        public void PageWorldAck(WWW www) {
            if (www.error != null) {
                Debug.LogError("PageWorldAck");
                return;
            }
            Dictionary<string, object> dict =
                 (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
            List<object> worldList = (List<System.Object>)dict["worlds"];
            int worldsCount = int.Parse(((System.Object)dict["worlds_count"]).ToString());
            if (worldList.Count == 0) {
                if (this.worldcount != worldsCount) {
                    this.view.ClearGroupList();
                    this.view.SetToggleGroup(worldsCount);
                }
            } else {
                if (this.view.togglePage ==
                    int.Parse(((System.Object)
                        ((Dictionary<string, object>)(worldList[0]))
                            ["world_number"]).ToString()) / 10 + 1) {
                    this.view.ClearDetailList();
                    this.view.SetNormalServerItem(worldList);
                }
            }
        }

        public void RecommendWorldReq() {
            this.view.ClearDetailList();
            string url = string.Concat(VersionConst.url, "api/client/worlds/recommend_world");
            StartCoroutine(NetManager.SendHttpMessage(url, this.RecommendWorldAck,
               new string[] { "client_agent", VersionConst.netGate.ToString(),
                   "client_locale", VersionConst.language}));
        }

        public void RecommendWorldAck(WWW www) {
            if (www.error != null) {
                Debug.LogError("RecommendWorldAck");
                return;
            }
            List<object> worldList =
                 (List<object>)Facebook.MiniJSON.Json.Deserialize(www.text);
            //List<object> worldList = (List<System.Object>)dict["worlds"];
            Dictionary<string, object> world = null;
            foreach (Dictionary<string, object> child in worldList) {
                if (((bool)child["fit_login"])) {
                    world = child;
                    if (this.view.togglePage == -1) {
                        this.view.ClearDetailList();
                        this.view.SetNormalServerItem(world);
                    }
                }
            }
        }
    }
}
