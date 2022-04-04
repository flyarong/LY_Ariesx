using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using LitJson;
using System;
using System.IO;
using System.Text;

namespace Poukoute {
    public class BattleSimulationViewModel : MonoBehaviour {
        private BattleSimulationModel model;
        private BattleSimulationView view;

        BattleSimulationReportViewModel reportModel;
        BattleSimulationReportDetailViewModel detailModel;
        BattleSimulationStatViewModel statModel;

        public struct HeroData {
            public string name;
            public int level;
            public float army_amount_percent;
            public int position;
        }
        public int simulationCount = 10;
        public HeroData[] defHeroData = new HeroData[6];
        public HeroData[] attHeroData = new HeroData[6];
        public bool[] defNeedHero = new bool[6];
        public bool[] attNeedHero = new bool[6];

        public List<string> HeroList {
            get {
                return this.model.heroList;
            }
        }

        private bool needRefres = false;
        public bool NeedRefresh {
            get {
                return needRefres;
            }
            set {
                needRefres = value;
                this.reportModel.NeedRefresh = value;
                this.detailModel.NeedRefresh = value;
                this.statModel.NeedRefresh = value;
            }
        }
        void Awake() {
            this.InitConfigure();

            this.model = BattleSimulationModel.Instance;
            this.model.Refresh(null);
            this.view = this.gameObject.AddComponent<BattleSimulationView>();

            this.detailModel = this.gameObject.AddComponent<BattleSimulationReportDetailViewModel>();
            this.reportModel = this.gameObject.AddComponent<BattleSimulationReportViewModel>();
            this.statModel = this.gameObject.AddComponent<BattleSimulationStatViewModel>();
        }

        #region Configure
        void InitConfigure() {
#if UNITY_EDITOR

            GameObject configureManager = new GameObject();
            configureManager.name = "ConfigureManager";
            configureManager.transform.position = UnityEngine.Vector3.zero;
            configureManager.transform.SetParent(this.transform);
            configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadBattleEditorConfigures();

            configureManager.AddComponent<PoolManager>();
            configureManager.AddComponent<UpdateManager>();
            configureManager.AddComponent<AudioManager>();
            configureManager.AddComponent<AnimationManager>();
            configureManager.AddComponent<RoleManager>();
            configureManager.AddComponent<ModelManager>();
            configureManager.AddComponent<LocalManager>();
#endif
        }

        public void ShowSimulationUI(bool isShow) {
            this.view.ShowSimulationUI(isShow);
        }
        #endregion

        void OnDisable() {

        }

        public void StartBattleSimulation() {
            this.NeedRefresh = true;
            StartCoroutine(this.Post());
        }

        public void ShowBattleReport() {
            this.reportModel.Show();
        }

        public void ShowStatReport() {
            this.statModel.Show();
        }

        public void ShowDetailReport() {
            //this.detailModel.Show();
        }

        private string url = "http://localhost:8000/battle";

        #region Post
        IEnumerator Post() {
            string attJd, defJd;
            HeroDataToJsonData(out attJd, out defJd);
            this.view.ShowMask(true, "正在请求模拟战斗。。。");
            WWWForm form = new WWWForm();
            form.AddField("times", this.simulationCount);
            form.AddField("attacker_troop", attJd);
            form.AddField("defender_troop", defJd);
            WWW w = new WWW(url, form);
            yield return w;
            if (w.error != null) Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "错误:" + w.error));
            else Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "OK 长度:" + w.bytes.Length));

            SetPostData(w);
            yield return null;
            this.view.ShowMask(false);
        }

        private void HeroDataToJsonData(out string attJd, out string defJd) {
            JsonData att = CreateHeorJsonData(this.attHeroData,this.attNeedHero);
            //att["heroes"] = JsonMapper.ToObject(JsonMapper.ToJson(SetNeedHeroData(this.attHeroData, this.attNeedHero)));

            JsonData def = CreateHeorJsonData(this.defHeroData, this.defNeedHero);
            //def["heroes"] = JsonMapper.ToObject(JsonMapper.ToJson(SetNeedHeroData(this.defHeroData, this.defNeedHero)));
            attJd = JsonMapper.ToJson(att);
            defJd = JsonMapper.ToJson(def);
        }

        private JsonData CreateHeorJsonData(HeroData[] herodata,bool[] needHero) {
            JsonData heroData = new JsonData();
            heroData["heroes"] = new JsonData();
            List<HeroData> heroDataList = SetNeedHeroData(herodata, needHero);
            foreach (var item in heroDataList) {
                JsonData jd = new JsonData();
                jd["name"] = item.name;
                jd["level"] = item.level;
                jd["army_amount_percent"] = item.army_amount_percent;
                jd["position"] = item.position;
                heroData["heroes"].Add(jd);
            }
            return heroData;
        }

        private List<HeroData> SetNeedHeroData(HeroData[] OriginalHD, bool[] needHero) {
            List<HeroData> heroData = new List<HeroData>();
            for (int i = 0; i < OriginalHD.Length; i++) {
                if (needHero[i]) heroData.Add(OriginalHD[i]);
            }
            return heroData;
        }

        private void SetPostData(WWW w) {
            this.model.battleReportRounds.Clear();
            this.model.battleReportList.Clear();
            this.model.battleReportDict.Clear();

            BattleReports br = DeSerialize<BattleReports>(w.bytes);
            for (int i = 0; i < br.Rounds.Count; i++) {
                this.model.battleReportRounds.Add(i, DeSerialize<Battle.ReportRounds>(br.Rounds[i]));
            }
            for (int i = 0; i < br.Summaries.Count; i++) {
                Battle.Report brq = DeSerialize<Battle.Report>(br.Summaries[i]);
                this.model.battleReportList.Add(CreateBattleReport(brq, i.ToString()));
                this.model.battleReportDict.Add(i.ToString(), CreateBattleReport(brq, i.ToString()));
            }
        }

        public static T DeSerialize<T>(byte[] content) {
            using (MemoryStream ms = new MemoryStream(content)) {
                T t = Serializer.Deserialize<T>(ms);
                return t;
            }
        }

        public BattleReport CreateBattleReport(Battle.Report br, string id) {
            return new BattleReport {
                ChestName = "",
                HasAlliance = false,
                Currency = new Currency() { Gem = 10, Gold = 10 },
                Id = id,
                IsRead = false,
                Report = br,
                Resources = new Protocol.Resources() { Crystal = 10, Food = 10, Lumber = 10, Marble = 10, Steel = 10 },
                Timestamp = 0
            };
        }
        #endregion

    }
}
