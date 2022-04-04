using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public enum MailType {
        None,
        System,
        Normal,
        Alliance
    }

    public class MailChannel {
        public int tailIndex = 0;
        public int countShow = 12;
        public int page = 0;
        public int pageCount = 0;
        public bool isLoadAll = false;

        public MailChannel(int pageCount, int countShow) {
            this.pageCount = pageCount;
            this.countShow = countShow;
        }
    }

	public class MailModel : BaseModel {
        /* Add data member in this */
        public Dictionary<string, BattleReport> battleReportDict = 
            new Dictionary<string, BattleReport>(10);
        public List<BattleReport> battleReportLsit = new List<BattleReport>(10);
        public Dictionary<string, PersonalMessage> mailDict = 
            new Dictionary<string, PersonalMessage>(10);  
        public List<PersonalMessage> mailList = new List<PersonalMessage>(10);
        public Dictionary<string, SystemMessage> systemDict = 
            new Dictionary<string, SystemMessage>(10);
        public List<SystemMessage> systemList = new List<SystemMessage>(10);
        public MailChannel normal = new MailChannel(13, 13);
        public MailChannel system = new MailChannel(8, 8);
        public MailChannel battle = new MailChannel(7, 7);
        // Normal mail.

        public string subject;
        public string to;
        public string content;

        public int newBattleCount = -1;
        public int newNormalCount = 0;
        public int newSystemCount = 0;

	}
}
