﻿// ****************************************************************************
// <copyright file="WeakEventSubscription.cs" company="Pedro Lamas">
// Copyright © Pedro Lamas 2014
// </copyright>
// ****************************************************************************
// <author>Pedro Lamas</author>
// <email>pedrolamas@gmail.com</email>
// <project>Cimbalino.Toolkit.Core</project>
// <web>http://www.pedrolamas.com</web>
// <license>
// See license.txt in this solution or http://www.pedrolamas.com/license_MIT.txt
// </license>
// ****************************************************************************

using System;
using System.Reflection;
using Windows.Foundation;

namespace Cimbalino.Toolkit.Helpers
{
    /// <summary>
    /// Stores a <see cref="TypedEventHandler{T1, T2}" /> without causing a hard reference to be created to the event handler's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    public class WeakEventSubscription<T1, T2> : IDisposable 
        where T2 : EventArgs
    {
        private Action<TypedEventHandler<T1, T2>> _unsubscribeAction;

        /// <summary>
        /// Gets or sets a <see cref="WeakReference"/> to the target <see cref="TypedEventHandler{T1, T2}"/>.
        /// </summary>
        /// <value>A <see cref="WeakReference"/> to the target <see cref="TypedEventHandler{T1, T2}"/>.</value>
        protected WeakReference EventHandlerReference { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="WeakReference"/> to the target of this <see cref="WeakEventSubscription{T1, T2}"/>.
        /// </summary>
        /// <value>A <see cref="WeakReference"/> to the target of this <see cref="WeakEventSubscription{T1, T2}"/>.</value>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this <see cref="WeakEventSubscription{T1, T2}"/>.
        /// </summary>
        /// <value>The <see cref="MethodInfo" /> corresponding to this <see cref="WeakEventSubscription{T1, T2}"/>.</value>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets a value indicating whether the event handler's owner is still alive (not yet collected by the Garbage Collector).
        /// </summary>
        /// <value>true if event handler's owner is still alive; otherwise, false.</value>
        public virtual bool IsAlive => Reference != null && Reference.IsAlive;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSubscription{T1, T2}" /> class.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="subscribeAction">The subscribe action.</param>
        /// <param name="unsubscribeAction">The unsubscribe action.</param>
        public WeakEventSubscription(TypedEventHandler<T1, T2> eventHandler, Action<TypedEventHandler<T1, T2>> subscribeAction, Action<TypedEventHandler<T1, T2>> unsubscribeAction)
            : this(eventHandler.Target, eventHandler, subscribeAction, unsubscribeAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSubscription{T1, T2}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="subscribeAction">The subscribe action.</param>
        /// <param name="unsubscribeAction">The unsubscribe action.</param>
        public WeakEventSubscription(object target, TypedEventHandler<T1, T2> eventHandler, Action<TypedEventHandler<T1, T2>> subscribeAction, Action<TypedEventHandler<T1, T2>> unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;

            Method = eventHandler.GetMethodInfo();
            EventHandlerReference = new WeakReference(eventHandler.Target);
            Reference = new WeakReference(target);

            subscribeAction(OnEvent);
        }

        private void OnEvent(T1 sender, T2 e)
        {
            if (IsAlive)
            {
                var eventHandlerTarget = EventHandlerReference.Target;

                if (Method != null && eventHandlerTarget != null)
                {
                    Method.Invoke(eventHandlerTarget, new object[] { sender, e });
                }
            }
            else
            {
                Dispose();
            }
        }

        #region IDisposable Interface

        /// <summary>
        /// Disposes this instance and releases all references.
        /// </summary>
        public void Dispose()
        {
            _unsubscribeAction?.Invoke(OnEvent);

            _unsubscribeAction = null;
            Reference = null;
            EventHandlerReference = null;
            Method = null;

            GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// Stores an <see cref="EventHandler" /> without causing a hard reference to be created to the event handler's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakEventSubscription : IDisposable
    {
        private Action<EventHandler> _unsubscribeAction;

        /// <summary>
        /// Gets or sets a <see cref="WeakReference"/> to the target <see cref="EventHandler"/>.
        /// </summary>
        /// <value>A <see cref="WeakReference"/> to the target <see cref="EventHandler"/>.</value>
        protected WeakReference EventHandlerReference { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="WeakReference"/> to the target of this <see cref="WeakEventSubscription"/>.
        /// </summary>
        /// <value>A <see cref="WeakReference"/> to the target of this <see cref="WeakEventSubscription"/>.</value>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this <see cref="WeakEventSubscription"/>.
        /// </summary>
        /// <value>The <see cref="MethodInfo" /> corresponding to this <see cref="WeakEventSubscription"/>.</value>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets a value indicating whether the event handler's owner is still alive (not yet collected by the Garbage Collector).
        /// </summary>
        /// <value>true if event handler's owner is still alive; otherwise, false.</value>
        public virtual bool IsAlive
        {
            get
            {
                return Reference != null && Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSubscription"/> class.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="subscribeAction">The action to subscribe the event handler.</param>
        /// <param name="unsubscribeAction">The action to unsubscribe the event handler.</param>
        public WeakEventSubscription(EventHandler eventHandler, Action<EventHandler> subscribeAction, Action<EventHandler> unsubscribeAction)
            : this(eventHandler.Target, eventHandler, subscribeAction, unsubscribeAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSubscription"/> class.
        /// </summary>
        /// <param name="target">The event handler's owner.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="subscribeAction">The action to subscribe the event handler.</param>
        /// <param name="unsubscribeAction">The action to unsubscribe the event handler.</param>
        public WeakEventSubscription(object target, EventHandler eventHandler, Action<EventHandler> subscribeAction, Action<EventHandler> unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;

            Method = eventHandler.GetMethodInfo();
            EventHandlerReference = new WeakReference(eventHandler.Target);
            Reference = new WeakReference(target);

            subscribeAction(OnEvent);
        }

        private void OnEvent(object sender, EventArgs e)
        {
            if (IsAlive)
            {
                var eventHandlerTarget = EventHandlerReference.Target;

                if (Method != null && eventHandlerTarget != null)
                {
                    Method.Invoke(eventHandlerTarget, new[] { sender, e });
                }
            }
            else
            {
                Dispose();
            }
        }

        #region IDisposable Interface

        /// <summary>
        /// Disposes this instance and releases all references.
        /// </summary>
        public void Dispose()
        {
            if (_unsubscribeAction != null)
            {
                _unsubscribeAction(OnEvent);
            }

            _unsubscribeAction = null;
            Reference = null;
            EventHandlerReference = null;
            Method = null;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}