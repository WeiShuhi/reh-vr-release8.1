using System; using ET;

namespace ET.Client {
    [ActorMessageHandler(SceneType.Current)]
    public class XRInputActorHandler : AMActorHandler<Unit, C2Server_XRInputSync> {
        protected override async ETTask Run(Unit unit, C2Server_XRInputSync message) {
            // 处理服务器同步的XR输入事件
            foreach (C2Server_XRInputSync.XRInputEvent inputEvent in message.InputEvents) {
                switch (inputEvent.EventType) {
                    case "ControllerButton":
                        // 处理控制器按钮事件
                        HandleControllerButtonEvent(unit, inputEvent);
                        break;
                    case "ControllerAxis":
                        // 处理控制器轴事件
                        HandleControllerAxisEvent(unit, inputEvent);
                        break;
                    case "HandGesture":
                        // 处理手势事件
                        HandleHandGestureEvent(unit, inputEvent);
                        break;
                    case "HeadPose":
                        // 处理头部姿态事件
                        HandleHeadPoseEvent(unit, inputEvent);
                        break;
                }
            }
            
            await ETTask.CompletedTask;
        }
        
        private void HandleControllerButtonEvent(Unit unit, C2Server_XRInputSync.XRInputEvent inputEvent) {
            // 触发本地ET事件
            EventSystem.Instance.Publish(unit.Scene(), new XREventControllerButton {
                DeviceName = inputEvent.DeviceName,
                InputName = inputEvent.InputName,
                Value = inputEvent.BoolValue
            });
        }
        
        private void HandleControllerAxisEvent(Unit unit, C2Server_XRInputSync.XRInputEvent inputEvent) {
            // 触发本地ET事件
            EventSystem.Instance.Publish(unit.Scene(), new XREventControllerAxis {
                DeviceName = inputEvent.DeviceName,
                InputName = inputEvent.InputName,
                Value = inputEvent.FloatValue
            });
        }
        
        private void HandleHandGestureEvent(Unit unit, C2Server_XRInputSync.XRInputEvent inputEvent) {
            // 触发本地ET事件
            EventSystem.Instance.Publish(unit.Scene(), new XREventHandGesture {
                GestureName = inputEvent.InputName,
                Confidence = inputEvent.FloatValue
            });
        }
        
        private void HandleHeadPoseEvent(Unit unit, C2Server_XRInputSync.XRInputEvent inputEvent) {
            // 触发本地ET事件
            EventSystem.Instance.Publish(unit.Scene(), new XREventHeadPose {
                Position = inputEvent.VectorValue,
                Rotation = inputEvent.QuaternionValue
            });
        }
    }
    
    // XR输入Actor组件
    [ComponentOf(typeof(Unit))]
    public class XRInputActorComponent : Entity, IAwake {
        // XR输入状态
        public Dictionary<string, bool> ControllerButtonStates = new Dictionary<string, bool>();
        public Dictionary<string, float> ControllerAxisStates = new Dictionary<string, float>();
        public string CurrentGesture = "None";
        public float GestureConfidence = 0;
        public Vector3 HeadPosition = Vector3.zero;
        public Quaternion HeadRotation = Quaternion.identity;
        
        public void Awake() {
            // 初始化输入状态
            ControllerButtonStates["Select"] = false;
            ControllerButtonStates["Activate"] = false;
            ControllerAxisStates["Rotate"] = 0;
            ControllerAxisStates["Translate"] = 0;
        }
    }
}