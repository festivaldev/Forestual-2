﻿using System.Collections.Generic;
using F2Core;
using F2Core.Extension;

namespace Forestual2CS.Management
{
    public class ListenerManager
    {
        public static List<Listener> Listeners = new List<Listener>();

        public static void RegisterListener(Listener listener) {
            Listeners.Add(listener);
        }

        public static void InvokeEvent(Event e, params object[] args) {
            try {
                Listeners.FindAll(l => l.Event == e).ForEach(l => l.Delegate.DynamicInvoke(args));
            } catch { }
        }

        public static void InvokeSpecialEvent(EventArguments e) {
            try {
                Listeners.FindAll(l => l.Event == Event.Dynamic).ForEach(l => l.Delegate.DynamicInvoke(e));
            } catch { }
        }
    }
}
