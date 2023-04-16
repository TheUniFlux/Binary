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
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Kingdox.UniFlux;
namespace Kingdox.UniFlux.Binary
{
    public sealed partial class Binary_Module : MonoFlux
    {
        private const string PATH_PROTECTION = "saved";
        private string path;
        private readonly StringBuilder _buildedPath = new StringBuilder();
        private readonly Dictionary<string, ActionSubscriber<object>> dic_data = new Dictionary<string, ActionSubscriber<object>>();
        private BinaryFormatter _formatter = new BinaryFormatter();
        private Action onSave = default;
        private void Awake()
        {
            SetPath(Application.persistentDataPath);
        }
#if UNITY_EDITOR
        [Header("DEBUG")]
        [Space]
        [SerializeField] private string _current_path = "";
        [SerializeField] private List<string> _list_debug = new List<string>();
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            _current_path = path;
            _list_debug.Clear();
            foreach (var item in dic_data)
            {
                _list_debug.Add($"[{item.Key}]: {item.Value.LastValue}");
            }
        }
#endif
        private StringBuilder _BuildPath(in string _name) => _buildedPath.Clear().Append(path).Append(_name);
        private bool _Exist_File(in string _name) => File.Exists(_BuildPath(_name).ToString());
        private bool _Exist_Dictionary(in string _name) => dic_data.ContainsKey(_name);
        private void _Delete(in string _name) => File.Delete(_BuildPath(_name).ToString());
        private void _Register(in string _name) => dic_data.Add(_name, new ActionSubscriber<object>(default));
        private void _Register(in string _name, in object _value) => dic_data.Add(_name, new ActionSubscriber<object>(_value));
        private void _Save_Dictionary(in string _name, in object _value)
        {
            if (!_Exist_Dictionary(_name))
            {
                _Register(_name,_value);
            }
            dic_data[_name].Invoke(_value);
            onSave?.Invoke();
        }
        [Flux(BinaryService.Key.SetPath)] private void SetPath(string _path)
        {
            _path = $"{_path}/{PATH_PROTECTION}/";
            path = _path;
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
        }
        [Flux(BinaryService.Key.Save)] private void Save((string _name, object _value) _data)
        {
            using (FileStream _stream = new FileStream(_BuildPath(_data._name).ToString(), FileMode.Create)){
               _formatter.Serialize(_stream, _data._value);
            }
            _Save_Dictionary(_data._name , _data._value);
        }
        [Flux(BinaryService.Key.Save)] private void Save(Type _type)
        {
            var _fields = _type.GetFields();
            for (int i = 0; i < _fields.Length; i++)
            {
                Save((_fields[i].Name, _fields[i].GetValue(null)));
            }
        }
        [Flux(BinaryService.Key.Load)] private object Load(string _name)
        {
            
            if (dic_data.TryGetValue(_name, out ActionSubscriber<object> _val))  
            {
                return _val.LastValue;
            }
            else if (_Exist_File(_name))
            {
                using (FileStream _stream = new FileStream(_BuildPath(_name).ToString(), FileMode.Open))
                {
                    object storedData = _formatter.Deserialize(_stream);
                    _Register(_name, storedData);
                    return storedData;
                }
            }
            else
            {
                _Register(_name);
#if UNITY_EDITOR
                Debug.LogWarning($"No hay datos de `{_name}` para cargar");
#endif
                return null;
            }
        }
        [Flux(BinaryService.Key.Subscribe)] private void Subscribe((bool _condition, string _name, Action _callback) _data)
        {
            if (!_Exist_Dictionary(_data._name))
            {
                _Register(_data._name);
                dic_data[_data._name].SubscribeWithoutNotify(_data._condition, _data._callback);
            }
            else
            {
                dic_data[_data._name].Subscribe(_data._condition, _data._callback);
            }
        }
        [Flux(BinaryService.Key.Subscribe)] private void Subscribe((bool _condition, string _name, Action<object> _callback) _data)
        {
            if (!_Exist_Dictionary(_data._name))
            {
                _Register(_data._name);
                dic_data[_data._name].SubscribeWithoutNotify(_data._condition, _data._callback);
            }
            else
            {
                dic_data[_data._name].Subscribe(_data._condition, _data._callback);
            }
        }
        [Flux(BinaryService.Key.Subscribe)] private void Subscribe((bool _condition, Action _callback) _data)
        {
            if (_data._condition)
            {
                onSave += _data._callback;
            }
            else
            {
                onSave -= _data._callback;
            }
        }
        [Flux(BinaryService.Key.Delete)] private void Delete()
        {   
            if (Directory.Exists(path)) Directory.Delete(path, true);
            dic_data.Clear();
            SetPath(Application.persistentDataPath);
        }
        [Flux(BinaryService.Key.Load)] private void Load(Type _type)
        {
            var _fields = _type.GetFields();
            var _dictionary = new Dictionary<string, object>();
            for (int i = 0; i < _fields.Length; i++)
            {
                _dictionary.Add(_fields[i].Name, Load(_fields[i].Name));
                if (_dictionary[_fields[i].Name] is null)
                {
                    Save((_fields[i].Name, _fields[i].GetValue(null)));
                    _dictionary.Remove(_fields[i].Name);
                }
                else
                {
                    _fields[i].SetValue(null, _dictionary[_fields[i].Name]);
                }
            }
        }
    }
}
