using DummyEgg.MasterDataWorker.Tables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DummyEgg.ProjectGK.MasterData
{
    public class MstExpModel
    {
        public static MstExp_MMItemTable _mmData = null;
        public static MstExp_MMItemTable MMData
        {
            get
            {
                if (_mmData == null)
                {
                    _mmData = MasterdataManager.Instance.Data.MstExp_MMItemTable;
                }
                return _mmData;
            }
            private set
            {
                _mmData = value;
            }
        }

        public static MstExp_MMItem GetRecordBy(int id)
        {
            return MMData.FindBylv(id);
        }

        public static int CalcExpToLv(int exp, out int nextExp)
        {
            foreach(var v in MMData.All)
            {
                if (v.needExp >= exp)
                {
                    nextExp = v.needExp - exp;
                    return v.lv;
                }
            }

            nextExp = 0;
            return MMData.All.Last.lv;
        }
    }
}
