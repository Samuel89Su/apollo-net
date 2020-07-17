using Com.Ctrip.Framework.Apollo.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// config 扩展
    /// </summary>
    public static class ConfigExtension
    {
        private static ConcurrentDictionary<string, JObject> NamespaceConfigJsonCache = new ConcurrentDictionary<string, JObject>();
        private static ConcurrentDictionary<string, bool> NamespaceChangedSubscribed = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// 完整 properties 名称空间，还原成配置对象
        /// </summary>
        /// <typeparam name="T">配置对象类型</typeparam>
        /// <param name="namespaceName">名称空间名称</param>
        /// <returns></returns>
        public static T GetNamespaceAsEntireConfig<T>(string namespaceName) where T : class
        {
            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }
            var configProvider = ConfigService.GetConfig(namespaceName);
            if (configProvider == null)
            {
                throw new InvalidOperationException($"namespace: {namespaceName} 不存在，请检查配置中心配置。");
            }

            return configProvider.GetNamespaceAsEntireConfig<T>();
        }

        /// <summary>
        /// 完整 properties 名称空间，还原成配置对象
        /// </summary>
        /// <typeparam name="T">配置对象类型</typeparam>
        /// <param name="config">名称空间配置</param>
        /// <returns></returns>
        public static T GetNamespaceAsEntireConfig<T>(this IConfig config) where T : class
        {
            var result = default(T);
            // 优先读取缓存
            if (!string.IsNullOrWhiteSpace(config.Namespace) && NamespaceConfigJsonCache.TryGetValue(config.Namespace, out JObject jObject) && jObject != null)
            {
                result = jObject.ToObject<T>();
            }
            else
            {
                var keys = config.GetPropertyNames();
                if (keys == null || keys.Count == 0)
                {
                    return default(T);
                }

                var keyVals = new Dictionary<string, string>();
                foreach (var key in keys)
                {
                    var val = config.GetProperty(key, null);
                    keyVals[key] = val;
                }

                jObject = InnerConvertProps2JObject<T>(keyVals);

                if (jObject == null)
                {
                    return default(T);
                }

                result = jObject.ToObject<T>();
                // 缓存
                if (result != null)
                {
                    NamespaceConfigJsonCache.TryAdd(config.Namespace, jObject);
                    // 订阅更新事件，用户清理缓存
                    if (!(NamespaceChangedSubscribed.TryGetValue(config.Namespace, out bool subed) && subed))
                    {
                        config.ConfigChanged += new ConfigChangeEvent(OnChanged);
                        NamespaceChangedSubscribed.TryAdd(config.Namespace, true);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="changeEvent"></param>
        private static void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            //  清理缓存
            NamespaceConfigJsonCache.TryRemove(changeEvent.Namespace, out JObject jObject);
        }

        /// <summary>
        /// key -> value 转换为 Dictionary(string, object)
        /// 使用 JObject 做中介
        /// </summary>
        /// <param name="keyVals"></param>
        /// <param name="valueType">value 的类型</param>
        /// <returns></returns>
        internal static JObject InnerConvertProps2Dic(IDictionary<string, string> keyVals, Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            var jObject = new JObject();
            var isConvertable = valueType.IsValueType || valueType == typeof(string);
            var isIEnumerable = valueType.GetInterface(typeof(System.Collections.IEnumerable).Name) != null;
            var isClass = valueType.IsClass;
            foreach (var kv in keyVals)
            {
                if (isConvertable)
                {
                    jObject.Add(kv.Key, kv.Value);
                }
                else if (isIEnumerable)
                {
                    var obj = JsonConvert.DeserializeObject<JArray>(kv.Value);
                    jObject.Add(kv.Key, obj);
                }
                else if (isClass)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(kv.Value);
                    jObject.Add(kv.Key, obj);
                }
            }

            return jObject;
        }

        internal static JObject InnerConvertProps2JObject<T>(IDictionary<string, string> keyVals) where T : class
        {
            if (keyVals == null || keyVals.Count == 0)
            {
                return null;
            }

            JObject jObject = null;
            var type = typeof(T);
            if (type.IsGenericType)
            {
                if (type.GetInterface(typeof(System.Collections.IDictionary).Name) != null)
                {
                    var genericArgs = type.GetGenericArguments();
                    if (genericArgs.Count() == 2 && genericArgs.First() == typeof(string))
                    {
                        jObject = InnerConvertProps2Dic(keyVals, genericArgs[1]);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("不支持 Dictionary<string, T> 以外的泛型对象转换，请使用 InnerConvertProps2Dic 替代。");
                }
            }
            else
            {
                var typeProps = type.GetProperties();
                jObject = InnerConvertProps2JObject(keyVals, typeProps);
            }

            return jObject;
        }

        private static JObject InnerConvertProps2JObject(IDictionary<string, string> keyVals, PropertyInfo[] typeProps)
        {
            if (keyVals == null || keyVals.Count == 0 || typeProps == null || typeProps.Count() == 0)
            {
                return null;
            }

            var jObject = new JObject();
            foreach (var kv in keyVals)
            {
                try
                {
                    var prop = typeProps.FirstOrDefault(f => f.Name.Equals(kv.Key, StringComparison.OrdinalIgnoreCase));
                    if (prop == null)
                    {
                        continue;
                    }
                    if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                    {
                        jObject.Add(prop.Name, kv.Value);
                    }
                    else if (prop.PropertyType.GetInterface(typeof(System.Collections.IEnumerable).Name) != null)
                    {
                        var obj = JsonConvert.DeserializeObject<JArray>(kv.Value);
                        jObject.Add(prop.Name, obj);
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        var obj = JsonConvert.DeserializeObject<JObject>(kv.Value);
                        jObject.Add(prop.Name, obj);
                    }
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"类型转换失败: {kv.Key}，请仔细核对代码中的模型定义与配置项的值。");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return jObject;
        }
    }
}
