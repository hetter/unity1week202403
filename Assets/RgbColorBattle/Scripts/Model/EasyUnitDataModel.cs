using DummyEgg.ProjectGK.MasterData;
using UniRx;

namespace DummyEgg.ProjectGK.Model
{
    //public class UnitModel
    //{
    //    public ReactiveProperty<uint> _maxHP = new ();
    //    public ReactiveProperty<uint> _nowHP = new();
    //}
    public class EasyUnitDataModel
    {
        public ReactiveProperty<int> Hp = new();
        public ReactiveProperty<int> Phy_atk = new();
        public ReactiveProperty<HeroModel.ELE_TYPE> Now_ShieldType = new();

        //Šî‘b’l
        public int BaseHp = 50;
        public int BaseAtk = 10;

        public void Setup()
        {
            Hp.Value = BaseHp;
            Phy_atk.Value = BaseAtk;
        }

        public void ChangeHp(int changeVal)
        {
            if (Hp.Value + changeVal < 0)
                Hp.Value = 0;
            else if (Hp.Value + changeVal > BaseHp)
                Hp.Value = BaseHp;
            else
                Hp.Value += changeVal;
        }
    }
}