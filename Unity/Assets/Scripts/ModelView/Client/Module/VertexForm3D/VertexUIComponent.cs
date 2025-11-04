using ET;
using UnityEngine;
using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class VertexUIComponent : Entity, IAwake, IDestroy
    {
        // VertexForm3D UI元素的引用
        public LoadingScreen LoadingScreen { get; private set; }
        public TabBarUIHandler TabBarUI { get; private set; }
        public PlayerUIManager PlayerUIManager { get; private set; }
        
        // 存储所有VertexForm3D UI元素
        private Dictionary<string, GameObject> vertexUIElements = new Dictionary<string, GameObject>();
        
        // 初始化VertexForm3D UI元素
        public void InitializeUIElements()
        {
            // 查找LoadingScreen
            var loadingScreenObj = GameObject.FindObjectOfType<LoadingScreen>();
            if (loadingScreenObj != null)
            {
                LoadingScreen = loadingScreenObj;
                vertexUIElements.Add("LoadingScreen", loadingScreenObj.gameObject);
                Log.Info("Found LoadingScreen: {0}", loadingScreenObj.gameObject.name);
            }
            
            // 查找TabBarUIHandler
            var tabBarUIObj = GameObject.FindObjectOfType<TabBarUIHandler>();
            if (tabBarUIObj != null)
            {
                TabBarUI = tabBarUIObj;
                vertexUIElements.Add("TabBarUI", tabBarUIObj.gameObject);
                Log.Info("Found TabBarUIHandler: {0}", tabBarUIObj.gameObject.name);
            }
            
            // 查找PlayerUIManager
            var playerUIManagerObj = GameObject.FindObjectOfType<PlayerUIManager>();
            if (playerUIManagerObj != null)
            {
                PlayerUIManager = playerUIManagerObj;
                vertexUIElements.Add("PlayerUIManager", playerUIManagerObj.gameObject);
                Log.Info("Found PlayerUIManager: {0}", playerUIManagerObj.gameObject.name);
            }
            
            // 注册UI事件
            RegisterUIEvents();
        }
        
        // 注册UI事件
        private void RegisterUIEvents()
        {
            // 注册TabBarUI事件
            if (TabBarUI != null)
            {
                // 为每个标签按钮添加点击事件
                for (int i = 0; i < TabBarUI.tabButtons.Length; i++)
                {
                    int tabIndex = i;
                    var button = TabBarUI.tabButtons[i];
                    
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => OnTabButtonClicked(tabIndex));
                    }
                }
            }
            
            // 注册LoadingScreen事件
            if (LoadingScreen != null)
            {
                // LoadingScreen事件可以通过SceneLoader监听
            }
        }
        
        // Tab按钮点击事件处理
        private void OnTabButtonClicked(int tabIndex)
        {
            // 将Tab点击事件转换为ET框架事件
            var tabClickedEvent = this.AddChild<VertexTabClickedEvent>();
            tabClickedEvent.TabIndex = tabIndex;
            tabClickedEvent.TabName = TabBarUI.tabButtons[tabIndex].name;
            
            EventSystem.Instance.Publish(this, tabClickedEvent);
            
            Log.Info("VertexForm3D Tab clicked: {0} (Index: {1})
", tabClickedEvent.TabName, tabClickedEvent.TabIndex);
        }
        
        // 显示指定名称的UI元素
        public void ShowUI(string uiName)
        {
            if (vertexUIElements.TryGetValue(uiName, out GameObject uiObject))
            {
                uiObject.SetActive(true);
                Log.Info("Show VertexForm3D UI: {0}", uiName);
            }
            else
            {
                Log.Warning("VertexForm3D UI not found: {0}", uiName);
            }
        }
        
        // 隐藏指定名称的UI元素
        public void HideUI(string uiName)
        {
            if (vertexUIElements.TryGetValue(uiName, out GameObject uiObject))
            {
                uiObject.SetActive(false);
                Log.Info("Hide VertexForm3D UI: {0}", uiName);
            }
            else
            {
                Log.Warning("VertexForm3D UI not found: {0}", uiName);
            }
        }
        
        // 获取指定名称的UI元素
        public GameObject GetUIElement(string uiName)
        {
            if (vertexUIElements.TryGetValue(uiName, out GameObject uiObject))
            {
                return uiObject;
            }
            
            Log.Warning("VertexForm3D UI not found: {0}", uiName);
            return null;
        }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            // 清理UI元素引用
            LoadingScreen = null;
            TabBarUI = null;
            PlayerUIManager = null;
            vertexUIElements.Clear();
            
            base.Dispose();
        }
    }
}

namespace ET.Client
{
    /// <summary>
    /// VertexForm3D Tab点击事件
    /// </summary>
    public class VertexTabClickedEvent : Entity
    {
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            TabName = null;
            base.Dispose();
        }
    }
    
    /// <summary>
    /// VertexForm3D加载进度更新事件
    /// </summary>
    public class VertexLoadingProgressEvent : Entity
    {
        public float Progress { get; set; }
        public string LoadingText { get; set; }
        
        public override void Dispose()
        {
            if (IsDisposed)
                return;
                
            LoadingText = null;
            base.Dispose();
        }
    }
}