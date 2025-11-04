using ET;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// UI数据绑定系统
    /// </summary>
    [ComponentOf(typeof(UI))]
    public class UIDataBindingSystem : Entity, IAwake, IDestroy
    {
        // 存储所有UI数据绑定
        private List<UIDataBinding> uiDataBindings = new List<UIDataBinding>();
        
        /// <summary>
        /// 添加UI数据绑定
        /// </summary>
        /// <param name="binding">UI数据绑定</param>
        public void AddUIDataBinding(UIDataBinding binding)
        {
            if (binding == null)
                return;
                
            uiDataBindings.Add(binding);
        }
        
        /// <summary>
        /// 移除UI数据绑定
        /// </summary>
        /// <param name="binding">UI数据绑定</param>
        public void RemoveUIDataBinding(UIDataBinding binding)
        {
            if (binding == null)
                return;
                
            uiDataBindings.Remove(binding);
        }
        
        /// <summary>
        /// 清理所有UI数据绑定
        /// </summary>
        public void ClearUIDataBindings()
        {
            foreach (UIDataBinding binding in uiDataBindings)
            {
                binding.Dispose();
            }
            
            uiDataBindings.Clear();
        }
        
        /// <summary>
        /// 绑定UI元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="uiElement">UI元素</param>
        /// <param name="updateUIAction">更新UI的动作</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindUI<T>(T component, string propertyName, UnityEngine.Object uiElement, Action<UnityEngine.Object, object> updateUIAction) where T : Entity, IDataBindable
        {
            if (component == null || uiElement == null || updateUIAction == null)
                return null;
                
            UIDataBinding binding = UIDataBinding.Create(component, propertyName, uiElement, updateUIAction);
            AddUIDataBinding(binding);
            
            return binding;
        }
        
        /// <summary>
        /// 绑定Text元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="textElement">Text元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindText<T>(T component, string propertyName, Text textElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, textElement, (uiElement, value) =>
            {
                Text text = uiElement as Text;
                if (text != null)
                {
                    text.text = value.ToString();
                }
            });
        }
        
        /// <summary>
        /// 绑定TMP_Text元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="textElement">TMP_Text元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindTMPText<T>(T component, string propertyName, TMP_Text textElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, textElement, (uiElement, value) =>
            {
                TMP_Text text = uiElement as TMP_Text;
                if (text != null)
                {
                    text.text = value.ToString();
                }
            });
        }
        
        /// <summary>
        /// 绑定Image元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="imageElement">Image元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindImage<T>(T component, string propertyName, Image imageElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, imageElement, (uiElement, value) =>
            {
                Image image = uiElement as Image;
                Sprite sprite = value as Sprite;
                
                if (image != null && sprite != null)
                {
                    image.sprite = sprite;
                }
            });
        }
        
        /// <summary>
        /// 绑定RawImage元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="rawImageElement">RawImage元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindRawImage<T>(T component, string propertyName, RawImage rawImageElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, rawImageElement, (uiElement, value) =>
            {
                RawImage rawImage = uiElement as RawImage;
                Texture texture = value as Texture;
                
                if (rawImage != null && texture != null)
                {
                    rawImage.texture = texture;
                }
            });
        }
        
        /// <summary>
        /// 绑定Slider元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sliderElement">Slider元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindSlider<T>(T component, string propertyName, Slider sliderElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, sliderElement, (uiElement, value) =>
            {
                Slider slider = uiElement as Slider;
                float floatValue = Convert.ToSingle(value);
                
                if (slider != null)
                {
                    slider.value = floatValue;
                }
            });
        }
        
        /// <summary>
        /// 绑定Toggle元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="toggleElement">Toggle元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindToggle<T>(T component, string propertyName, Toggle toggleElement) where T : Entity, IDataBindable
        {
            return BindUI(component, propertyName, toggleElement, (uiElement, value) =>
            {
                Toggle toggle = uiElement as Toggle;
                bool boolValue = Convert.ToBoolean(value);
                
                if (toggle != null)
                {
                    toggle.isOn = boolValue;
                }
            });
        }
        
        /// <summary>
        /// 绑定InputField元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="inputFieldElement">InputField元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindInputField<T>(T component, string propertyName, InputField inputFieldElement) where T : Entity, IDataBindable
        {
            UIDataBinding binding = BindUI(component, propertyName, inputFieldElement, (uiElement, value) =>
            {
                InputField inputField = uiElement as InputField;
                if (inputField != null)
                {
                    inputField.text = value.ToString();
                }
            });
            
            // 添加InputField的OnValueChanged事件
            InputField inputField = inputFieldElement as InputField;
            if (inputField != null)
            {
                inputField.onValueChanged.AddListener((value) =>
                {
                    UpdateComponentProperty(component, propertyName, value);
                });
            }
            
            return binding;
        }
        
        /// <summary>
        /// 绑定TMP_InputField元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="inputFieldElement">TMP_InputField元素</param>
        /// <returns>UI数据绑定</returns>
        public UIDataBinding BindTMPInputField<T>(T component, string propertyName, TMP_InputField inputFieldElement) where T : Entity, IDataBindable
        {
            UIDataBinding binding = BindUI(component, propertyName, inputFieldElement, (uiElement, value) =>
            {
                TMP_InputField inputField = uiElement as TMP_InputField;
                if (inputField != null)
                {
                    inputField.text = value.ToString();
                }
            });
            
            // 添加TMP_InputField的OnValueChanged事件
            TMP_InputField inputField = inputFieldElement as TMP_InputField;
            if (inputField != null)
            {
                inputField.onValueChanged.AddListener((value) =>
                {
                    UpdateComponentProperty(component, propertyName, value);
                });
            }
            
            return binding;
        }
        
        /// <summary>
        /// 更新组件属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">新的属性值</param>
        private void UpdateComponentProperty<T>(T component, string propertyName, object value) where T : Entity, IDataBindable
        {
            Type componentType = component.GetType();
            System.Reflection.PropertyInfo property = componentType.GetProperty(propertyName);
            
            if (property != null && property.CanWrite)
            {
                try
                {
                    property.SetValue(component, value);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to update property {0}: {1}", propertyName, e.Message);
                }
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            ClearUIDataBindings();
        }
    }
    
    /// <summary>
    /// UI数据绑定
    /// </summary>
    public class UIDataBinding : IDisposable
    {
        public long ComponentId { get; private set; }
        public string PropertyName { get; private set; }
        public UnityEngine.Object UIElement { get; private set; }
        public Action<UnityEngine.Object, object> UpdateUIAction { get; private set; }
        
        private Action<object> subscriptionCallback;
        
        /// <summary>
        /// 创建UI数据绑定
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="uiElement">UI元素</param>
        /// <param name="updateUIAction">更新UI的动作</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding Create<T>(T component, string propertyName, UnityEngine.Object uiElement, Action<UnityEngine.Object, object> updateUIAction) where T : Entity, IDataBindable
        {
            UIDataBinding binding = new UIDataBinding();
            binding.ComponentId = component.Id;
            binding.PropertyName = propertyName;
            binding.UIElement = uiElement;
            binding.UpdateUIAction = updateUIAction;
            
            // 订阅属性变化
            binding.subscriptionCallback = (value) =>
            {
                binding.UpdateUIAction?.Invoke(binding.UIElement, value);
            };
            
            component.Subscribe(propertyName, binding.subscriptionCallback);
            
            // 初始化UI值
            System.Reflection.PropertyInfo property = component.GetType().GetProperty(propertyName);
            if (property != null)
            {
                object value = property.GetValue(component);
                binding.UpdateUIAction?.Invoke(binding.UIElement, value);
            }
            
            return binding;
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 取消订阅属性变化
            if (subscriptionCallback != null)
            {
                Scene scene = UIElement.GetEntity().DomainScene();
                DataSubscriptionSystem subscriptionSystem = scene.GetComponent<DataSubscriptionSystem>();
                
                if (subscriptionSystem != null)
                {
                    subscriptionSystem.Unsubscribe(ComponentId, PropertyName, subscriptionCallback);
                }
                
                subscriptionCallback = null;
            }
            
            UIElement = null;
            UpdateUIAction = null;
        }
    }
    
    /// <summary>
    /// UI数据绑定扩展方法
    /// </summary>
    public static class UIDataBindingExtensions
    {
        /// <summary>
        /// 获取UI数据绑定系统
        /// </summary>
        /// <param name="ui">UI实体</param>
        /// <returns>UI数据绑定系统</returns>
        public static UIDataBindingSystem GetUIDataBindingSystem(this UI ui)
        {
            return ui.GetComponent<UIDataBindingSystem>();
        }
        
        /// <summary>
        /// 绑定Text元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="textElement">Text元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindText<T>(this UI ui, T component, string propertyName, Text textElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindText(component, propertyName, textElement);
        }
        
        /// <summary>
        /// 绑定TMP_Text元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="textElement">TMP_Text元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindTMPText<T>(this UI ui, T component, string propertyName, TMP_Text textElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindTMPText(component, propertyName, textElement);
        }
        
        /// <summary>
        /// 绑定Image元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="imageElement">Image元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindImage<T>(this UI ui, T component, string propertyName, Image imageElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindImage(component, propertyName, imageElement);
        }
        
        /// <summary>
        /// 绑定RawImage元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="rawImageElement">RawImage元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindRawImage<T>(this UI ui, T component, string propertyName, RawImage rawImageElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindRawImage(component, propertyName, rawImageElement);
        }
        
        /// <summary>
        /// 绑定Slider元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="sliderElement">Slider元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindSlider<T>(this UI ui, T component, string propertyName, Slider sliderElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindSlider(component, propertyName, sliderElement);
        }
        
        /// <summary>
        /// 绑定Toggle元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="toggleElement">Toggle元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindToggle<T>(this UI ui, T component, string propertyName, Toggle toggleElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindToggle(component, propertyName, toggleElement);
        }
        
        /// <summary>
        /// 绑定InputField元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="inputFieldElement">InputField元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindInputField<T>(this UI ui, T component, string propertyName, InputField inputFieldElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindInputField(component, propertyName, inputFieldElement);
        }
        
        /// <summary>
        /// 绑定TMP_InputField元素到ET组件的属性
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="ui">UI实体</param>
        /// <param name="component">组件</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="inputFieldElement">TMP_InputField元素</param>
        /// <returns>UI数据绑定</returns>
        public static UIDataBinding BindTMPInputField<T>(this UI ui, T component, string propertyName, TMP_InputField inputFieldElement) where T : Entity, IDataBindable
        {
            UIDataBindingSystem bindingSystem = ui.GetUIDataBindingSystem();
            return bindingSystem?.BindTMPInputField(component, propertyName, inputFieldElement);
        }
    }
}