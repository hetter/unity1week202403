using Cysharp.Threading.Tasks;
using DummyEgg.ProjectGK.Home;
using DummyEgg.ProjectGK.UiModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DummyEgg.ProjectGK.GameOver
{
    public class GameOverView : UiBasePanel
    {
        [SerializeField] Button _titleButton;
        [SerializeField] public TMP_Text TxtScore;

        public IObservable<Unit> OnClickTitle => _titleButton.OnClickAsObservable();

        public HomeView GoToHomeView()
        {
            PopThisUI();
            //TransitionManager.Instance.LoadSceneWithFade("Main.unity").Forget();
            TransitionManager.Instance.LoadSceneWithFade("RgbColorBattle/Scenes/Main").Forget();            
            var u = UiManagerRoot.Instance;
            return u.UiStackMain.PushUI(u.UiPrefabSo.HomeViewSo);
        }
    }
}
