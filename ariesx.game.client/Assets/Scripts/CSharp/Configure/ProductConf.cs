using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public enum ProductType {
        Gem,
        DailyGift,
        MonthCard,
        GemExchangeGold,
        SmartGiftBag,
        None
    }

    public enum SmartGiftBagType {
        Gem,
        Gold,
        Hero,
        None
    }

    public class ProductConf : BaseConf {
        public string id;
        public string price;
        public ProductType type;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["product_id"];
            this.price = attrDict["price"];
            switch (attrDict["product_type"]) {
                case "gem":
                    type = ProductType.Gem;
                    break;
                case "daily_gift":
                    type = ProductType.DailyGift;
                    break;
                case "month_card":
                    type = ProductType.MonthCard;
                    break;
                default:
                    type = ProductType.None;
                    break;
            }
        }

        public override string GetId() {
            return this.id;
        }

        static ProductConf() {
            ConfigureManager.Instance.LoadConfigure<ProductConf>();
        }

        public static ProductConf GetConf(string id) {
            return ConfigureManager.GetConfById<ProductConf>(id);
        }
    }
}
