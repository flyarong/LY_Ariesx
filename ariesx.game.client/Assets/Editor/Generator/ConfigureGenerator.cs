using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Poukoute {
    public class ConfigureGenerator : MonoBehaviour {
        private static StreamWriter configureWriter;

        public static void GeneratorConfigure() {
            Caching.CleanCache();
            string configuresPath = "/Resources/Configures";
            if (Directory.Exists(configuresPath)) {
                Directory.Delete(configuresPath, true);
            }
            Directory.CreateDirectory(configuresPath);

            string path = string.Concat(
                 Application.dataPath, "/Resources/Configures/configure_list.csv");
            File.WriteAllText(path, "");
            configureWriter = new StreamWriter(path, true, Encoding.UTF8);
            configureWriter.WriteLine("file,amount");
            ReadFile("terrain_resource_production");
            ReadFile("terrain_basic_type");
            ReadFile("resource_basic_type");
            ReadFile("army_type");
            ReadFile("hero_attribute");
            ReadFile("hero_levels");
            ReadFile("default_hero_skill_value");
            ReadFile("gacha_group");
            ReadFile("hero_attribute_basic");
            ReadFile("terrain_resource_troop");
            ReadFile("art_prefab");
            ReadFile("art_alliance_emblem");
            ReadFile("art_lottery");
            ReadFile("art_hero_avatar");
            ReadFile("art_animation_hero");
            ReadFile("art_tile_layer");
            ReadFile("art_audio");
            ReadFile("art_audio_import");
            ReadFile("campaign_hero_effect");
            ReadFile("hero_audio");
            ReadFile("skill_audio");
            ReadFile("art_skill");
            ReadFile("art_npc_city");
            ReadFile("building");
            ReadFile("enemy_heroes_level_show");
            ReadFile("terrain_resource_reward");
            ReadFile("hero_rarity");
            ReadFile("skill");
            ReadFile("skill_effect");
            ReadFile("mini_map_city");
            ReadFile("mini_map_pass");
            ReadFile("pass");
            ReadFile("stronghold_recruit");
            ReadFile("fd_field_reward");
            ReadFile("points_limit");
            ReadFile("npc_city");
            ReadFile("tribute");
            ReadFile("treasure_map_reward");
            ReadFile("army_camp");
            ReadFile("warehouse_attribute");
            ReadFile("resource_produce_building");
            ReadFile("dominant_up_building");
            ReadFile("siege_up_building");
            ReadFile("march_speed_up_building");
            ReadFile("tribute_gold_building");
            ReadFile("task");
            ReadFile("task_daily");
            ReadFile("task_daily_vitality");
            ReadFile("force_reward");
            ReadFile("occupy_points");
            ReadFile("capture_points");
            ReadFile("explorercamp_exp");
            ReadFile("alliance_level");
            ReadFile("fte_step");
            ReadFile("chapter");
            ReadFile("chapter_task");
            ReadFile("daily_reward_limit");
            ReadFile("demon_troop");
            ReadFile("shop_product");
            ReadFile("product_gem");
            ReadFile("product_gold");
            ReadFile("daily_gift");
            ReadFile("month_card");
            ReadFile("player_hero_avatar");
            ReadFile("hero_attack_up_building");
            ReadFile("hero_defence_up_building");
            ReadFile("durability_up_building");
            ReadFile("alliance_language");
            ReadFile("first_time_pay");

            // Editor for battle
            ReadFile("battle_hero_position");
            ReadFile("battle_hero_attack");
            ReadFile("battle_hero_skill");
            ReadFile("battle_curve");
            ReadFile("battle_hero_animation");
            // loginReward
            ReadFile("login_reward");
            configureWriter.Flush();
            configureWriter.Close();
        }

        private static void ReadFile(string name) {
            TextAsset conf =
                AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Configures/" + name + ".csv");
            string confText = conf.text;
            string[] splitArray = { "\r\n", "\r", "\n" };
            string[] lines = confText.CustomSplit(ref splitArray);
            StreamWriter streamWriter = null;
            int count = 0;
            for (int i = 1; i < lines.Length; i++) {
                if (lines[i] != "") {
                    if (i % GameConst.configMaxLine == 1) {
                        string path = string.Format(Application.dataPath + "/Resources/Configures/{0}_{1}.csv",
                      name, ++count);
                        File.WriteAllText(path, "");
                        streamWriter = new StreamWriter(path, true, Encoding.UTF8);
                        streamWriter.WriteLine(lines[0]);
                    }
                    streamWriter.WriteLine(lines[i]);
                }
                if (i % GameConst.configMaxLine == 0 || i == lines.Length - 1) {
                    try {
                        streamWriter.Flush();
                        streamWriter.Close();
                    } catch {
                        Debug.LogError(name + " has empty line in " + i);
                    }
                }
            }
            configureWriter.WriteLine(name + "," + count);
        }

        private static void ReadEditorBattleFile() {

        }
    }
}
