using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using LyStatistic;
using System.Runtime.InteropServices;
using LitJson;
using Protocol;

namespace Poukoute {
    public class LYGameData : MonoBehaviour {
        private static LYGameData instance;
        public static LYGameData Instance {
            get {
                return instance;
            }
        }
        private static readonly string KEY_DEVICE_ID = "key_device_id";
        private static readonly string COUNTRY_NAME = "安哥拉,AO#阿富汗,AF#阿尔巴尼亚,AL#阿尔及利亚,DZ#安道尔共和国,AD#安圭拉岛,AI#安提瓜和巴布达,AG#阿根廷,AR#亚美尼亚,AM#阿森松,00#澳大利亚,AU#奥地利,AT#阿塞拜疆,AZ#巴哈马,BS#巴林,BH#孟加拉国,BD#巴巴多斯,BB#白俄罗斯,BY#比利时,BE#伯利兹,BZ#贝宁,BJ#百慕大群岛,BM#玻利维亚,BO#博茨瓦纳,BW#巴西,BR#文莱,BN#保加利亚,BG#布基纳法索,BF#缅甸,MM#布隆迪,BI#喀麦隆,CM#加拿大,CA#开曼群岛,00#中非共和国,CF#乍得,TD#智利,CL#中国,CN#哥伦比亚,CO#刚果,CG#库克群岛,CK#哥斯达黎加,CR#古巴,CU#塞浦路斯,CY#捷克,CZ#丹麦,DK#吉布提,DJ#多米尼加共和国,DO#厄瓜多尔,EC#埃及,EG#萨尔瓦多,SV#爱沙尼亚,EE#埃塞俄比亚,ET#斐济,FJ#芬兰,FI#法国,FR#法属圭亚那,GF#加蓬,GA#冈比亚,GM#格鲁吉亚,GE#德国,DE#加纳,GH#直布罗陀,GI#希腊,GR#格林纳达,GD#关岛,GU#危地马拉,GT#几内亚,GN#圭亚那,GY#海地,HT#洪都拉斯,HN#香港,HK#匈牙利,HU#冰岛,IS#印度,IN#印度尼西亚,ID#伊朗,IR#伊拉克,IQ#爱尔兰,IE#以色列,IL#意大利,IT#科特迪瓦,00#牙买加,JM#日本,JP#约旦,JO#柬埔寨,KH#哈萨克斯坦,KZ#肯尼亚,KE#韩国,KR#科威特,KW#吉尔吉斯坦,KG#老挝,LA#拉脱维亚,LV#黎巴嫩,LB#莱索托,LS#利比里亚,LR#利比亚,LY#列支敦士登,LI#立陶宛,LT#卢森堡,LU#澳门,MO#马达加斯加,MG#马拉维,MW#马来西亚,MY#马尔代夫,MV#马里,ML#马耳他,MT#马里亚那群岛,00#马提尼克,00#毛里求斯,MU#墨西哥,MX#摩尔多瓦,MD#摩纳哥,MC#蒙古,MN#蒙特塞拉特岛,MS#摩洛哥,MA#莫桑比克,MZ#纳米比亚,NA#瑙鲁,NR#尼泊尔,NP#荷属安的列斯,00#荷兰,NL#新西兰,NZ#尼加拉瓜,NI#尼日尔,NE#尼日利亚,NG#朝鲜,KP#挪威,NO#阿曼,OM#巴基斯坦,PK#巴拿马,PA#巴布亚新几内亚,PG#巴拉圭,PY#秘鲁,PE#菲律宾,PH#波兰,PL#法属玻利尼西亚,PF#葡萄牙,PT#波多黎各,PR#卡塔尔,QA#留尼旺,00#罗马尼亚,RO#俄罗斯,RU#圣卢西亚,LC#圣文森特岛,VC#东萨摩亚(美),00#西萨摩亚,00#圣马力诺,SM#圣多美和普林西比,ST#沙特阿拉伯,SA#塞内加尔,SN#塞舌尔,SC#塞拉利昂,SL#新加坡,SG#斯洛伐克,SK#斯洛文尼亚,SI#所罗门群岛,SB#索马里,SO#南非,ZA#西班牙,ES#斯里兰卡,LK#圣卢西亚,LC#圣文森特,VC#苏丹,SD#苏里南,SR#斯威士兰,SZ#瑞典,SE#瑞士,CH#叙利亚,SY#台湾省,TW#塔吉克斯坦,TJ#坦桑尼亚,TZ#泰国,TH#多哥,TG#汤加,TO#特立尼达和多巴哥,TT#突尼斯,TN#土耳其,TR#土库曼斯坦,TM#乌干达,UG#乌克兰,UA#阿拉伯联合酋长国,AE#英国,GB#美国,US#乌拉圭,UY#乌兹别克斯坦,UZ#委内瑞拉,VE#越南,VN#也门,YE#南斯拉夫,YU#津巴布韦,ZW#扎伊尔,ZR#赞比亚,ZM#";

