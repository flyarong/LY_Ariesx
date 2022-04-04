using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Poukoute {
    public class LocalConst {
        public const int PLAYER_NAME = 1;
        public const int PLAYER_DESCRIPE = 2;
        public const int ALLIANCE_NAME = 3;
        public const int ALLIANCE_DESCRIPE = 4;
        public const int ALLIANCE_APPLY = 5;
        public const int MAIL = 6;
        public const int ALLIANCE_MARK = 7;

        private readonly static Dictionary<string, Dictionary<int, int>> characterLimit =
            new Dictionary<string, Dictionary<int, int>> {
                { "cn", new Dictionary<int, int> {
                        {PLAYER_NAME, 8 },
                        {PLAYER_DESCRIPE, 20 },
                        {ALLIANCE_NAME, 8 },
                        {ALLIANCE_DESCRIPE, 60 },
                        {ALLIANCE_APPLY, 20},
                        {MAIL, 100 },
                        {ALLIANCE_MARK, 8}
                    }
                },
                { "en", new Dictionary<int, int> {
                        {PLAYER_NAME, 16 },
                        {PLAYER_DESCRIPE, 40 },
                        {ALLIANCE_NAME, 16 },
                        {ALLIANCE_DESCRIPE, 120 },
                        {ALLIANCE_APPLY, 40 },
                        {MAIL, 200 },
                        {ALLIANCE_MARK, 16 }
                    }
                }
            };

        public static int GetLimit(int index) {
            return characterLimit[LocalManager.Language][index];
        }
    }
}
