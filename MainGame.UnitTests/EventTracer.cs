using System;
using System.Collections.Generic;
using System.Linq;
using CraftingGame;

namespace MainGame.UnitTests
{
    public interface IEventListener<TEventArgs>
    {
        IEventListener<TEventArgs> Action(Action action);
        bool RaisedEvent(Func<TEventArgs, bool> predicat);
    }

    public class EventTracer
    {
        public static IEventListener<TEventArgs> With<TEventArgs>(Action<EventHandler<TEventArgs>> subscribeAction) where TEventArgs : EventArgs
        {
            return new EventListener<TEventArgs>(subscribeAction);
        }
    }

    public class EventListener<TEventArgs> : IEventListener<TEventArgs>
    {
        private EventHandler<TEventArgs> handler;
        private List<TEventArgs> capturedEvents = new List<TEventArgs>();

        internal EventListener(Action<EventHandler<TEventArgs>> subscribeAction)
        {
            handler += (sender, args) => {
                capturedEvents.Add(args);
            };
            subscribeAction(handler);
        }

        public IEventListener<TEventArgs> Action(Action action)
        {
            action();
            return this;
        }

        public bool RaisedEvent(Func<TEventArgs, bool> predicat)
        {
            return capturedEvents.Any(predicat);
        }
    }
}