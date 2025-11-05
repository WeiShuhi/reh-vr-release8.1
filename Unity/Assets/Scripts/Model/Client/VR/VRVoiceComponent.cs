using System;using ET;using System.Collections.Generic;using MemoryPack;

namespace ET.Client.VR
{
    [EntitySystemOf(typeof(VRVoiceComponent))]
    [FriendOf(typeof(VRVoiceComponent))]
    public static partial class VRVoiceComponentSystem
    {
        [EntitySystem]
        private static void Awake(this VRVoiceComponent self)
        {
            self.IsInitialized = false;
            self.IsRecording = false;
            self.IsPlaying = false;
            self.VoiceDataBuffer = new Queue<byte[]>();
        }

        [EntitySystem]
        private static void Start(this VRVoiceComponent self)
        {
            self.InitializeVoiceAsync().Coroutine();
        }

        [EntitySystem]
        private static void Update(this VRVoiceComponent self)
        {
            if (!self.IsInitialized) return;
            
            // 处理语音输入
            self.ProcessVoiceInput();
            
            // 处理语音输出
            self.ProcessVoiceOutput();
        }

        [EntitySystem]
        private static void Destroy(this VRVoiceComponent self)
        {
            self.CleanupVoice();
        }

        private static async ETTask InitializeVoiceAsync(this VRVoiceComponent self)
        {
            try
            {
                self.IsInitialized = true;
                Log.Info("VR voice system initialized successfully");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to initialize VR voice system: {e}");
                self.IsInitialized = false;
            }
        }

        private static void ProcessVoiceInput(this VRVoiceComponent self)
        {
            // 处理语音输入数据
            if (self.IsRecording && self.VoiceInputCallback != null)
            {
                // 模拟语音输入数据
                byte[] voiceData = new byte[1024];
                self.VoiceInputCallback(voiceData);
            }
        }

        private static void ProcessVoiceOutput(this VRVoiceComponent self)
        {
            // 处理语音输出数据
            if (self.IsPlaying && self.VoiceDataBuffer.Count > 0)
            {
                byte[] voiceData = self.VoiceDataBuffer.Dequeue();
                self.PlayVoiceData(voiceData);
            }
        }

        private static void OnSpeechRecognized(this VRVoiceComponent self, string text)
        {
            Log.Info($"Speech recognized: {text}");
            
            // 发送语音识别结果到服务器
            self.SendSpeechRecognitionResultAsync(text).Coroutine();
            
            // 触发语音识别事件
            if (self.SpeechRecognizedCallback != null)
            {
                self.SpeechRecognizedCallback(text);
            }
        }

        private static async ETTask SendSpeechRecognitionResultAsync(this VRVoiceComponent self, string text)
        {
            try
            {
                VRNetworkComponent vrNetwork = self.Root().GetComponent<VRNetworkComponent>();
                if (vrNetwork == null || !vrNetwork.IsConnected) return;
                
                // 序列化语音识别结果
                VRSpeechRecognitionResultMessage message = VRSpeechRecognitionResultMessage.Create();
                message.Text = text;
                message.Timestamp = TimeInfo.Instance.ClientNow();
                
                // 发送消息
                await self.Root().GetComponent<ClientSenderComponent>().Send(message);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send speech recognition result: {e}");
            }
        }

        private static void CleanupVoice(this VRVoiceComponent self)
        {
            // 清理语音识别
            if (self.SpeechRecognizer != null)
            {
                self.SpeechRecognizer.Stop();
                self.SpeechRecognizer.Dispose();
                self.SpeechRecognizer = null;
            }
        }
    }

    public class VRVoiceComponent : Entity, IAwake, IStart, IUpdate, IDestroy
    {
        public bool IsInitialized { get; set; }
        public bool IsRecording { get; set; }
        public bool IsPlaying { get; set; }
        public object SpeechRecognizer { get; set; } // UnityEngine.Windows.Speech.SpeechRecognizer类型
        public object TextToSpeech { get; set; } // UnityEngine.Windows.Speech.TextToSpeech类型
        public object OculusLipSync { get; set; } // OVRLipSync类型
        public object AudioSource { get; set; } // AudioSource类型
        public Queue<byte[]> VoiceDataBuffer { get; set; }
        public Action<byte[]> VoiceInputCallback { get; set; }
        public Action<string> SpeechRecognizedCallback { get; set; }
        public Action<byte[]> VoiceOutputCallback { get; set; }
    }

    [MemoryPackable]
    public partial class VRSpeechRecognitionResultMessage : MessageObject, IMessage
    {
        public static int opcode = 10003;
        public string Text;
        public long Timestamp;
    }

    [MemoryPackable]
    public partial class VRSpeechSynthesisRequestMessage : MessageObject, IMessage
    {
        public static int opcode = 10004;
        public string Text;
        public long Timestamp;
    }

    [MemoryPackable]
    public partial class VRSpeechSynthesisResultMessage : MessageObject, IMessage
    {
        public static int opcode = 10005;
        public byte[] VoiceData;
        public long Timestamp;
    }
}