        private void Start() {
            UpdateManager.Regist(UpdateInfo.LYGameData, this.UpdateAction);
        }

        public static void initSDK(string appid, string channelname, string version, int serverType) {
            try {
                getInstance().doInit(appid, channelname, version, serverType);
            }
            catch (Exception e) {
                Debug.Log("initSDK: " + e.ToString());
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    private string replaceCountryName(string original) {
        try {
            JsonData json = JsonMapper.ToObject(original);
            string state = (string)json["state"];
            int index = COUNTRY_NAME.IndexOf(state);
            string str1 = COUNTRY_NAME.Substring(index);
            int index2 = str1.IndexOf("#");
            string str2 = str1.Substring(0, index2);
            string[] arr = str2.CustomSplit(',');
            original = original.Replace(state, arr[1]);
            Debug.Log(original);
            return original;
        } catch (Exception e) {
            Debug.Log(e.ToString());
            return original;
        }
    }
#endif

        public static LYGameData getInstance() {
            return instance;
        }

        void OnApplicationPause(bool isPause) {
            if (isPause) {
                LyStatistic.Manager.Me.on_Pause();
            }
            else {
                LyStatistic.Manager.Me.on_Resume();
            }
            //如果还有事件没有上传，那么先保存下来，下次启动的时候再上传
            LyStatistic.Manager.Me.OnApplicationPause();
        }

        void OnApplicationQuit() {
            //如果还有事件没有上传，那么先保存下来，下次启动的时候再上传
            LyStatistic.Manager.Me.OnApplicationQuit();
        }

        // To do: need check.
        private void UpdateAction() {
            //每帧都要调用
            try {
                LyStatistic.Manager.Me.Update();
            }
            catch {
                ;
            }
        }

        string getPackKey() {
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		
		string packKey = jmain.CallStatic<string>("getMark", jo);
		Debug.Log ("packKey: " + packKey);
		return packKey;
#endif

            return string.Empty;
        }


        private void doInit(string appid, string channelname, string version, int serverType) {
            //扩展参数，初始化时需要
            LyStatistic.ExtraParams extra = new ExtraParams();
            LyStatistic.Manager.Log = (string msg) => {
                //Debug.Log(string.Format("<color=#FFFF00FF>{0}</color>", msg));
            };

            //平台编号
            if (Application.platform == RuntimePlatform.Android)
                extra.platform = 1;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                extra.platform = 2;
            //分包ID
            extra.packageId = getPackKey();

            //设备唯一ID
            extra.deviceId = SystemInfo.deviceUniqueIdentifier;

            //设置品牌
            extra.brand = SystemInfo.deviceModel;


            extra.ip = getIP();
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
                //Do sth.
                extra.network = "wifi";
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {//当用户使用移动网络时
                                                                                                              //Do sth.
                extra.network = "4G";
            }



#if UNITY_IOS && !UNITY_EDITOR
		string sysIdKeyChain = getKeyChainDUI();
		if (sysIdKeyChain == null ||
            sysIdKeyChain.CustomIsEmpty()) {
			sysIdKeyChain = extra.deviceId;
			saveKeyChainDUI (sysIdKeyChain);
		}else{
			extra.deviceId = sysIdKeyChain;
		}

		extra.manufacturer = "Apple";
		extra.resolution = getResolution ();
		extra.idfaimei = getIDFA();
		getLoaction();
		//注册崩溃
		registerErrors ();

        LyStatistic.Manager.Me.loginBack += new LoginCallBack(getLoaction);
#elif UNITY_ANDROID && !UNITY_EDITOR
		string sysDeviceId = getSysData();
		Debug.Log("sysData: " + extra.deviceId + ", " + sysDeviceId);
		if(sysDeviceId == null || sysDeviceId.CustomIsEmpty()){
			setSysData(extra.deviceId);
		}else{
			extra.deviceId = sysDeviceId;
		}
		
		extra.idfaimei = getImei();
		extra.ip = getIp ();
		extra.manufacturer = getManufacturer ();
		extra.resolution = getResolution ();
		extra.network = getNetwork ();
        LyStatistic.Manager.Me.loginBack += new LoginCallBack(getLocation);
#elif UNITY_EDITOR
            extra.idfaimei = SystemInfo.deviceUniqueIdentifier;
#endif

            //初始化日志事件管理器
            bool ret = LyStatistic.Manager.Me.Init(appid, channelname, version, extra, Application.persistentDataPath, serverType);
            Debug.Log("init end calling..." + ret);


            if (ret == false) {
                Debug.Log("init failed !!");
                return;
            }
        }

        private string getIP() {
            string ip = string.Empty;
            try {
                var strHostName = System.Net.Dns.GetHostName();
                var ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                ip = ipEntry.AddressList[2].ToString();
                //Debug.Log("ipipppp" + addr[2].ToString());
            }
            catch (Exception e) { }
            return ip;
        }

        void Awake() {
            DontDestroyOnLoad(gameObject);
            gameObject.name = "LyGame";
            instance = this;
        }


#if UNITY_IOS && !UNITY_EDITOR
	
	//获取IDFA
	[DllImport("__Internal")]
	private static extern string iOSdoGetIDFA();

	public static string getIDFA(){		
		string result = string.Empty;		
		if (Application.platform != RuntimePlatform.OSXEditor) 
		{
			result = iOSdoGetIDFA();			
		}
		
		return result;
		
	}

	//获取钥匙串UnityUID
	[DllImport("__Internal")]
	private static extern string iOSdoGetKeyChainDUI();
	
	public static string getKeyChainDUI(){
		string result = string.Empty;
		if (Application.platform != RuntimePlatform.OSXEditor) 
		{
		result = iOSdoGetKeyChainDUI();
		}
		return result;
	}
	

	//保存UnityUID到钥匙串
	[DllImport("__Internal")]
	private static extern void iOSdoSaveKeyChainUID(string uid);

	public static void saveKeyChainDUI(string uid){
		if (Application.platform != RuntimePlatform.OSXEditor) {
	iOSdoSaveKeyChainUID(uid);
	Debug.Log("iOSdoSaveKeyChainUID ....");
		}
		
	}

	
	//获取运营商
	[DllImport("__Internal")]
	private static extern string iOSdoGetResolution();
	
	public static string getResolution(){
		
		string result = string.Empty;
		
		if (Application.platform != RuntimePlatform.OSXEditor) 
		{
			result = iOSdoGetResolution();			
		}
		
		return result;
		
	}
	
	
	
	//获取地理位置注册方法
	[DllImport("__Internal")]
	private static extern void iOSdoGetLoaction();
	
	public static void getLoaction(){
		
		if (Application.platform != RuntimePlatform.OSXEditor) 
		{
			iOSdoGetLoaction();
			
		}
	}
	
	//地理位置回调
	public void receiveLoaction(string location){
		Debug.Log ("receiveLoaction"+location);
		LyStatistic.Manager.Me.on_location(location);
	}
	

	//错误搜集注册方法
	[DllImport("__Internal")]
	private static extern void iOSdoRegisterErrors();

	public static void registerErrors(){

	if (Application.platform != RuntimePlatform.OSXEditor) 
	{
	iOSdoRegisterErrors();

	}
	}

	//错误收集回调
	public void receiveErrors(string errors){
	Debug.Log ("receiveErrors"+errors);
	LyStatistic.Manager.Me.on_error(errors);
	}


#elif UNITY_ANDROID && !UNITY_EDITOR

    //开始让android 获取系统地理位置
    public static void getLocation(){
        Debug.Log("getLocation calling...");
        AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jmain.CallStatic("androidGetlocation", jo);
		
    }

	//地理位置回调
	public void receiveLoaction(string location){
		Debug.Log ("receiveLoaction: "+location);
		location = replaceCountryName(location);
		LyStatistic.Manager.Me.on_location(location);
	}

	private string getImei(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string imei = jmain.CallStatic<string>("getImei");
		return imei;
	}

    public void onReceivedError(string error){
        Debug.Log("onReceivedError calling...." + error);
        LyStatistic.Manager.Me.on_error(error);
    }


	public void onReceivedImei(string imei)
	{
		Debug.Log("onReceivedImei calling...." + imei);
		LyStatistic.Manager.setIdfaImei(imei);
	}

	public string getIp(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string ip = jmain.CallStatic<string>("getIp");
		return ip;
	}

	public string getManufacturer(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string manufacturer = jmain.CallStatic<string>("getManufacturer");
		return manufacturer;
	}

	public string getNetwork(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string network = jmain.CallStatic<string>("getNetwork");
		return network;
	}

	public string getResolution(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string resolution = jmain.CallStatic<string>("getResolution");
		return resolution;
	}
	
	public string getSysData(){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		string deviceId = jmain.CallStatic<string>("getSysData", KEY_DEVICE_ID);
		return deviceId;
	}
	
	public void setSysData(string deviceId){
		AndroidJavaClass jmain = new AndroidJavaClass("com.game.unity.tool.SDKMark");
		jmain.CallStatic("setSysData", KEY_DEVICE_ID, deviceId);
	}

#endif








        public static string getDeviceId() {
            string sysId = SystemInfo.deviceUniqueIdentifier + "#";
            string deviceId = "";
#if UNITY_IOS && !UNITY_EDITOR
        string sysIdKeyChain = getKeyChainDUI();
		if (sysIdKeyChain.Length == 0 || sysIdKeyChain.CustomIsEmpty()) {
			sysIdKeyChain = SystemInfo.deviceUniqueIdentifier;
			saveKeyChainDUI (sysIdKeyChain);
			deviceId = sysId + getIDFA ();
		} else {
			deviceId = sysIdKeyChain+"#"+getIDFA ();
		}
#elif UNITY_ANDROID && !UNITY_EDITOR
		string sysDeviceId = getInstance ().getSysData();
		string imei = getInstance ().getImei ();
		Debug.Log("getDeviceId: " + SystemInfo.deviceUniqueIdentifier + ", " + sysDeviceId);
		if(sysDeviceId == null || sysDeviceId.Equals("")){
			getInstance ().setSysData(SystemInfo.deviceUniqueIdentifier);
			deviceId = sysId + imei;
		}else{
			deviceId = sysDeviceId + "#" + imei;
		}
#endif
            Debug.Log("getDeviceId..." + deviceId);
            return deviceId;
        }


        public static String getBasicData(string apkVersion, string channelId, string gameArea, string accountId, string appId, string ip) {
            JsonData json = new JsonData();
            json["appId"] = appId;
#if UNITY_IOS && !UNITY_EDITOR
        json["platform"] = "2";
        json["manufacturer"] = "Apple";
        json["resolution"] = getResolution();
#elif UNITY_ANDROID && !UNITY_EDITOR
        json["platform"] = "1";
        json["manufacturer"] = getInstance().getManufacturer ();;
        json["resolution"] = getInstance().getResolution();
#endif
            json["channelId"] = channelId;
            json["gameArea"] = gameArea;
            json["packageId"] = getInstance().getPackKey();
            json["apkVersion"] = apkVersion;
            json["sdkVersion"] = StatisticData.sdkVersion;
            json["ip"] = ip;
            json["deviceId"] = getDeviceId();
            json["accountId"] = accountId;

            json["brand"] = SystemInfo.deviceModel;
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
                json["network"] = "wifi";
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
                json["network"] = "4G";
            }

            string result = json.ToJson();
            Debug.Log(result);
            return result;
        }

        private static string JsonToString(string json) {
            return json.Replace("\"", "\\\"");
        }
        #region OnOtherEvent
        public static void OnOtherOpenStore() {
            JsonData json = new JsonData();
            //json["null"] = "null";
            string result = JsonToString(json.ToJson());
            string roleId = RoleManager.GetRoleId();
            string otherEvent = "open_store";
            //Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "roleId : " + roleId + "  otherEvent :" + otherEvent + "  result： " + result));
            LyStatistic.Manager.Me.on_other(roleId, otherEvent, result);
        }

        public static void OnOtherCloseStore() {
            JsonData json = new JsonData();
            //json["null"] = "null";
            string result = JsonToString(json.ToJson());
            string roleId = RoleManager.GetRoleId();
            string otherEvent = "close_store";
            //Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "roleId : " + roleId + "  otherEvent :" + otherEvent + "  result： " + result));
            LyStatistic.Manager.Me.on_other(roleId, otherEvent, result);
        }

        public static void OnOtherClickIgButton() {
            JsonData json = new JsonData();
            DailyShop dailyShop = RoleManager.Instance.GetDailyShop();
            json["gift_id"] = dailyShop == null ? string.Empty : dailyShop.RequestId;
            string result = JsonToString(json.ToJson());
            string roleId = RoleManager.GetRoleId();
            string otherEvent = "click_ig_button";
            //Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "roleId : " + roleId + "  otherEvent :" + otherEvent + "  result： " + result));
            LyStatistic.Manager.Me.on_other(roleId, otherEvent, result);
        }

        public static void OnOtherClickBuyButton(string itemName, string itemNum, string payMoney) {
            JsonData json = new JsonData();
            json["item"] = itemName;
            json["item_num"] = itemNum;
            json["pay_money"] = payMoney;
            string result = JsonToString(json.ToJson());
            string roleId = RoleManager.GetRoleId();
            string otherEvent = "click_buy_button";
            //Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "roleId : " + roleId + "  otherEvent :" + otherEvent + "  result： " + result));
            LyStatistic.Manager.Me.on_other(roleId, otherEvent, result);
        }

        public static void OnOtherBuyItemSuccess(string itemName, string itemNum, string payMoney) {
            JsonData json = new JsonData();
            json["item"] = itemName;
            json["item_num"] = itemNum;
            json["pay_money"] = payMoney;
            string result = JsonToString(json.ToJson());
            string roleId = RoleManager.GetRoleId();
            string otherEvent = "buy_item_success";
            //Debug.Log(string.Format("<color=#00FF00FF>{0}</color>", "roleId : " + roleId + "  otherEvent :" + otherEvent + "  result： " + result));
            LyStatistic.Manager.Me.on_other(roleId, otherEvent, result);
        }
        #endregion
    }
}
