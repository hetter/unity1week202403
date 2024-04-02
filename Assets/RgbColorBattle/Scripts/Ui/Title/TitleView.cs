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

namespace DummyEgg.ProjectGK.Title
{
    public class TitleView : UiBasePanel
    {
        [SerializeField] Button _loginButton;
        [SerializeField] TMP_InputField _inputName;
        [SerializeField] public TMP_Dropdown Dp_job;

        public IObservable<Unit> OnClickLogin => _loginButton.OnClickAsObservable();

        public HomeView GoToHomeView()
        {
            PopThisUI();
            //TransitionManager.Instance.LoadSceneWithFade("Main.unity").Forget();
            TransitionManager.Instance.LoadSceneWithFade("RgbColorBattle/Scenes/Main").Forget();
            var u = UiManagerRoot.Instance;
            return u.UiStackMain.PushUI(u.UiPrefabSo.HomeViewSo);
        }

        public string GetInputName()
        {
            return _inputName.text;
        }
    }
}
