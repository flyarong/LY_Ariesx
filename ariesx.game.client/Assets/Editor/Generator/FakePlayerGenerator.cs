using UnityEngine;
using UnityEditor;
using System.Collections;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class FakePlayerGenerator : EditorWindow {
        private int count;
        private string worldId;
        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Player count");
            count = EditorGUILayout.IntField(count);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("World id");
            worldId = EditorGUILayout.TextField(worldId);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Players")) {
                this.GenerateFakePlayers(count);
            }
        }

        private void GenerateFakePlayers(int udid) {
            if (udid == 0) {
                return;
            } else {
                NetManager.Disconnect();
                NetManager.RemoveAllConnectedEvent();
                NetManager.AddConnectedAction(() => {
                    ConnectReq connectReq = new ConnectReq() {
                        WorldId = "b8d4369e-cdad-4ff2-a14a-c26d6bb790ae"
                    };
                    
                    NetManager.SendMessage(connectReq, "", null);
                    GenerateFakePlayers(--udid);
                });
                NetManager.ConnectToServer("192.168.20.212");
            }
        }
    }
}