using Protocol;
using System.Collections.Generic;

namespace Poukoute {
    public class PayModel : BaseModel {
        public DailyShop dailyShop = null;
        public List<DailyGift> dailyGifts = new List<DailyGift>();
        public List<MonthCard> monthCards = new List<MonthCard>();
        
        // pay reward
        public int payRewardLevel = 0;
        public float payAmount = 0;

        public bool haveMonthcard;
        public bool canmonthCardReward;
        public long expiredTime;
        public void Refresh(LoginAck loginAck) {
            if (loginAck.MonthCards.Count > 0) {
                haveMonthcard = true;
                canmonthCardReward = loginAck.MonthCards[0].IsReward;
                expiredTime = loginAck.MonthCards[0].ExpiredTime;
            } else {
                haveMonthcard = false;
            }
        }
    }
}
