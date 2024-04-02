using DummyEgg.MasterDataWorker.Tables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DummyEgg.ProjectGK.MasterData
{
    public class MstJobDescModel
    {
        public static MstJobDesc_MMItemTable _mmData = null;
        public static MstJobDesc_MMItemTable MMData
        {
            get
            {
                if (_mmData == null)
                    _mmData = MasterdataManager.Instance.Data.MstJobDesc_MMItemTable;
                return _mmData;
            }
            private set
            {
                _mmData = value;
            }
        }

        public static MstJobDesc_MMItem GetRecordBy(int id)
        {
            return MMData.FindByjob_id(id);
        }
    }
}
