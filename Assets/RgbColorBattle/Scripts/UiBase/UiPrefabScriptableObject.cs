using UnityEngine;

namespace DummyEgg.ProjectGK
{
    [CreateAssetMenu]
    public class UiPrefabScriptableObject : ScriptableObject
    {
        public Title.TitleView TitleViewSo;
        public Home.HomeView HomeViewSo;
        public HeroUi.HeroStatusView HeroStatusViewSo;
        public GameOver.GameOverView GameOverViewSo;
    }
}