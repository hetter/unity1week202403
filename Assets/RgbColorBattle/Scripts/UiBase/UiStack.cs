using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DummyEgg.ProjectGK.UiModel
{
    public class UiStack : MonoBehaviour
    {
        private LinkedList<UiBasePanel> _modelList = new LinkedList<UiBasePanel>();

        [SerializeField]
        private GameObject _root = null;

        private void Awake()
        {
            if (!_root)
            {
                _root = gameObject;
            }
        }
        private void OnDestroy()
        {
            var p = _modelList.Last;
            while (p != null)
            {
                p.Value.OnDestroyUI();
                p = p.Previous;
            }
        }

        public TModel PushUIOnSite<TModel>(TModel model) where TModel : UiBasePanel
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return null;
#endif
            if (_modelList.Count > 0)
            {
                _modelList.Last.Value.OnTopLostUI();
            }
            GameObject modelObj = model.gameObject;

            model.IsOnSite = true;

            model.__InitUiStack(this);
            model.OnInitUI();

            _modelList.AddLast(model);
            model.OnTopUI();
            return model;
        }

        public TModel PushUI<TModel>(TModel modelPrefab, GameObject parent = null) where TModel : UiBasePanel
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return null;
#endif
            if (_modelList.Count > 0)
            {
                _modelList.Last.Value.OnTopLostUI();
            }

            if (parent == null)
                parent = _root;

            var model = Instantiate(modelPrefab, parent.transform);

            model.__InitUiStack(this);
            model.OnInitUI();

            _modelList.AddLast(model);
            model.OnTopUI();
            return model;
        }


        public void PopUI()
        {
            if (_modelList.Last != null)
            {
                var model = _modelList.Last.Value;
                _modelList.RemoveLast();
                _InvokeRemove(model, true);
            }
        }


        public void PopAll()
        {
            while (_modelList.Count != 0)
            {
                if (_modelList.Last != null)
                {
                    var model = _modelList.Last.Value;
                    _modelList.RemoveLast();
                    _InvokeRemove(model, true);
                }
            }
        }

        public void PopUI(UiBasePanel model)
        {
            if (model)
            {
                if (GetTopUI() == model)
                {
                    _modelList.RemoveLast();
                    _InvokeRemove(model, true);
                }
                else
                {
                    var node = _modelList.Find(model);
                    if (node != null)
                    {
                        _modelList.Remove(node);
                        _InvokeRemove(model, false);
                    }
                }
            }
        }

        public UiBasePanel GetTopUI()
        {
            if (_modelList.Last != null)
            {
                return _modelList.Last.Value;
            }
            return null;
        }

        public void _InvokeRemove(UiBasePanel model, bool bTop)
        {
            if (bTop)
            {
                var topUI = GetTopUI();
                if (topUI)
                {
                    topUI.OnTopUI();
                    topUI.OnTopBackUI();
                }
            }

            model.__SetHideListener(() =>
            {
                model.OnDestroyUI();
                if (model.IsOnSite)
                    model.gameObject.SetActive(false);
                else
                    Destroy(model.gameObject);
            });
            model.HideUI();
        }

    };
}
