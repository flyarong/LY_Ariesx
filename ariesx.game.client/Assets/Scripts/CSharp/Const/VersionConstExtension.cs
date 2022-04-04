using Protocol;

namespace Poukoute{
	public partial class VersionConst {
        //public static bool IsDeveloper() {
        //    return version == "develop" ||
        //           version == "qaen" || 
        //           version == "qacn";
        //}

        //public static bool IsOnline() {
        //    return version == "googleplay" ||
        //           version == "taptap";
        //}

        public static bool CanLinkFacebook() {
#if UNITY_EDITOR || DEVELOPER
            return true;
#else
            return version.CustomEquals("qaenproduct") || 
                   version.CustomEquals("googleplay");
#endif
        }
    }
}
