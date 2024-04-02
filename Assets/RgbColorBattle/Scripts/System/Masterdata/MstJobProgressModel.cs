using DummyEgg.MasterDataWorker.Tables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DummyEgg.ProjectGK.MasterData
{
    public class MstJobProgressModel
    {
        public static MstJobProgress_MMItemTable _mmData = null;
        public static MstJobProgress_MMItemTable MMData
        {
            get
            {
                if (_mmData == null)
                    _mmData = MasterdataManager.Instance.Data.MstJobProgress_MMItemTable;
                return _mmData;
            }
            private set
            {
                _mmData = value;
            }
        }

        public static MstJobProgress_MMItem GetRecordBy(int id)
        {
            return MMData.FindByjob_id(id);
        }
    }
}
