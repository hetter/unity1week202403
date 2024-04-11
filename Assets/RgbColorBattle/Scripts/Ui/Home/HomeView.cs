using Cysharp.Threading.Tasks;
using DummyEgg.ProjectGK.UiModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DummyEgg.ProjectGK.Home
{
    public class HomeView : UiBasePanel
    {
        [SerializeField] public TMP_Text TxtScore;

        //[SerializeField] Button _heroIconButton;

        //[SerializeField] Button _tileButton;

        [SerializeField] public BaseProgressBar HpBar;

        [SerializeField] public BaseProgressBar EnBar;

        [SerializeField] public BaseProgressBar[] ElementBars;

        [SerializeField] GameObject ImgElementSel;

        [SerializeField] GameObject UiPause;

        private int _nowSelect = 0;

        //[SerializeField] public BaseProgressBar ExpBar;

        //public IObservable<Unit> OnClickHeroIcon => _heroIconButton.OnClickAsObservable();

        //public IObservable<Unit> OnClickTile => _tileButton.OnClickAsObservable();

        public void OpenHeroStatus()
        {
            var u = UiManagerRoot.Instance;
            u.PushUi(u.UiPrefabSo.HeroStatusViewSo);
        }

        public void BackToTitle()
        {
            PopThisUI();
            //TransitionManager.Instance.LoadSceneWithFade("Title.unity").Forget();
            TransitionManager.Instance.LoadSceneWithFade("RgbColorBattle/Scenes/Title").Forget();
        }

        public void OnSelectElement(int inx)
        {
            if (ElementBars == null || ElementBars.Length <= inx)
                return;

            _nowSelect = inx;
            ImgElementSel.transform.position = ElementBars[_nowSelect].transform.position;
        }

        public void ShowPauseUi(bool isPause)
        {
            UiPause.SetActive(isPause);
        }
    }
}
