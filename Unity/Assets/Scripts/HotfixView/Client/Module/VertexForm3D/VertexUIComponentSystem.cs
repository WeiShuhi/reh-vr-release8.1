using ET;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(VertexUIComponent))]
    [FriendOf(typeof(VertexUIComponent))]
    public static partial class VertexUIComponentSystem
    {
        [EntitySystem]
        private static void Awake(this VertexUIComponent self)
        {
            Log.Info("VertexUIComponentSystem Awake");
            
            // 初始化VertexForm3D UI元素
            self.InitializeUIElements();
            
            // 注册UI事件
            RegisterUIEvents(self);
            
            // 启动UI更新循环
            self.StartCoroutine(UIUpdateLoop(self).Coroutine());
        }
        
        [EntitySystem]
        private static void Destroy(this VertexUIComponent self)
        {
            Log.Info("VertexUIComponentSystem Destroy");
            
            // 取消注册UI事件
            UnregisterUIEvents(self);
        }
        
        private static void RegisterUIEvents(VertexUIComponent self)
        {
            // 注册ET框架UI事件
            EventSystem.Instance.Subscribe(self, typeof(ShowUIEvent), OnShowUI);
            EventSystem.Instance.Subscribe(self, typeof(HideUIEvent), OnHideUI);
            
            // 注册VertexForm3D特定UI事件
            EventSystem.Instance.Subscribe(self, typeof(VertexTabClickedEvent), OnTabClicked);
        }
        
        private static void UnregisterUIEvents(VertexUIComponent self)
        {
            // 取消注册ET框架UI事件
            EventSystem.Instance.Unsubscribe(self, typeof(ShowUIEvent), OnShowUI);
            EventSystem.Instance.Unsubscribe(self, typeof(HideUIEvent), OnHideUI);
            
            // 取消注册VertexForm3D特定UI事件
            EventSystem.Instance.Unsubscribe(self, typeof(VertexTabClickedEvent), OnTabClicked);
        }
        
        private static void OnShowUI(Entity self, object args)
        {
            var vertexUI = self as VertexUIComponent;
            var showEvent = args as ShowUIEvent;
            
            if (vertexUI == null || showEvent == null)
                return;
                
            // 显示指定的VertexForm3D UI
            vertexUI.ShowUI(showEvent.UIName);
        }
        
        private static void OnHideUI(Entity self, object args)
        {
            var vertexUI = self as VertexUIComponent;
            var hideEvent = args as HideUIEvent;
            
            if (vertexUI == null || hideEvent == null)
                return;
                
            // 隐藏指定的VertexForm3D UI
            vertexUI.HideUI(hideEvent.UIName);
        }
        
        private static void OnTabClicked(Entity self, object args)
        {
            var vertexUI = self as VertexUIComponent;
            var tabEvent = args as VertexTabClickedEvent;
            
            if (vertexUI == null || tabEvent == null)
                return;
                
            // 处理Tab点击事件
            Log.Info("Processing tab click: {0} (Index: {1})
", tabEvent.TabName, tabEvent.TabIndex);
            
            // 可以在这里添加额外的逻辑，比如更新数据或触发其他事件
        }
        
        private static async ETTask UIUpdateLoop(VertexUIComponent self)
        {
            while (true)
            {
                await ETTask.DelayFrame(1);
                
                // 处理LoadingScreen的进度更新
                UpdateLoadingProgress(self);
            }
        }
        
        private static void UpdateLoadingProgress(VertexUIComponent self)
        {
            if (self.LoadingScreen == null)
                return;
                
            // 获取当前加载进度
            float progress = self.LoadingScreen.loadingProgress;
            string loadingText = self.LoadingScreen.loadingText.text;
            
            // 发布加载进度更新事件
            var loadingProgressEvent = self.AddChild<VertexLoadingProgressEvent>();
            loadingProgressEvent.Progress = progress;
            loadingProgressEvent.LoadingText = loadingText;
            
            EventSystem.Instance.Publish(self, loadingProgressEvent);
        }
        
        /// <summary>
        /// 创建VertexForm3D UI并挂接到ET的UIComponent管理
        /// </summary>
        public static async ETTask<UI> CreateVertexUI(this UIComponent self, string uiName, UILayer uiLayer = UILayer.Mid)
        {
            Log.Info("Creating VertexForm3D UI: {0}", uiName);
            
            // 创建UI实体
            UI ui = await self.Create(uiName, uiLayer);
            
            // 添加VertexUIComponent到UI实体
            VertexUIComponent vertexUI = ui.AddComponent<VertexUIComponent>();
            
            return ui;
        }
        
        /// <summary>
        /// 显示VertexForm3D UI
        /// </summary>
        public static void ShowVertexUI(this UIComponent self, string uiName)
        {
            Log.Info("Showing VertexForm3D UI: {0}", uiName);
            
            // 触发ShowUIEvent事件
            var showEvent = self.AddChild<ShowUIEvent>();
            showEvent.UIName = uiName;
            
            EventSystem.Instance.Publish(self, showEvent);
        }
        
        /// <summary>
        /// 隐藏VertexForm3D UI
        /// </summary>
        public static void HideVertexUI(this UIComponent self, string uiName)
        {
            Log.Info("Hiding VertexForm3D UI: {0}", uiName);
            
            // 触发HideUIEvent事件
            var hideEvent = self.AddChild<HideUIEvent>();
            hideEvent.UIName = uiName;
            
            EventSystem.Instance.Publish(self, hideEvent);
        }
        
        /// <summary>
        /// 获取VertexForm3D UI组件
        /// </summary>
        public static VertexUIComponent GetVertexUI(this UIComponent self, string uiName)
        {
            UI ui = self.Get(uiName);
            if (ui == null)
                return null;
                
            return ui.GetComponent<VertexUIComponent>();
        }
    }
    
    /// <summary>
    /// 显示VertexForm3D UI事件
    /// </summary>
    public class ShowUIEvent : Entity
    {
        public string UIName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            UIName = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// 隐藏VertexForm3D UI事件
    /// </summary>
    public class HideUIEvent : Entity
    {
        public string UIName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            UIName = null;
            base.Dispose();
        }
    }
}