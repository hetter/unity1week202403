using System;
using System.Collections.Generic;
using DummyEgg.ProjectGK.UiModel;
using UnityEngine;
using UnityEngine.UI;

namespace DummyEgg.ProjectGK
{
    public class UiManagerRoot : Singleton<UiManagerRoot>
    {
        [SerializeField] public UnityEngine.Camera UiCamera;

        //prefrebs(s)
        [SerializeField] public UiPrefabScriptableObject UiPrefabSo;

        [SerializeField] GameObject UiNodeBack;
        [SerializeField] GameObject UiNodeMid;
        [SerializeField] GameObject UiNodeTop;
        [SerializeField] public UiStack UiStackMain;

        [SerializeField] GameObject WaitViewMaskView;

        public void PushTitle()
        {
            UiStackMain.PushUI(UiPrefabSo.TitleViewSo);
        }

        public void PushUi(UiBasePanel ViewPrefab)
        {
            UiStackMain.PushUI(ViewPrefab);
        }

        public void PopupDlg(string title, string content = "", Action onOk = null)
        {

        }

        public void PushHud()
        {

        }

        private HashSet<int> waitViewIdSets = new HashSet<int>();
        private int idCount = 0;
        public int ShowWaitView(bool isDefault = true)
        {
            int id = -1;
            if (!isDefault)
                id = idCount++;
            waitViewIdSets.Add(id);
            WaitViewMaskView.SetActive(true);
            return id;
        }


        public void CloseWaitView(int id = -1)
        {
            waitViewIdSets.Remove(id);
            if(waitViewIdSets.Count == 0)
                WaitViewMaskView.SetActive(false);
        }
    }
}
