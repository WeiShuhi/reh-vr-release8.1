using System; using System.Collections.Generic; using System.IO; using ET;

namespace ET.Client {
    [ComponentOf(typeof(Scene))]
    public class XRLoggerComponent : Entity, IAwake, IDestroy {
        // 日志文件路径
        private string logFilePath;
        
        // 日志文件流
        private StreamWriter logWriter;
        
        // 日志级别
        public enum LogLevel {
            Debug, Info, Warning, Error
        }
        
        public void Awake() {
            // 初始化日志文件
            InitializeLogFile();
            
            // 记录启动日志
            LogInfo("XRLoggerComponent初始化完成");
            
            // 订阅ET日志事件
            // TODO: 订阅ET日志事件
        }
        
        public void Destroy() {
            // 关闭日志文件
            if (logWriter != null) {
                logWriter.Close();
                logWriter.Dispose();
            }
        }
        
        private void InitializeLogFile() {
            // 创建日志文件目录
            string logDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "XRLogs");
            Directory.CreateDirectory(logDirectory);
            
            // 创建日志文件
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logFilePath = Path.Combine(logDirectory, $"XRLog_{timestamp}.txt");
            
            // 打开日志文件流
            logWriter = new StreamWriter(logFilePath, true);
            logWriter.AutoFlush = true;
            
            // 记录日志文件路径
            LogInfo($"日志文件路径: {logFilePath}");
        }
        
        public void LogDebug(string message) {
            WriteLog(LogLevel.Debug, message);
        }
        
        public void LogInfo(string message) {
            WriteLog(LogLevel.Info, message);
        }
        
        public void LogWarning(string message) {
            WriteLog(LogLevel.Warning, message);
        }
        
        public void LogError(string message) {
            WriteLog(LogLevel.Error, message);
        }
        
        private void WriteLog(LogLevel level, string message) {
            // 格式化日志
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logLine = $"[{timestamp}] [{level}] {message}";
            
            // 写入日志文件
            if (logWriter != null) {
                logWriter.WriteLine(logLine);
            }
            
            // 输出到Unity控制台
            switch (level) {
                case LogLevel.Debug:
                    UnityEngine.Debug.Log(logLine);
                    break;
                case LogLevel.Info:
                    UnityEngine.Debug.Log(logLine);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logLine);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(logLine);
                    break;
            }
        }
        
        // 获取日志文件内容
        public string GetLogContent() {
            if (File.Exists(logFilePath)) {
                return File.ReadAllText(logFilePath);
            }
            return "日志文件不存在";
        }
        
        // 获取所有日志文件
        public List<string> GetLogFiles() {
            string logDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "XRLogs");
            if (Directory.Exists(logDirectory)) {
                string[] files = Directory.GetFiles(logDirectory, "*.txt");
                List<string> logFiles = new List<string>(files);
                logFiles.Sort((a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
                return logFiles;
            }
            return new List<string>();
        }
    }
    
    // XR日志辅助类
    public static class XRLogger {
        public static void Debug(string message) {
            Scene currentScene = ET.World.Instance.CurrentScene;
            if (currentScene != null) {
                XRLoggerComponent logger = currentScene.GetComponent<XRLoggerComponent>();
                if (logger != null) {
                    logger.LogDebug(message);
                }
            }
        }
        
        public static void Info(string message) {
            Scene currentScene = ET.World.Instance.CurrentScene;
            if (currentScene != null) {
                XRLoggerComponent logger = currentScene.GetComponent<XRLoggerComponent>();
                if (logger != null) {
                    logger.LogInfo(message);
                }
            }
        }
        
        public static void Warning(string message) {
            Scene currentScene = ET.World.Instance.CurrentScene;
            if (currentScene != null) {
                XRLoggerComponent logger = currentScene.GetComponent<XRLoggerComponent>();
                if (logger != null) {
                    logger.LogWarning(message);
                }
            }
        }
        
        public static void Error(string message) {
            Scene currentScene = ET.World.Instance.CurrentScene;
            if (currentScene != null) {
                XRLoggerComponent logger = currentScene.GetComponent<XRLoggerComponent>();
                if (logger != null) {
                    logger.LogError(message);
                }
            }
        }
    }
}