using ET;
using System;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 数据订阅系统
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class DataSubscriptionSystem : Entity, IAwake, IDestroy
    {
        // 存储所有订阅关系
        private Dictionary<string, List<Action<object>>> subscriptions = new Dictionary<string, List<Action<object>>>();
        
        /// <summary>
        /// 订阅属性变化
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="callback">回调函数</param>
        public void Subscribe(long componentId, string propertyName, Action<object> callback)
        {
            if (callback == null)
                return;
                
            string key = GetSubscriptionKey(componentId, propertyName);
            
            if (!subscriptions.ContainsKey(key))
            {
                subscriptions[key] = new List<Action<object>>();
            }
            
            subscriptions[key].Add(callback);
        }
        
        /// <summary>
        /// 取消订阅属性变化
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="callback">回调函数</param>
        public void Unsubscribe(long componentId, string propertyName, Action<object> callback)
        {
            if (callback == null)
                return;
                
            string key = GetSubscriptionKey(componentId, propertyName);
            
            if (subscriptions.ContainsKey(key))
            {
                subscriptions[key].Remove(callback);
                
                if (subscriptions[key].Count == 0)
                {
                    subscriptions.Remove(key);
                }
            }
        }
        
        /// <summary>
        /// 属性值变化时通知所有订阅者
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">新的属性值</param>
        public void NotifyPropertyChanged(long componentId, string propertyName, object value)
        {
            string key = GetSubscriptionKey(componentId, propertyName);
            
            if (subscriptions.ContainsKey(key))
            {
                foreach (Action<object> callback in subscriptions[key])
                {
                    try
                    {
                        callback(value);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Failed to notify subscriber for property {0}: {1}", propertyName, e.Message);
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取订阅键
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>订阅键</returns>
        private string GetSubscriptionKey(long componentId, string propertyName)
        {
            return $"{componentId}_{propertyName}";
        }
        
        public override void Dispose()
        {
            base.Dispose();
            subscriptions.Clear();
        }
    }
    
    /// <summary>
    /// 数据订阅扩展方法
    /// </summary>
    public static class DataSubscriptionExtensions
    {
        /// <summary>
        /// 订阅属性变化
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="callback">回调函数</param>
        public static void Subscribe<T>(this T component, string propertyName, Action<object> callback) where T : Entity, IDataBindable
        {
            Scene scene = component.DomainScene();
            DataSubscriptionSystem subscriptionSystem = scene.GetComponent<DataSubscriptionSystem>();
            
            if (subscriptionSystem != null)
            {
                subscriptionSystem.Subscribe(component.Id, propertyName, callback);
            }
        }
        
        /// <summary>
        /// 取消订阅属性变化
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="callback">回调函数</param>
        public static void Unsubscribe<T>(this T component, string propertyName, Action<object> callback) where T : Entity, IDataBindable
        {
            Scene scene = component.DomainScene();
            DataSubscriptionSystem subscriptionSystem = scene.GetComponent<DataSubscriptionSystem>();
            
            if (subscriptionSystem != null)
            {
                subscriptionSystem.Unsubscribe(component.Id, propertyName, callback);
            }
        }
        
        /// <summary>
        /// 通知属性变化
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">新的属性值</param>
        public static void NotifyPropertyChanged<T>(this T component, string propertyName, object value) where T : Entity, IDataBindable
        {
            Scene scene = component.DomainScene();
            DataSubscriptionSystem subscriptionSystem = scene.GetComponent<DataSubscriptionSystem>();
            DataBindingManager bindingManager = scene.GetComponent<DataBindingManager>();
            
            if (subscriptionSystem != null)
            {
                subscriptionSystem.NotifyPropertyChanged(component.Id, propertyName, value);
            }
            
            if (bindingManager != null)
            {
                bindingManager.NotifyPropertyChanged(component.Id, propertyName, value);
            }
        }
    }
}