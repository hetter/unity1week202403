﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DummyEgg.ProjectGK.UiModel
{
	public abstract class UiBasePanel : MonoBehaviour {
		public UiStack uiStack { get; private set; } = null;
		public bool IsOnSite { get; set; } = false;

        private Action _hideAction = null;
		private bool _bShowAfterInit = true;

		#region "Life Circle"
		public virtual void OnInitUI() {}
		public virtual void OnDestroyUI() {}
		public virtual void OnStartUI() {}
		public virtual float OnShowUI() { return 0; }
		public virtual float OnHideUI() { return 0; }
		public virtual void OnTopUI() {}
		public virtual void OnTopBackUI() {}
		public virtual void OnTopLostUI() {}
		#endregion

		#region "Function"
		public void ShowUI() {
			float delay = OnShowUI();
			if (delay > 0) {
				gameObject.SetActive(false);
				StartDelayAction(delay, () => {
					gameObject.SetActive(true);
				});
			} else {
				gameObject.SetActive(true);
			}
		}

		public void HideUI() {
			float delay = OnHideUI();
			if (delay > 0) {
				StartDelayAction(delay, () => {
					gameObject.SetActive(false);
					if (_hideAction != null) {
						_hideAction.Invoke();
					}
				});
			} else {
				gameObject.SetActive(false);
				if (_hideAction != null) {
					_hideAction.Invoke();
				}
			}
		}
		public void SetShowAfterInit(bool e) {
			_bShowAfterInit = e;
		}

		public void PopThisUI() {
			uiStack.PopUI(this);
		}
		public void PopTopUI() {
			uiStack.PopUI();
		}

		public void StartDelayAction(float second, Action action) {
			StartCoroutine(_Wait(second, action));
		}

		protected GameObject InstantiateUI(GameObject prefab) {
			var obj = Instantiate(prefab, transform);
			obj.transform.localPosition = Vector3.zero;
			return obj;
		}
		#endregion

		#region "Don't call it manually"
		public void __InitUiStack(UiStack stack) {
			uiStack = stack;
		}

		public void __SetHideListener(Action action) {
			_hideAction = action;
		}
		private IEnumerator _Wait(float sec, Action action) {
			yield return new WaitForSeconds(sec);
			if (action != null) {
				action.Invoke();
			}
		}
		private void Start() {
			if (_bShowAfterInit) {
				ShowUI();
			} else {
				gameObject.SetActive(false);
			}
			OnStartUI();
		}
		#endregion
	}}