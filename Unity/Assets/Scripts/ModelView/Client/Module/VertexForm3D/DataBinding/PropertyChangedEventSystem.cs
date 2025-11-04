using ET;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ET.Client
{
    /// <summary>
    /// 属性变化事件系统
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class PropertyChangedEventSystem : Entity, IAwake, IDestroy
    {
        // 存储所有可观察组件
        private Dictionary<long, ObservableComponent> observableComponents = new Dictionary<long, ObservableComponent>();
        
        /// <summary>
        /// 添加可观察组件
        /// </summary>
        /// <param name="component">可观察组件</param>
        public void AddObservableComponent(Entity component)
        {
            if (component == null)
                return;
                
            if (!observableComponents.ContainsKey(component.Id))
            {
                ObservableComponent observable = new ObservableComponent(component);
                observableComponents[component.Id] = observable;
            }
        }
        
        /// <summary>
        /// 移除可观察组件
        /// </summary>
        /// <param name="component">可观察组件</param>
        public void RemoveObservableComponent(Entity component)
        {
            if (component == null)
                return;
                
            if (observableComponents.ContainsKey(component.Id))
            {
                observableComponents[component.Id].Dispose();
                observableComponents.Remove(component.Id);
            }
        }
        
        /// <summary>
        /// 更新所有可观察组件
        /// </summary>
        public void UpdateObservableComponents()
        {
            foreach (ObservableComponent observable in observableComponents.Values)
            {
                observable.Update();
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            foreach (ObservableComponent observable in observableComponents.Values)
            {
                observable.Dispose();
            }
            
            observableComponents.Clear();
        }
    }
    
    /// <summary>
    /// 可观察组件
    /// </summary>
    public class ObservableComponent : IDisposable
    {
        private Entity component;
        private Dictionary<string, object> propertyValues = new Dictionary<string, object>();
        private List<PropertyInfo> observableProperties = new List<PropertyInfo>();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component">组件</param>
        public ObservableComponent(Entity component)
        {
            this.component = component;
            InitializeObservableProperties();
            SaveInitialPropertyValues();
        }
        
        /// <summary>
        /// 初始化可观察属性
        /// </summary>
        private void InitializeObservableProperties()
        {
            Type componentType = component.GetType();
            PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (PropertyInfo property in properties)
            {
                // 只观察带有PropertyChangedAttribute属性的属性
                if (property.GetCustomAttribute<PropertyChangedAttribute>() != null)
                {
                    observableProperties.Add(property);
                }
            }
        }
        
        /// <summary>
        /// 保存初始属性值
        /// </summary>
        private void SaveInitialPropertyValues()
        {
            foreach (PropertyInfo property in observableProperties)
            {
                object value = property.GetValue(component);
                propertyValues[property.Name] = value;
            }
        }
        
        /// <summary>
        /// 更新属性值并检测变化
        /// </summary>
        public void Update()
        {
            foreach (PropertyInfo property in observableProperties)
            {
                object currentValue = property.GetValue(component);
                object previousValue = propertyValues[property.Name];
                
                // 如果属性值发生变化
                if (!Equals(currentValue, previousValue))
                {
                    // 更新属性值
                    propertyValues[property.Name] = currentValue;
                    
                    // 通知属性变化
                    (component as IDataBindable)?.NotifyPropertyChanged(property.Name, currentValue);
                }
            }
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            component = null;
            propertyValues.Clear();
            observableProperties.Clear();
        }
    }
    
    /// <summary>
    /// 属性变化属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyChangedAttribute : Attribute
    {
    }
    
    /// <summary>
    /// 属性变化事件扩展方法
    /// </summary>
    public static class PropertyChangedEventExtensions
    {
        /// <summary>
        /// 启用属性变化事件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        public static void EnablePropertyChangedEvents<T>(this T component) where T : Entity, IDataBindable
        {
            Scene scene = component.DomainScene();
            PropertyChangedEventSystem eventSystem = scene.GetComponent<PropertyChangedEventSystem>();
            
            if (eventSystem != null)
            {
                eventSystem.AddObservableComponent(component);
            }
        }
        
        /// <summary>
        /// 禁用属性变化事件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        public static void DisablePropertyChangedEvents<T>(this T component) where T : Entity, IDataBindable
        {
            Scene scene = component.DomainScene();
            PropertyChangedEventSystem eventSystem = scene.GetComponent<PropertyChangedEventSystem>();
            
            if (eventSystem != null)
            {
                eventSystem.RemoveObservableComponent(component);
            }
        }
    }
}