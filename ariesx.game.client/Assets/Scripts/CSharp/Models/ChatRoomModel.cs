using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public enum ChatChannel {
        None,
        Alliance,
        World,
        Private,
        State
    }

    public class ChatRoomModel : BaseModel {
        public Dictionary<string, PersonalMessage> privateDict = new Dictionary<string, PersonalMessage>();
        public List<PersonalMessage> privateList = new List<PersonalMessage>();
        public int privateCountShow = 9;
        public int privateTailIndex = -1;
        public int privatePage = 1;

        public List<ChatMessage> worldMessageList = new List<ChatMessage>();
        public int worldMaxCount = 200;

        public List<ChatMessage> stateMessageList = new List<ChatMessage>();
        public int stateMaxCount = 200;

        public List<AllianceMessage> allianceMessageList = new List<AllianceMessage>(50);
        public List<AllianceMessage> allianceInfoList = new List<AllianceMessage>(10);
        public List<AllianceMessage> allianceSlaveInfoList = new List<AllianceMessage>(10);
        public int allianceMessageMaxCount = 200;

        public bool isPrivateLoadAll = false;
        //public string privateMsgUserName;
        public string privateMsgTo;
        public string privateMsgContent;
        public bool dirty = false;
        public ChatChannel channel = ChatChannel.None;
        public MailType mailType;

        public int newPrivateMessage = 0;

        public void Refresh(GetWorldChatAck worldChat) {
            this.worldMessageList = worldChat.ChatRoom.Messages;
        }

        public void Refresh(GetStateChatAck stateChat) {
            this.stateMessageList = stateChat.ChatRoom.Messages;
        }

        public void Refresh(GetAllianceMessagesAck allianceChat) {
            this.allianceSlaveInfoList = allianceChat.SlaveInfo;
            this.allianceInfoList = allianceChat.Messages;
            this.allianceMessageList = allianceChat.Chats;
            this.allianceMessageList.AddRange(allianceInfoList);
            this.allianceMessageList.AddRange(allianceSlaveInfoList);
            this.allianceMessageList.Sort((a, b) => {
                return a.Timestamp.CompareTo(b.Timestamp);
            });
            //Debug.LogError("GetAllianceMessagesAck " + this.allianceMessageList.Count);
        }
    }
}
