using UnityEngine; using TMPro; using ET;

namespace ET.Client {
    public class XRInputDebugger : MonoBehaviour {
        public TextMeshProUGUI controllerButtonText;
        public TextMeshProUGUI controllerAxisText;
        public TextMeshProUGUI handGestureText;
        public TextMeshProUGUI headPoseText;
        
        private Scene currentScene;
        private EventSystem eventSystem;
        
        // 控制器按钮状态
        private bool leftSelectPressed = false;
        private bool leftActivatePressed = false;
        private bool rightSelectPressed = false;
        private bool rightActivatePressed = false;
        
        // 控制器轴状态
        private float leftRotateAxis = 0;
        private float leftTranslateAxis = 0;
        private float rightRotateAxis = 0;
        private float rightTranslateAxis = 0;
        
        void Start() {
            // 获取当前ET场景
            currentScene = World.Instance.CurrentScene;
            if (currentScene == null) {
                Debug.LogError("XRInputDebugger: 当前场景为空");
                return;
            }
            
            // 获取事件系统
            eventSystem = currentScene.GetComponent<EventSystem>();
            if (eventSystem == null) {
                Debug.LogError("XRInputDebugger: 事件系统为空");
                return;
            }
            
            // 订阅XR输入事件
            eventSystem.Subscribe<XREventControllerButton>(currentScene, OnControllerButtonEvent);
            eventSystem.Subscribe<XREventControllerAxis>(currentScene, OnControllerAxisEvent);
            eventSystem.Subscribe<XREventHandGesture>(currentScene, OnHandGestureEvent);
            eventSystem.Subscribe<XREventHeadPose>(currentScene, OnHeadPoseEvent);
            
            // 初始化文本
            UpdateControllerButtonText();
            UpdateControllerAxisText();
            UpdateHandGestureText("None", 0);
            UpdateHeadPoseText(Vector3.zero, Quaternion.identity);
        }
        
        void OnDestroy() {
            if (eventSystem != null && currentScene != null) {
                // 取消订阅事件
                eventSystem.Unsubscribe<XREventControllerButton>(currentScene, OnControllerButtonEvent);
                eventSystem.Unsubscribe<XREventControllerAxis>(currentScene, OnControllerAxisEvent);
                eventSystem.Unsubscribe<XREventHandGesture>(currentScene, OnHandGestureEvent);
                eventSystem.Unsubscribe<XREventHeadPose>(currentScene, OnHeadPoseEvent);
            }
        }
        
        private void OnControllerButtonEvent(Scene scene, XREventControllerButton e) {
            // 更新按钮状态
            if (e.DeviceName.Contains("Left")) {
                if (e.InputName == "Select") {
                    leftSelectPressed = e.Value;
                } else if (e.InputName == "Activate") {
                    leftActivatePressed = e.Value;
                }
            } else if (e.DeviceName.Contains("Right")) {
                if (e.InputName == "Select") {
                    rightSelectPressed = e.Value;
                } else if (e.InputName == "Activate") {
                    rightActivatePressed = e.Value;
                }
            }
            
            UpdateControllerButtonText();
        }
        
        private void OnControllerAxisEvent(Scene scene, XREventControllerAxis e) {
            // 更新轴状态
            if (e.DeviceName.Contains("Left")) {
                if (e.InputName == "Rotate") {
                    leftRotateAxis = e.Value;
                } else if (e.InputName == "Translate") {
                    leftTranslateAxis = e.Value;
                }
            } else if (e.DeviceName.Contains("Right")) {
                if (e.InputName == "Rotate") {
                    rightRotateAxis = e.Value;
                } else if (e.InputName == "Translate") {
                    rightTranslateAxis = e.Value;
                }
            }
            
            UpdateControllerAxisText();
        }
        
        private void OnHandGestureEvent(Scene scene, XREventHandGesture e) {
            UpdateHandGestureText(e.GestureName, e.Confidence);
        }
        
        private void OnHeadPoseEvent(Scene scene, XREventHeadPose e) {
            UpdateHeadPoseText(e.Position, e.Rotation);
        }
        
        private void UpdateControllerButtonText() {
            controllerButtonText.text = $"左手控制器按钮:\n选择: {leftSelectPressed}\n激活: {leftActivatePressed}\n\n右手控制器按钮:\n选择: {rightSelectPressed}\n激活: {rightActivatePressed}";
        }
        
        private void UpdateControllerAxisText() {
            controllerAxisText.text = $"左手控制器轴:\n旋转: {leftRotateAxis:F2}\n平移: {leftTranslateAxis:F2}\n\n右手控制器轴:\n旋转: {rightRotateAxis:F2}\n平移: {rightTranslateAxis:F2}";
        }
        
        private void UpdateHandGestureText(string gesture, float confidence) {
            handGestureText.text = $"手势识别:\n当前手势: {gesture}\n置信度: {confidence:F2}";
        }
        
        private void UpdateHeadPoseText(Vector3 position, Quaternion rotation) {
            headPoseText.text = $"头部姿态:\n位置: {position:F2}\n旋转: {rotation.eulerAngles:F2}";
        }
    }
}