using Protocol;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public enum RoleAction {
        Idle,
        Attack,
        Skill,
        Hit,
        Move,
        Victor,
        Failed
    }

    public class BattleHero {
        public string name;
        public int damage;
        public int skillCount;
        public int health;
        public int lose;
    }

    public class BattleFrame {
        public int index;
        public int position;
        public RoleAction action;
    }

    public class BattleBuff {
        public string name;
        public int round;
        public GameObject obj;
        public BattleEffectTargetType target;
    }

    public class BattleModel : BaseModel {
        private Troop troopAttack;
        private Troop troopDefense;

        public bool isFte = false;

        public string battleID;
        public GetBattleReportAck battleDetail;
        public BattleReport battleResult;
        public Dictionary<string, BattleHero> battleHeroDict;
        public int totalLost;
        public string attackerName;
        public string defenderName;
        public Battle.PlayerInfo attacker;
        public Battle.PlayerInfo defender;

        public void LoadBattleTroop(GetBattleReportAck battleDetail, BattleReport battleResult) {
            this.battleDetail = battleDetail;
            this.battleResult = battleResult;
        }

        public void LoadBattle(byte[] detail, byte[] troop) {
            string package;
            int tag;
            this.battleDetail = 
                NetManager.Instance.DeserializePackage(out package, out tag, detail) as GetBattleReportAck;
            GetBattleReportsAck battleReports = 
                NetManager.Instance.DeserializePackage(out package, out tag, troop) as GetBattleReportsAck;
            this.battleResult = battleReports.Reports[0];
            this.battleResult.Report.Attacker.BasicInfo.Name =
                LocalManager.GetValue(LocalHashConst.fte_battle_troop_1);
            this.battleResult.Report.Defender.BasicInfo.Name =
               LocalManager.GetValue(LocalHashConst.fte_battle_troop_2);
        }
    }
}