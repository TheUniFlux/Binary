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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdox.UniFlux;
namespace Kingdox.UniFlux.Binary
{
    public static partial class BinaryService // Data
    {
        private static partial class Data
        {

        }
    }
    public static partial class BinaryService // Key
    {
        public static partial class Key
        {
            private const string _BinaryService =  nameof(BinaryService) + ".";
            public const string SetPath = _BinaryService + nameof(SetPath);
            public const string Save = _BinaryService + nameof(Save);
            public const string Load = _BinaryService + nameof(Load);
            public const string Delete = _BinaryService + nameof(Delete);
            public const string Subscribe = _BinaryService + nameof(Subscribe);
        }
    }
    public static partial class BinaryService // Methods
    {
        public static void SetPath(in string data) => Key.SetPath.Dispatch(data);
        public static object Load(in string data) => Key.Load.Dispatch<string, object>(data);
        public static void Load(in Type data) => Key.Load.Dispatch(data);
        public static void Save(in (string name, object value) data) => Key.Save.Dispatch(data);
        public static void Save(in Type data) => Key.Save.Dispatch(data);
        public static void Delete() => Key.Delete.Dispatch();
        public static void Subscribe(in (bool condition, string name, Action callback) data) => Key.Subscribe.Dispatch(data);
        public static void Subscribe(in (bool condition, string name, Action<object> callback) data) => Key.Subscribe.Dispatch(data);
        public static void Subscribe(in (bool condition, Action callback) data) => Key.Subscribe.Dispatch(data);
    }
}