using ET;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ET.Client
{
    /// <summary>
    /// 数据绑定管理器
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class DataBindingManager : Entity, IAwake, IDestroy
    {
        // 存储所有数据绑定
        private Dictionary<string, List<DataBinding>> dataBindings = new Dictionary<string, List<DataBinding>>();
        
        // 存储所有可绑定组件
        private Dictionary<long, IDataBindable> bindableComponents = new Dictionary<long, IDataBindable>();
        
        /// <summary>
        /// 添加数据绑定
        /// </summary>
        /// <param name="binding">数据绑定</param>
        public void AddDataBinding(DataBinding binding)
        {
            if (binding == null)
                return;
                
            string key = GetBindingKey(binding.ComponentId, binding.PropertyName);
            
            if (!dataBindings.ContainsKey(key))
            {
                dataBindings[key] = new List<DataBinding>();
            }
            
            dataBindings[key].Add(binding);
        }
        
        /// <summary>
        /// 移除数据绑定
        /// </summary>
        /// <param name="binding">数据绑定</param>
        public void RemoveDataBinding(DataBinding binding)
        {
            if (binding == null)
                return;
                
            string key = GetBindingKey(binding.ComponentId, binding.PropertyName);
            
            if (dataBindings.ContainsKey(key))
            {
                dataBindings[key].Remove(binding);
                
                if (dataBindings[key].Count == 0)
                {
                    dataBindings.Remove(key);
                }
            }
        }
        /// <summary>
        /// 获取数据绑定
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>数据绑定列表</returns>
        public List<DataBinding> GetDataBindings(long componentId, string propertyName)
        {
            string key = GetBindingKey(componentId, propertyName);
            
            if (dataBindings.ContainsKey(key))
            {
                return dataBindings[key];
            }
            
            return new List<DataBinding>();
        }
        
        /// <summary>
        /// 属性值变化时通知所有绑定的UI元素
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">新的属性值</param>
        public void NotifyPropertyChanged(long componentId, string propertyName, object value)
        {
            List<DataBinding> bindings = GetDataBindings(componentId, propertyName);
            
            foreach (DataBinding binding in bindings)
            {
                try
                {
                    binding.UpdateUI(value);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to update UI for property {0}: {1}", propertyName, e.Message);
                }
            }
        }
        
        /// <summary>
        /// 添加可绑定组件
        /// </summary>
        /// <param name="component">可绑定组件</param>
        public void AddBindableComponent(IDataBindable component)
        {
            if (component == null)
                return;
                
            if (!bindableComponents.ContainsKey(component.Id))
            {
                bindableComponents[component.Id] = component;
            }
        }
        
        /// <summary>
        /// 移除可绑定组件
        /// </summary>
        /// <param name="component">可绑定组件</param>
        public void RemoveBindableComponent(IDataBindable component)
        {
            if (component == null)
                return;
                
            if (bindableComponents.ContainsKey(component.Id))
            {
                bindableComponents.Remove(component.Id);
            }
        }
        
        /// <summary>
        /// 获取可绑定组件
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <returns>可绑定组件</returns>
        public IDataBindable GetBindableComponent(long componentId)
        {
            if (bindableComponents.ContainsKey(componentId))
            {
                return bindableComponents[componentId];
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取绑定键
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>绑定键</returns>
        private string GetBindingKey(long componentId, string propertyName)
        {
            return $"{componentId}_{propertyName}";
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            // 清理所有数据绑定
            foreach (List<DataBinding> bindings in dataBindings.Values)
            {
                foreach (DataBinding binding in bindings)
                {
                    binding.Dispose();
                }
            }
            
            dataBindings.Clear();
            bindableComponents.Clear();
        }
    }
    
    /// <summary>
    /// 数据绑定接口
    /// </summary>
    public interface IDataBindable
    {
        long Id { get; }
    }
    
    /// <summary>
    /// 数据绑定
    /// </summary>
    public class DataBinding : Entity
    {
        public long ComponentId { get; private set; }
        public string PropertyName { get; private set; }
        public Action<object> UpdateUIAction { get; private set; }
        
        /// <summary>
        /// 初始化数据绑定
        /// </summary>
        /// <param name="componentId">组件ID</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="updateUIAction">更新UI的动作</param>
        public void Initialize(long componentId, string propertyName, Action<object> updateUIAction)
        {
            this.ComponentId = componentId;
            this.PropertyName = propertyName;
            this.UpdateUIAction = updateUIAction;
        }
        
        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="value">新的属性值</param>
        public void UpdateUI(object value)
        {
            UpdateUIAction?.Invoke(value);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            UpdateUIAction = null;
        }
    }
}