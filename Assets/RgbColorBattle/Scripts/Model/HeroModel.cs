using DummyEgg.ProjectGK.MasterData;
using UniRx;

namespace DummyEgg.ProjectGK.Model
{
    //public class UnitModel
    //{
    //    public ReactiveProperty<uint> _maxHP = new ();
    //    public ReactiveProperty<uint> _nowHP = new();
    //}

    public class UnitBaseData
    {
        public int hp;
        public int mp;
        public int mp_red;
        public int mp_green;
        public int mp_blue;
        public int phy_atk;
        public int phy_def;
        public int mag_atk;
        public int mag_def;
        public int dex;
    }

    public class HeroModel : SingletonClass<HeroModel>
    {
        //åªç›ÇÃíl
        public ReactiveProperty<int> Hp = new();
        public ReactiveProperty<float> Mp = new();
        public ReactiveProperty<int> MpRed = new();
        public ReactiveProperty<int> MpGreen = new();
        public ReactiveProperty<int> MpBlue = new();
        public ReactiveProperty<int> Phy_atk = new();
        public ReactiveProperty<int> Phy_def = new();
        public ReactiveProperty<int> Mag_atk = new();
        public ReactiveProperty<int> Mag_def = new();
        public ReactiveProperty<int> Dex = new();

        //äÓëbíl
        private UnitBaseData _baseData = new();
        public UnitBaseData GetBaseData() { return _baseData; }

        //
        public ReactiveProperty<int> NowExp = new(0);
        public ReactiveProperty<int> NextExp = new(16);
        public ReactiveProperty<int> NowLevel = new(1);

        public enum ELE_TYPE
        {
            RED,
            GREEN,
            BLUE,
            TOTAL
        }

        public ReactiveProperty<ELE_TYPE> NOW_ELETYPE = new();

        //to do stage model
        public ReactiveProperty<int> NOW_SCORE = new();
        public ReactiveProperty<bool> IS_GAME_OVER = new();
        public ReactiveProperty<bool> IS_PAUSE = new();

        public Subject<int> HpChangeSubject = new();


        public void SetLevel(int lv)
        {
            var id = PlayerModel.Instance.JobId;
            var bd = MstJobBaseDataModel.GetRecordBy(id);
            var pm = MstJobProgressModel.GetRecordBy(id);

            _baseData.hp = _calcAttrValue(bd.hp, pm.hp, lv);
            _baseData.mp = _calcAttrValue(bd.mp, pm.mp, lv);
            _baseData.phy_atk = _calcAttrValue(bd.phy_atk, pm.phy_atk, lv);
            _baseData.phy_def = _calcAttrValue(bd.phy_def, pm.phy_def, lv);
            _baseData.mag_atk = _calcAttrValue(bd.mag_atk, pm.mag_atk, lv);
            _baseData.mag_def = _calcAttrValue(bd.mag_def, pm.mag_def, lv);
            _baseData.dex = _calcAttrValue(bd.dex, pm.dex, lv);

            Hp.Value = _baseData.hp;
            Mp.Value = _baseData.mp;
            Phy_atk.Value = _baseData.phy_atk;
            Phy_def.Value = _baseData.phy_def;
            Mag_atk.Value = _baseData.mag_atk;
            Mag_def.Value = _baseData.mag_def;
            Dex.Value = _baseData.dex;

            //init rgb//
            _baseData.mp_red = 100;
            _baseData.mp_green = 100;
            _baseData.mp_blue = 100;

            MpRed.Value = 30;
            MpGreen.Value = 30;
            MpBlue.Value = 30;
            NOW_ELETYPE.Value = ELE_TYPE.RED;
        }

        public int _calcAttrValue(int value, int calcRate, int lv)
        {
            if (lv == 1)
                return value;
            int ret = value;
            for (int i = 1; i < lv; ++i)
            {
                var addValue = (int)(value * (calcRate / 100.0f) * 0.1f + 0.5f);
                if (addValue == 0)
                    addValue = 1;
                ret += addValue;
            }

            return ret;
        }

        public void AddExp(int addExp)
        {
            if (NowExp.Value < MstExpModel.MMData.All.Last.needExp)
                SetupExp(NowExp.Value + addExp);
        }

        public void SetupExp(int v)
        {
            int nextExp = 0;
            NowLevel.Value = MstExpModel.CalcExpToLv(v, out nextExp);
            NextExp.Value = v + nextExp;
            NowExp.Value = v;

            SetLevel(NowLevel.Value);

            //todo stage data
            NOW_SCORE.Value = 0;
            IS_GAME_OVER.Value = false;
        }

        //----------------mp work-----------------
        //master date
        public const int WastJumpMp = 20;
        public const int WastStartFlyMp = 5;
        public const int WastFlyPerTime = 50;
        public const int RecvMpPerTime = 200;
        public const int WastShoot = 10;

        public bool WasteJump()
        {
            if (Mp.Value > 0)
            {
                Mp.Value -= WastJumpMp;
                return true;
            }
            return false;
        }

        public bool WasteStartFly()
        {
            if (Mp.Value > 0)
            {
                Mp.Value -= WastStartFlyMp;
                return true;
            }
            return false;
        }

        public bool RecvMpOnLand(float dt)
        {
            if (Mp.Value <= _baseData.mp)
            {
                float tempValue = Mp.Value;
                tempValue += (RecvMpPerTime * dt);
                if (tempValue >= _baseData.mp)
                {
                    Mp.Value = _baseData.mp;
                    return true;
                }
                else
                {
                    Mp.Value = tempValue;
                    return false;
                }
            }
            return true;
        }

        public bool WasteMpFlying(float dt)
        {
            if (Mp.Value > 0)
            {
                Mp.Value -= (WastFlyPerTime * dt);
                return true;
            }
            else
                return false;
        }

        private ReactiveProperty<int> _getEleMp(ELE_TYPE eype)
        {
            switch(eype)
            {
                case ELE_TYPE.RED:
                    return MpRed;
                case ELE_TYPE.GREEN:
                    return MpGreen;
                case ELE_TYPE.BLUE:
                    return MpBlue;
                default:
                    return MpRed;
            }
        }

        private int _getEleBaseMp(ELE_TYPE eype)
        {
            switch (eype)
            {
                case ELE_TYPE.RED:
                    return _baseData.mp_red;
                case ELE_TYPE.GREEN:
                    return _baseData.mp_green;
                case ELE_TYPE.BLUE:
                    return _baseData.mp_blue;
                default:
                    return _baseData.mp_red;
            }
        }

        public bool CanShoot()
        {
            return (_getEleMp(NOW_ELETYPE.Value).Value >= WastShoot);
        }

        public void WasteShoot()
        {
            _getEleMp(NOW_ELETYPE.Value).Value -= WastShoot;
        }

        public void RecorverElmMp(ELE_TYPE etype, int addValue)
        {
            int baseMp = _getEleBaseMp(etype);
            var tgMp = _getEleMp(etype);
            if (tgMp.Value + addValue < baseMp)
                tgMp.Value += addValue;
            else
                tgMp.Value = baseMp;
        }

        public void ChangeHp(int changeVal)
        {
            HpChangeSubject.OnNext(changeVal);
            if ((Hp.Value + changeVal) <= 0)
            {
                Hp.Value = 0;
                IS_GAME_OVER.Value = true;
            }
            else if (Hp.Value + changeVal > _baseData.hp)
                Hp.Value = _baseData.hp;
            else
                Hp.Value += changeVal;
        }
    }
}