using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public enum ElementType {
        mountain = 1,
        river,
        road,
        plain,
        food,
        lumber,
        marble,
        steel,
        crystal,
        pass,
        camp,
        npc_city,
        townhall = 80,
        armycamp,
        produce_lumber,
        produce_steel,
        produce_food,
        produce_marble,
        storage_lumber,
        storage_steel,
        storage_food,
        storage_marble,
        tribute_gold,
        siege_up,
        march_speed_up,
        paper_dominant_up,
        scissors_dominant_up,
        rock_dominant_up,
        stronghold,
        hero_attack_up,
        hero_defence_up,
        durability_up,
        border,
        none
    }

    public class ElementName {
        public static readonly string mountain = "mountain";
        public static readonly string river = "river";
        public static readonly string road = "road";
        public static readonly string plain = "plain";
        public static readonly string food = "food";
        public static readonly string lumber = "lumber";
        public static readonly string marble = "marble";
        public static readonly string steel = "steel";
        public static readonly string crystal = "crystal";
        public static readonly string pass = "pass";
        public static readonly string camp = "camp";
        public static readonly string npc_city = "npc_city";
        public static readonly string townhall = "townhall";
        public static readonly string armycamp = "armycamp";
        public static readonly string produce_lumber = "produce_lumber";
        public static readonly string produce_steel = "produce_steel";
        public static readonly string produce_food = "produce_food";
        public static readonly string produce_marble = "produce_marble";
        public static readonly string storage_lumber = "storage_lumber";
        public static readonly string storage_steel = "storage_steel";
        public static readonly string storage_food = "storage_food";
        public static readonly string storage_marble = "storage_marble";
        public static readonly string tribute_gold = "tribute_gold";
        public static readonly string siege_up = "siege_up";
        public static readonly string march_speed_up = "march_speed_up";
        public static readonly string paper_dominant_up = "paper_dominant_up";
        public static readonly string scissors_dominant_up = "scissors_dominant_up";
        public static readonly string rock_dominant_up = "rock_dominant_up";
        public static readonly string stronghold = "stronghold";
        public static readonly string border = "border";
    }

    public class ElementCategory {
        public static readonly string resource = "resource";
        public static readonly string pass = ElementName.pass;
        public static readonly string camp = ElementName.camp;
        public static readonly string npc_city = ElementName.npc_city;
        public static readonly string building = "building";
        public static readonly string river = ElementName.river;
        public static readonly string mountain = ElementName.mountain;
        public static readonly string forest = "forest";
    }

    public class GameConst {
        public static readonly float frameAnimationDelta = 0.03333f;
        public const int frame = 30;

#if UNITY_EDITOR
        public const int configMaxLine = 50;
#endif

        //#if UNITY_EDITOR
        //        public static readonly float frameDelta = 0.0333f;
        //#else
        //        public static readonly float frameDelta = 0.0333f;
        //#endif

        public static DateTime ORIGIN_TIME = new DateTime(1970, 1, 1);
        public static long ZONE_TIME = (long)System.DateTime.Now.Subtract(System.DateTime.UtcNow).TotalMilliseconds;

        public static float CAMERA_ASPECT = 0.5625f;
        public static int MAX_BUILD_QUEUE_COUNT = 2;

        public static readonly int animIdle = Animator.StringToHash("Idle");
        public static readonly int animPlay = Animator.StringToHash("Play");
        public static readonly int animStart = Animator.StringToHash("Start");
        public static readonly int animRepeat = Animator.StringToHash("Repeat");
        public static readonly int animMove = Animator.StringToHash("Move");
        public static readonly int animAttack = Animator.StringToHash("Attack");
        public static readonly int animHit = Animator.StringToHash("Hit");
        public static readonly int animSkill = Animator.StringToHash("Skill");
        public static readonly int animCast = Animator.StringToHash("Cast");
        public static readonly int animMoveRight = Animator.StringToHash("MoveRight");
        public static readonly int animReset = Animator.StringToHash("Reset");
        public static readonly int animDead = Animator.StringToHash("Dead");
        public static readonly int animIsMove = Animator.StringToHash("IsMove");
        public static readonly int animVictor = Animator.StringToHash("Victor");
        public static readonly int animFailed = Animator.StringToHash("Failed");

        private static string auto_battle_key = string.Empty;
        public static string AUTO_BATTLE_KEY {
            get {
                if (string.IsNullOrEmpty(auto_battle_key)) {
                    auto_battle_key = string.Concat(RoleManager.GetRoleId(), "_auto_battle");
                }
                return auto_battle_key;
            }
        }

#if UNITY_EDITOR
        public static string ASSETBUNDLE_PATH = Application.dataPath + "/AssetBundles/";
        public static string ASSETBUNDLE_CONFIG_PATH = Application.dataPath + "/AssetBundles/BundleRes/BundleConfig.csv";
#else
        public static string ASSETBUNDLE_PATH = Application.persistentDataPath;
        public static string ASSETBUNDLE_CONFIG_PATH = Application.persistentDataPath + "/BundleConfig.csv";
#endif
        public static string ANDROID_APPID = "com.poukoute.ariesx";
        //public static long TIMEZONE_OFFSET = (long)DateTime.Now.Subtract(DateTime.UtcNow).TotalMilliseconds;

        //public const string CAVALARY = "cavalary";
        public const int MAX_ENDURANCE = 200;
        //public const string NORMAL_MAIL = "message";
        //public const string SYSTEM_MAIL = "system";
        public const string TWO_PART_WITH_COMMA = "{0},{1}";
        public const int BATTLE_MUST_REPLAY_TIMES = 5; 
        // city type
        public const string CAPITAL = "capital";
        public const string REGION_CAPITAL = "region_capital";
        public const string ZONE_CAPITAL = "zone_capital";
        public const string NORMAL_CITY = "normal";

        // city enum
        public const int NPC_CITY_SN = 200;
        public const int CITY_CENTER = 100;
        public const int CITY_ENTRANCE_NS = 25;
        public const int CITY_ENTRANCE_EW = 50;

        // gift group
        public const string GIFT_GROUP_PREFIX = "guide_free_gift";
        public const string GIFT_GROUP_1 = "guide_free_gift_1";
        public const string GIFT_GROUP_2 = "guide_free_gift_2";
        public const string GIFT_GROUP_3 = "guide_free_gift_3";
        public const string GIFT_GROUP_4 = "guide_free_gift_4";

        // hero
        public const int HERO_MAX_TIER = 5;
        public const int HERO_COUNT = 46;
        public const int HERO_ROW = 3;

        // Mission 
        public const int DAILY_TASK_STAGE = 5;
        public const int DAILY_TASK_REWARD_COUNT = 6;

        // hero energy
        public const int HERO_ENERGY_MAX = 20;
        public const int HERO_ENERGY_COST = 2;
        public const int HALF_HOUR_MILLION_SECONDS = 30 * 60 * 1000;
        public const int HOUR_MILLION_SECONDS = 3600000;
        public const int DAY_MILLION_SECONDS = 86400000;

        public const int LOTTERY_LOW_LEVEL = 3;

        // troop distance
        public const int TROOP_NEAR = 50;
        public const int TROOP_MEDIUM = 150;
        public const int TROOP_FAR = 300;
        public const float TROOP_SHOW_CURE = 0.6f;

        public const int MESSAGE_TIME_SHOW_OFFSET = 300;
        public const float CREATE_ALLIANCE_GOLD_COST = 200;

        // task type
        public const string RECRUIT = "recruit";
        public const string HERO_LEVEL = "hero_level";
        public const string TROOP_ADD_HERO = "troop_add_hero";
        public const string BUILDING_LEVEL = "building_level";
        public const string RESOURCE_LEVEL = "resource_level"; // find level tile
        public const string BUILDING_UPGRADE = "building_upgrade";
        public const string JOIN_ALLIANCE = "join_alliance";
        public const string ALLIANCE_CHAT = "alliance_chat";

        public const string HERO_LEVELUP_TIMES = "hero_levelup_times";
        public const string BUILDING_LEVELUP_TIMES = "building_levelup_times"; // jump building
        public const string RESOURCE_AMOUNT = "resource_amount";
        public const string RESOURCE_OCCUPY = "occupied_resource_times";
        public const string RESOURCE_PRODUCE = "resource_produce";
        public const string RAID_RESOURCE_LEVEL = "raid_resource_level";
        public const string FORCE = "force";
        public const string GOLD_AMOUNT = "gold_totalamount";
        public const string STRONGHOLD_AMOUNT = "stronghold_amount";
        public const string TASK_RECRUIT_AMOUNT = "recruit_amount";
        public const string BATTLE_WIN = "battle_win";
        public const string PVP_TIMES = "pvp_times";
        public const string GET_TRIBUTE_TIMES = "get_tribute_times";
        public const string ATTACK_NPCCITY = "attack_npccity";
        public const string ATTACK_PASS = "attack_pass";
        public const string RESOURCE_REVOKE = "resource_revoke";
        public const string SPECIAL_BUILDING_LEVEL = "specify_building_level";

        public const string NORMAL = "normal";
        public const int TASK_REFRESH_PERIOD = 60;

        // camp
        public const int CAMP_MAX_TIMES = 3;
        public const string PASS_PASS = "pass";
        public const string PASS_BRIDGE = "bridge";

        public const int BUILD_QUEUE_COST = 300;

        // dominant basic
        public const int DOMINANT_BASIC = 20;

        // Resoulution
        public const float RESOULUTION_HEIGHT = 1334F;
        public const float RESOULUTION_WIDTH = 750F;

        public const string AVATAR_DEFAULT = "hero_106";
        public const string ALLIANCE_LANGUAGE = "alliance_language_";

        //PointsLimit
        public const string POINTS_LIMIT_TYPE_LAND = "land";

        public const string MONTH_CARD_1 = "month_card_1";

        #region direction
        public static readonly Vector2 RightDown = new Vector2(1, -1);
        public static readonly Vector2 LeftUp = new Vector2(-1, 1);
        public static readonly Vector2 LeftDown = new Vector2(-1, -1);
        #endregion
    }
}
