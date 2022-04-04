using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LyStatistic;

namespace Poukoute {
    public class LyDataSdk : MonoBehaviour {
        private string roleId = string.Empty;

        void Awake() {
            TriggerManager.Regist(Trigger.Login, this.Login);
            TriggerManager.Regist(Trigger.Logout, this.LogOut);
        }

        void Init() {
            int serverType = 0;
            switch (VersionConst.version) {
                case "googleplay":
                    serverType = Manager.TYPE_RELEASE;
                    break;
                case "taptap":
                    serverType = Manager.TYPE_RELEASE;
                    break;
                default:
                    serverType = Manager.TYPE_DEBUG;
                    break;
            }
            LYGameData.initSDK(
                "bc4455c1f7a9c0f7",
                VersionConst.channel,
                Application.version,
                serverType
            );
            Manager.Me.GameAera = "1";
            Manager.Me.Account = RoleManager.GetRoleId();
        }

        void Login() {
            if (LYGameData.Instance == null) {
                GameObject lyGame = new GameObject();
                lyGame.AddComponent<LYGameData>();
                this.Init();
            }
            this.roleId = RoleManager.GetRoleId();
            Manager.Me.on_login(roleId);
        }

        void LogOut() {
            try { 
                Manager.Me.on_exit(roleId);
            } catch {
                ;
            }
        }

        void OnDestroy() {
            this.LogOut();
        }
    }
}
