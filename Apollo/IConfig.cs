using System;
using Com.Ctrip.Framework.Apollo.Model;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Config change event fired when there is any config change for the namespace.
    /// </summary>
    /// <param name="sender"> the sender </param>
    /// <param name="args"> the changes </param>
    public delegate void ConfigChangeEvent(object sender, ConfigChangeEventArgs args);

    public interface IConfig
    {
        /// <summary>
        /// Config 对应的名称空间
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value </returns>
        string GetProperty(string key, string defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as int </returns>
        int? GetIntProperty(string key, int? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as long </returns>
        long? GetLongProperty(string key, long? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as short </returns>
        short? GetShortProperty(string key, short? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as float </returns>
        float? GetFloatProperty(string key, float? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as double </returns>
        double? GetDoubleProperty(string key, double? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as byte </returns>
        sbyte? GetByteProperty(string key, sbyte? defaultValue);

        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as bool </returns>
        bool? GetBooleanProperty(string key, bool? defaultValue);

        /// <summary>
        /// Return the array property value with the given key, or {@code defaultValue} if the key doesn't
        /// exist.
        /// </summary>
        /// <param name="key"> the property name </param>
        /// <param name="delimiter"> the delimeter regex </param>
        /// <param name="defaultValue"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value as array </returns>
        string[] GetArrayProperty(string key, string delimiter, String[] defaultValue);

        /// <summary>
        /// Return a set of the property names
        /// </summary>
        /// <returns> the property names </returns>
        ISet<string> GetPropertyNames();

        /// <summary>
        /// Config change event subscriber
        /// </summary>
        event ConfigChangeEvent ConfigChanged;

        /// <summary>
        /// 获取的属性值，并转换为列表
        /// 使用 Convert.ChangeType 进行转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">不支持此转换。 - 或 - value 是 null 并且 conversionType 是一个值类型。 - 或 - value 不实现 System.IConvertible 接口。</exception>
        /// <exception cref="System.FormatException">value 的格式不是 conversionType 可识别的格式。</exception>
        /// <exception cref="System.OverflowException">value 表示不在 conversionType 的范围内的数字。</exception>
        /// <exception cref="System.ArgumentNullException">conversionType 为 null。</exception>
        List<T> GetListFromProp<T>(string key, string delimiter) where T : struct;

        #region 扩展 property 类型名称空间中存储 json 的支持

        /// <summary>
        /// 获取 json 格式的属性值，并转换为字符串列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<string> GetListFromJsonFormattedProp(string key);

        /// <summary>
        /// 获取 json 格式的属性值，并转换为指定的泛型对象
        /// 使用 Json.net 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetTypedFromJsonFormattedProp<T>(string key) where T : class;

        #endregion
    }
}

