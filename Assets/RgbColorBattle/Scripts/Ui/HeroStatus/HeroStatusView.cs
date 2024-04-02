using Cysharp.Threading.Tasks;
using DummyEgg.ProjectGK.UiModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DummyEgg.ProjectGK.HeroUi
{
    public class HeroStatusView : UiBasePanel
    {
        [SerializeField] Button _closeButton;
        [SerializeField] Button _addExpButton;
        [SerializeField] TMP_InputField _inputAddExp;

        [SerializeField] public TMP_Text Txt_level;
        [SerializeField] public TMP_Text Txt_exp;

        [SerializeField] public TMP_Text Txt_name;
        [SerializeField] public TMP_Text Txt_job;
        [SerializeField] public TMP_Text Txt_hp;
        [SerializeField] public TMP_Text Txt_mp;
        [SerializeField] public TMP_Text Txt_phyAtk;
        [SerializeField] public TMP_Text Txt_phyDef;
        [SerializeField] public TMP_Text Txt_magAtk;
        [SerializeField] public TMP_Text Txt_magDef;
        [SerializeField] public TMP_Text Txt_dex;

        public IObservable<Unit> OnClickClose => _closeButton.OnClickAsObservable();

        public IObservable<Unit> OnClickAddExp => _addExpButton.OnClickAsObservable();

        public void DoClose()
        {
            PopThisUI();
        }

        public int GetAddExp()
        {
            try
            {
                return int.Parse(_inputAddExp.text);
            }
            catch
            {
                return 0;
            }
        }
    }
}
