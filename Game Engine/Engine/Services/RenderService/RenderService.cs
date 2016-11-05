﻿using Game_Engine.Engine.Services.RenderSystem;
using Game_Engine.Objects;
using Game_Engine.Services.RenderService.Configs;
using Game_Engine.Services.RenderService.Internals;
using Game_Engine.Services.ServiceManager.ServiceMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine.Services.RenderService
{
    internal class RenderService : Service
    {
        Renderer _Renderer;
        Window _Window;
        List<object> renderBuf = new List<object>();

        internal override void Init()
        {
            base.Init();
            Message.On("append-buffer", new MessageAct(appendBuffer));
            Message.On("set-config", new MessageAct(setConfig));
        }

        void appendBuffer(params object[] o)
        {
            foreach(object nO in o)
            {
                if(_Renderer.isCorrectRenderContract(nO))
                {
                    renderBuf.Add(nO);
                }
            }
        }

        void setConfig(object[] o)
        {
            SetConfig(new RendererConfigs((Type)o[0]));
        }

        public void SetConfig(RendererConfigs conf)
        {
            _Renderer = new Renderer(conf);
        }

        public void Start()
        {
            if (_Renderer == null)
            {
                Logman.Logger.Log(Logman.LogLevel.Errors, "Renderer configs must be set before trying to start the service.");
                return;
            }
        }

        internal override void UpdateService(double delta)
        {
            _Renderer.Update(delta, renderBuf.ToArray());
            renderBuf.Clear();
        }
    }
}