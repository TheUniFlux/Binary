/*
Copyright (c) 2023 Xavier Arpa López Thomas Peter ('Kingdox')

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
namespace Kingdox.UniFlux.Binary
{
    public sealed class ActionSubscriber<TValue>
    {
        public TValue LastValue { get; private set; }
        private Action<TValue> onInvokeValue;
        private Action onInvoke;
        public ActionSubscriber(
            TValue initValue = default
        ){
            LastValue = initValue;
            onInvokeValue = default;
            onInvoke = default;
        }
        public void Subscribe(bool condition, Action<TValue> callback)
        {
            if (condition) callback.Invoke(LastValue);
            SubscribeWithoutNotify(condition, callback);
        }
        public void SubscribeWithoutNotify(bool condition, Action<TValue> callback){
            if (condition) onInvokeValue += callback;
            else onInvokeValue -= callback;
        }
        public void Subscribe(bool condition, Action callback)
        {
            if (condition) callback.Invoke();
            SubscribeWithoutNotify(condition, callback);
        }
        public void SubscribeWithoutNotify(bool condition, Action callback)
        {
            if (condition) onInvoke += callback;
            else onInvoke -= callback;
        }
        public void Invoke(TValue val)
        {
            LastValue = val;
            Invoke();
        }
        public void Invoke()
        {
            onInvokeValue?.Invoke(LastValue);
            onInvoke?.Invoke();
        }
    }
}