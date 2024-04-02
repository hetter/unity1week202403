using DummyEgg.MasterDataWorker.Tables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DummyEgg.ProjectGK.MasterData
{
    public class MstJobBaseDataModel
    {
        public static MstJobBaseData_MMItemTable _mmData = null;
        public static MstJobBaseData_MMItemTable MMData
        {
            get
            {
                if (_mmData == null)
                    _mmData = MasterdataManager.Instance.Data.MstJobBaseData_MMItemTable;
                return _mmData;
            }
            private set
            {
                _mmData = value;
            }
        }

        public static MstJobBaseData_MMItem GetRecordBy(int id)
        {
            return MMData.FindByjob_id(id);
        }
    }
}
