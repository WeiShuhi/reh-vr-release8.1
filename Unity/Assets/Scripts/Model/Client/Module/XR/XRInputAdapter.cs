using System; using System.Collections.Generic; using UnityEngine.XR.Interaction.Toolkit; using ET;

namespace ET.Client {
    [ComponentOf(typeof(Scene))]
    public class XRInputAdapterComponent : Entity, IAwake, IUpdate {
        // ET事件系统
        private EventSystem eventSystem;
        
        // XR输入事件缓冲区
        private Queue<XRInputEvent> inputEventBuffer;
        
        // 输入事件缓冲区最大大小
        private const int MaxInputEventBufferSize = 100;
        
        // 时间戳对齐阈值（毫秒）
        private const long TimestampAlignmentThreshold = 50;
        
        // 丢帧防护阈值（毫秒）
        private const long FrameDropProtectionThreshold = 1000;
        
        // 输入事件类型
        public enum XRInputEventType {
            ControllerButton, ControllerAxis, HandGesture, HeadPose
        }
        
        // XR输入事件结构
        public struct XRInputEvent {
            public XRInputEventType eventType;
            public string deviceName;
            public string inputName;
            public float floatValue;
            public bool boolValue;
            public Vector3 vectorValue;
            public Quaternion quaternionValue;
            public long timestamp;
        }
        
        public void Awake() {
            eventSystem = World.Instance.Get<EventSystem>();
            inputEventBuffer = new Queue<XRInputEvent>();
            
            // 注册XR输入事件
            RegisterXRInputEvents();
            
            Log.Info("XRInputAdapterComponent初始化完成");
        }
        
        public void Update() {
            // 处理输入事件缓冲区
            ProcessInputEventBuffer();
        }
        
        private void RegisterXRInputEvents() {
            // 注册控制器输入事件
            XRController[] controllers = UnityEngine.Object.FindObjectsOfType<XRController>();
            foreach (XRController controller in controllers) {
                // 注册按钮事件
                controller.selectAction.action.performed += (context) => {
                    OnControllerButtonInput(controller.name, "Select", true);
                };
                
                controller.selectAction.action.canceled += (context) => {
                    OnControllerButtonInput(controller.name, "Select", false);
                };
                
                controller.activateAction.action.performed += (context) => {
                    OnControllerButtonInput(controller.name, "Activate", true);
                };
                
                controller.activateAction.action.canceled += (context) => {
                    OnControllerButtonInput(controller.name, "Activate", false);
                };
                
                // 注册轴事件
                controller.rotateAnchorAction.action.performed += (context) => {
                    OnControllerAxisInput(controller.name, "Rotate", context.ReadValue<float>());
                };
                
                controller.translateAnchorAction.action.performed += (context) => {
                    OnControllerAxisInput(controller.name, "Translate", context.ReadValue<float>());
                };
            }
            
            // 注册手势输入事件
            // TODO: 实现手势输入事件注册
            
            // 注册头部姿态事件
            // TODO: 实现头部姿态事件注册
        }
        
