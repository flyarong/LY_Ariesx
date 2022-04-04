using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Protocol;

namespace Poukoute {

    public class LoginView : BaseView {
        LoginViewModel viewModel;
        // Choose server.
        Transform pnlWorld;
        Transform pnlReturnLogin;

        void Awake() {
            this.viewModel = this.GetComponent<LoginViewModel>();
            this.ui = UIManager.GetUI("UILogin");
            this.pnlWorld = this.ui.transform.Find("PnlWorld");
            this.ui.transform.Find("TxtVersion").GetComponent<TextMeshProUGUI>().text = "Version " +
                Application.version;
            this.pnlReturnLogin = this.ui.transform.Find("PnlReturnLogin");
            this.pnlReturnLogin.GetComponent<Button>().onClick.AddListener(OnBtnReturnLoginClick);
        }

        public void SetReturnLoginView() {
            this.pnlReturnLogin.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(this.pnlReturnLogin.gameObject, "Flash");
        }

        public void OnWorldChange() {
            //GameHelper.ClearChildren(this.pnlWorld);
            int worldsCount = this.viewModel.LoginWorlds.Count;
            bool hasGameWorld = (worldsCount > 0);
            this.pnlWorld.gameObject.SetActiveSafe(hasGameWorld);
            if (hasGameWorld) {
                GameHelper.ResizeChildreCount(this.pnlWorld,
                    worldsCount, PrefabPath.btnWorld);
                int index = 0;
                foreach (LoginWorld world in this.viewModel.LoginWorlds) {
                    Transform worldView = this.pnlWorld.GetChild(index++);
                    Button worldButton = worldView.GetComponent<Button>();
                    worldView.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                        world.name;// + ": " + world.world_id;
                    if (world.agent_ips.Length > 0) {
                        string ip = world.agent_ips[0];
                        string worldId = world.world_id;
                        worldButton.onClick.AddListener(() => {
                            this.OnBtnWorldClick(worldId, ip);
                        });
                    } else {
                        worldButton.onClick.AddListener(() => {
                            UIManager.ShowAlert(
                                LocalManager.GetValue(LocalHashConst.server_under_maintain));
                        });
                    }

                }
            }
        }

        private void OnBtnWorldClick(string worldId, string ip) {
            this.viewModel.OnBtnWorldClick(worldId, ip);
            this.pnlWorld.gameObject.SetActiveSafe(false);
        }

        private void OnBtnReturnLoginClick() {
            this.pnlReturnLogin.gameObject.SetActiveSafe(false);
            this.viewModel.ReturnLogin();
        }

        void OnEnable() {
            UIManager.ShowUI(this.ui);
        }
        
        void OnDisable() {
            GameObject.Destroy(this.ui);
        }
    }
}
