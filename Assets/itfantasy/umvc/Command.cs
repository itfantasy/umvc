﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

namespace itfantasy.umvc
{
    public class Command : IDisposable
    {
        public const int SystemIndex = 0;

        public const int Monitor_Inited = 101;
        public const int Monitor_Showed = 102;
        public const int Monitor_Closed = 103;
        public const int Monitor_Disposed = 104;
        public const int Monitor_Clicked = 105;

        public const int System_SceneChange = 201;
        
        public const int Command_Reactive = 1001;
        public const int Command_Show = 1002;
        public const int Command_Close = 1003;
        public const int Command_OK = 1004;
        public const int Command_Trigger = 1005;
        public const int Command_Cancel = 1006;

        protected Mediator _mediator;

        public bool isActive
        {
            get
            {
                return _mediator != null && 
                    _mediator.gameObject != null && 
                    _mediator.gameObject.activeSelf;
            }
        }

        public bool isDispose
        {
            get
            {
                return _mediator == null || _mediator.gameObject == null;
            }
        }

        private bool _isRegisted;
        public bool isRegisted
        {
            get
            {
                return _isRegisted;
            }
        }

        private string _sceneName;
        public string sceneName
        {
            get
            {
                return _sceneName;
            }
        }

        public object token { get; set; }

        protected T RegisterMediator<T>(GameObject go, bool monitor=true) where T : Mediator
        {
            if (_mediator == null)
            {
                _mediator = go.GetComponent<T>();
                if (_mediator == null)
                {
                    _mediator = go.AddComponent<T>();
                }
                _mediator.SignCommand(this, monitor);
            }
            _mediator.Show();
            _sceneName = Facade.curSceneName;
            _isRegisted = true;
            return _mediator as T;
        }

        protected void RemoveMediator(bool dispose=false)
        {
            if (_mediator != null)
            {
                _mediator.Close(dispose);
            }
            _sceneName = "";
            _isRegisted = false;
        }

        protected void UpdateMediator()
        {
            if (_mediator != null)
            {
                _mediator.UpdateViewContent();
            }
        }

        protected void SendNotice(int cmdIndex, int noticeType, params object[] body)
        {
            Facade.SendNotice(cmdIndex, noticeType, body);
        }

        protected void SendAsyncNotice(int cmdIndex, int noticeType, Action<INotice> callback, object token, params object[] body)
        {
            Facade.SendAsyncNotice(cmdIndex, noticeType, callback, token, body);
        }

        protected void SendNotice(int noticeType, params object[] body)
        {
            Notice notice = new Notice(noticeType, body);
            SendNotice(notice);
        }

        protected void SendNotice(INotice notice)
        {
            if (_mediator != null)
            {
                _mediator.HandleNotice(notice);
            }
        }

        protected void BroadNotice(int noticeType, params object[] body)
        {
            Facade.BroadNotice(noticeType, body);
        }

        private List<INotice> _noticeList = new List<INotice>();

        public void InsertNotice(INotice notice)
        {
            _noticeList.Add(notice);
        }

        protected void PushNotice(int cmdIndex, int noticeType, params object[] body)
        {
            Facade.PushNotice(cmdIndex, noticeType, body);
        }

        protected bool PopNotice(int noticeType=0)
        {
            if (_noticeList.Count > 0)
            {
                INotice target = null;
                foreach (INotice notice in _noticeList)
                {
                    if (notice.GetType() == noticeType || noticeType == 0)
                    {
                        target = notice;
                        break;
                    }
                }
                if (target != null)
                {
                    _noticeList.Remove(target);
                    Execute(target);
                    return true;
                }
            }
            return false;
        }

        public virtual void Execute(INotice notice) { }

        public virtual void Dispose()
        {
            _noticeList.Clear();
        }
    }
}