        private void OnControllerButtonInput(string deviceName, string inputName, bool value) {
            XRInputEvent inputEvent = new XRInputEvent {
                eventType = XRInputEventType.ControllerButton,
                deviceName = deviceName,
                inputName = inputName,
                boolValue = value,
                timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到缓冲区
            AddInputEventToBuffer(inputEvent);
            
            // 直接触发ET事件
            eventSystem.Publish(this.Scene(), new XREventControllerButton {
                DeviceName = deviceName,
                InputName = inputName,
                Value = value
            });
        }
        
        private void OnControllerAxisInput(string deviceName, string inputName, float value) {
            XRInputEvent inputEvent = new XRInputEvent {
                eventType = XRInputEventType.ControllerAxis,
                deviceName = deviceName,
                inputName = inputName,
                floatValue = value,
                timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到缓冲区
            AddInputEventToBuffer(inputEvent);
            
            // 直接触发ET事件
            eventSystem.Publish(this.Scene(), new XREventControllerAxis {
                DeviceName = deviceName,
                InputName = inputName,
                Value = value
            });
        }
        
        private void OnHandGestureInput(string gestureName, float confidence) {
            XRInputEvent inputEvent = new XRInputEvent {
                eventType = XRInputEventType.HandGesture,
                deviceName = "Hand",
                inputName = gestureName,
                floatValue = confidence,
                timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到缓冲区
            AddInputEventToBuffer(inputEvent);
            
            // 直接触发ET事件
            eventSystem.Publish(this.Scene(), new XREventHandGesture {
                GestureName = gestureName,
                Confidence = confidence
            });
        }
        
        private void OnHeadPoseInput(Vector3 position, Quaternion rotation) {
            XRInputEvent inputEvent = new XRInputEvent {
                eventType = XRInputEventType.HeadPose,
                deviceName = "Head",
                inputName = "Pose",
                vectorValue = position,
                quaternionValue = rotation,
                timestamp = TimeInfo.Instance.ServerNow()
            };
            
            // 将事件添加到缓冲区
            AddInputEventToBuffer(inputEvent);
            
            // 直接触发ET事件
            eventSystem.Publish(this.Scene(), new XREventHeadPose {
                Position = position,
                Rotation = rotation
            });
        }
        
        private void AddInputEventToBuffer(XRInputEvent inputEvent) {
            // 确保缓冲区不超过最大大小
            if (inputEventBuffer.Count >= MaxInputEventBufferSize) {
                // 移除最旧的事件
                inputEventBuffer.Dequeue();
            }
            
            // 添加新事件到缓冲区
            inputEventBuffer.Enqueue(inputEvent);
        }
        
        private void ProcessInputEventBuffer() {
            // 处理输入事件缓冲区
            long currentTime = TimeInfo.Instance.ServerNow();
            
            while (inputEventBuffer.Count > 0) {
                XRInputEvent inputEvent = inputEventBuffer.Dequeue();
                
                // 丢帧防护：跳过超过阈值的旧事件
                long timeDiff = currentTime - inputEvent.timestamp;
                if (timeDiff > FrameDropProtectionThreshold) {
                    continue;
                }
                
                // 时间戳对齐：如果事件时间戳与当前时间差在阈值内，则调整为当前时间
                if (Math.Abs(timeDiff) < TimestampAlignmentThreshold) {
                    inputEvent.timestamp = currentTime;
                }
                
                // 触发ET事件
                switch (inputEvent.eventType) {
                    case XRInputEventType.ControllerButton:
                        eventSystem.Publish(this.Scene(), new XREventControllerButton {
                            DeviceName = inputEvent.deviceName,
                            InputName = inputEvent.inputName,
                            Value = inputEvent.boolValue
                        });
                        break;
                    case XRInputEventType.ControllerAxis:
                        eventSystem.Publish(this.Scene(), new XREventControllerAxis {
                            DeviceName = inputEvent.deviceName,
                            InputName = inputEvent.inputName,
                            Value = inputEvent.floatValue
                        });
                        break;
                    case XRInputEventType.HandGesture:
                        eventSystem.Publish(this.Scene(), new XREventHandGesture {
                            GestureName = inputEvent.inputName,
                            Confidence = inputEvent.floatValue
                        });
                        break;
                    case XRInputEventType.HeadPose:
                        eventSystem.Publish(this.Scene(), new XREventHeadPose {
                            Position = inputEvent.vectorValue,
                            Rotation = inputEvent.quaternionValue
                        });
                        break;
                }
            }
        }
    }
    
    // ET事件定义
    public struct XREventControllerButton : IEvent {
        public string DeviceName;
        public string InputName;
        public bool Value;
    }
    
    public struct XREventControllerAxis : IEvent {
        public string DeviceName;
        public string InputName;
        public float Value;
    }
    
    public struct XREventHandGesture : IEvent {
        public string GestureName;
        public float Confidence;
    }
    
    public struct XREventHeadPose : IEvent {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}