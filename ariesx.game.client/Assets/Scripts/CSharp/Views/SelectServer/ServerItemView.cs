using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;
namespace Poukoute {
    public class ServerItemView : MonoBehaviour {
        public CustomButton btnServer;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtID;
        //public TextMeshProUGUI txtTime;
        public Image imgSmallLight;
        public Transform pnlIntroduction;
        public TextMeshProUGUI txtForce;
        public TextMeshProUGUI txtLevel;
        public GameObject pnlNew;
        public GameObject imgNow;
        private string worldId;
        private bool isMaintenance = false;
        private bool isSelf = false;

        private void Awake() {
            this.btnServer.onClick.AddListener(OnServerClick);
        }

        public void SetItemView(string worldId,string name, string ID,
            long time,int servionState,
                bool isSelf,string force,string level,bool isNew = false) {
            this.btnServer.interactable = enabled;
            this.worldId = worldId;
            this.txtName.text = name;
            this.txtID.text =string.Concat(ID,LocalManager.GetValue(
                LocalHashConst.change_server_server));/*.PadLeft(5,'0');*/
            //this.txtTime.text = LocalManager.GetValue(
            //    LocalHashConst.change_server_start_time) + 
            //    GameHelper.DateFormat(time,format: "yyyy-MM-dd");
            string path = string.Empty;
            switch (servionState) {
                case 0:
                    path = "point_busy";
                    isMaintenance = false;
                    break;
                case 1:
                    path = "point_smooth";
                    isMaintenance = false;
                    break;
                case 2:
                    path = "point_maintenance";
                    isMaintenance = true;
                    break;
            }
            this.imgSmallLight.sprite = ArtPrefabConf.GetSprite(path);
            this.pnlIntroduction.gameObject.SetActiveSafe(isSelf);
            this.isSelf = isSelf;
            if (!force.Equals(string.Empty)) {
                txtForce.text = GameHelper.GetFormatNum(long.Parse(force), maxLength: 3, decLength: 2);
            }
            txtLevel.text = string.Format( LocalManager.GetValue(LocalHashConst.level),level);
            this.pnlNew.SetActiveSafe(isNew);
            this.imgNow.SetActiveSafe(false);
        }

        public void SetNowServer() {
            this.imgNow.SetActiveSafe(true);
        }

        private void OnServerClick() {
            if (!imgNow.activeSelf && !this.isMaintenance) {
                string tip = this.isSelf ? LocalManager.GetValue(
                    LocalHashConst.change_server_exist_role_confirm) :
                    string.Format(LocalManager.GetValue
                    (LocalHashConst.change_server_confirm_desc), txtName.text);
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.button_confirm),
                   tip,onYes: () => { this.ChooseWorldReq(); }, onNo: () => { });
            }
        }

        private void ChooseWorldReq() {
            string url = string.Concat(VersionConst.url, "api/client/worlds/choose_world");
            StartCoroutine(NetManager.SendHttpMessage(url, this.ChooseWorldAck,
               new string[] { "login_token", RoleManager.LoginToken,
                   "world_id", this.worldId}));
        }

        private void ChooseWorldAck(WWW www) {
            if (www.error != null) {
                Debug.LogError("ChooseWorldAck");
                return;
            }
            GameManager.RestartGame();
        }
    }
}
