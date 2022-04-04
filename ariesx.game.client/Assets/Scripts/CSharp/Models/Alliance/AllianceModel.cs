using System.Collections.Generic;

namespace Poukoute {
    public enum JoinConditionType {
        Free = 0,
        Close,
        Limit,
        Apply
    }

    public enum AllianceRole {
        None = -1,
        Member = 0,
        Elder,
        Leader,
        Owner
    }

    public enum AllianceViewType {
        None,
        Tips,
        Search,
        Create,
        Alliance,
        City,
        Subordinate
    }

    public enum AllianceDistanceColor {
        None,
        Nearnest = 0x82ffae,
        Nearer = 0xa2ff77,
        Near = 0xffe08d,
        Medium = 0xffc485,
        Far = 0xff9f82,
        Farther = 0xff7575,
        Farthest = 0xff7d9f
    }

    // distance power 2
    public enum AllianceSqrDistance {
        None,
        Nearnest = 250,
        Nearer = 10000,
        Near = 16500,
        Medium = 40000,
        Far = 62500,
        Farther = 90000,
        Farthest = 122500
    }

    public enum AllianceSubWindowType {
        Apply,
        Logo,
        Setting,
        SubordinateStatus,
        None
    }

    public enum AllianceMemberSort {
        Activity,
        Crow
    }

    public enum AllianceFallenType {
        City,
        Pass
    }

    public enum AllianceMemberSortType {
        role,
        force,
        alliance_exp
    }

    public class AllianceModel : BaseModel {
        public static Dictionary<AllianceSqrDistance, string> DistanceLocal =
            new Dictionary<AllianceSqrDistance, string> {
                {AllianceSqrDistance.Farthest, LocalManager.GetValue(LocalHashConst.alliance_farthest)},
                {AllianceSqrDistance.Farther,  LocalManager.GetValue(LocalHashConst.alliance_farther)},
                {AllianceSqrDistance.Far,      LocalManager.GetValue(LocalHashConst.alliance_far)},
                {AllianceSqrDistance.Medium,   LocalManager.GetValue(LocalHashConst.alliance_medium)},
                {AllianceSqrDistance.Near,     LocalManager.GetValue(LocalHashConst.alliance_near)},
                {AllianceSqrDistance.Nearer,   LocalManager.GetValue(LocalHashConst.alliance_nearer)},
                {AllianceSqrDistance.Nearnest, LocalManager.GetValue(LocalHashConst.alliance_nearnest)},
            };

        public static readonly int townhallRequireLevel = 3;
        public static string AllianceMarkPre = "AllianceMark";
        public static string AllianceMarkRecorde = "recorded";

        public string ApplyContent;

        private static Dictionary<string, BaseConf> allianceLanguageConf;

        public static Dictionary<string, BaseConf> AllianceLanguageConf {
            get {
                if (allianceLanguageConf == null)
                    allianceLanguageConf = ConfigureManager.GetConfDict<AllianceLanguageConf>();
                return allianceLanguageConf;
            }
        }
    }
}
