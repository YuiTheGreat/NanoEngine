﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game_Engine.Services.ServiceManager.ServiceMessage;
using Game_Engine.Engine.Core;

namespace Game_Engine.Engine.Services.ServiceManager
{
    internal class ServiceRoot
    {
        List<Service> _Services;
        MessageRoot Message;
        bool servicesLoaded;
        public ServiceRoot()
        {
            _Services = new List<Service>();
            Message = new MessageRoot(null);
            InitializeServices();
        }

        public void SetRenderConfig(Type renderConf)
        {
            GetService<Render.RenderService>().Message.Receive("set-config", null, renderConf);
        }

        void InitializeServices()
        {

            var serviceSubclasses =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(Service))
                select type;

            List<PrioritizedService> srvcList = new List<PrioritizedService>();
            foreach (Type t in serviceSubclasses)
            {
                PrioritizedService pS;
                if (BootPriority.serviceInitPriority.ContainsKey(t))
                {
                    pS = new PrioritizedService(t, BootPriority.serviceInitPriority[t]);
                }
                else
                {
                    pS = new PrioritizedService(t, 0);
                }
                srvcList.Add(pS);
            }

            srvcList = srvcList.OrderByDescending(p => p.P).ToList();

            foreach (PrioritizedService t in srvcList)
            {
                Logman.Logger.Log(Logman.LogLevel.Info, "Loading service: " + t.T.Name);
                Service srvc = (Service)Activator.CreateInstance(t.T);
                srvc.Priority = t.P;
                srvc.SrvcRoot = this;
                srvc.Message._SrvcRoot = this;
                _Services.Add(srvc);
                Logman.Logger.Log(Logman.LogLevel.Info, "Service loaded: " + t.T.Name);
            }
            Logman.Logger.Log(Logman.LogLevel.Info, "DONE LOADING SERVICES");

            foreach (Service s in _Services)
            {
                s.Init();
                Logman.Logger.Log(Logman.LogLevel.Debug, s.GetType().Name + " initialized");
            }
            Logman.Logger.Log(Logman.LogLevel.Debug, "DONE INITIALIZING SERVICES");
            servicesLoaded = true;
        }
        bool updateStarted;
        internal void UpdateServices(double delta)
        {
            if (!servicesLoaded)
                return;
            if (!updateStarted)
            {
                updateStarted = true;
                Logman.Logger.Log(Logman.LogLevel.Debug, "Done, starting update");
            }
            foreach (Service srvc in _Services)
            {
                srvc.Update(delta);
            }
        }

        public T GetService<T>() where T : Service
        {
            foreach (Service srvc in _Services)
            {
                if (typeof(T) == srvc.GetType())
                    return (T)srvc;
            }
            throw new ArgumentException();
        }

        public Service GetService(Type T)
        {
            foreach (Service srvc in _Services)
            {
                if (T == srvc.GetType())
                    return srvc;
            }
            throw new ArgumentException();
        }
    }

    struct PrioritizedService
    {
        public Type T;
        public int P;

        public PrioritizedService(Type a, int b)
        {
            T = a;
            P = b;
        }
    }
}
