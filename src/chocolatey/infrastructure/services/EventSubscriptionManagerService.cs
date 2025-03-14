﻿// Copyright © 2017 - 2025 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using chocolatey.infrastructure.events;
using chocolatey.infrastructure.guards;
using chocolatey.infrastructure.logging;

namespace chocolatey.infrastructure.services
{
    /// <summary>
    ///   Implementation of IEventSubscriptionManagerService
    /// </summary>
    public class EventSubscriptionManagerService : IEventSubscriptionManagerService
    {
        //http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/
        //https://github.com/shiftkey/Reactive.EventAggregator

        private readonly ISubject<object> _subject = new Subject<object>();

        public void Publish<Event>(Event eventMessage) where Event : class, IMessage
        {
            Ensure.That(() => eventMessage).NotNull();

            this.Log().Debug(ChocolateyLoggers.Verbose, () => "Sending message '{0}' out if there are subscribers...".FormatWith(typeof(Event).Name));

            _subject.OnNext(eventMessage);
        }

        public IDisposable Subscribe<Event>(Action<Event> handleEvent, Action<Exception> handleError, Func<Event, bool> filter) where Event : class, IMessage
        {
            if (filter == null)
            {
                filter = (message) => true;
            }

            if (handleError == null)
            {
                handleError = (ex) => { };
            }

            var subscription = _subject.OfType<Event>().AsObservable()
                                       .Where(filter)
                                       .Subscribe(handleEvent, handleError);

            return subscription;
        }

#pragma warning disable IDE0022, IDE1006
        [Obsolete("This overload is deprecated and will be removed in v3.")]
        public void publish<Event>(Event eventMessage) where Event : class, IMessage
            => Publish(eventMessage);

        [Obsolete("This overload is deprecated and will be removed in v3.")]
        public IDisposable subscribe<Event>(Action<Event> handleEvent, Action<Exception> handleError, Func<Event, bool> filter) where Event : class, IMessage
            => Subscribe(handleEvent, handleError, filter);
#pragma warning restore IDE0022, IDE1006
    }
}
