using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ET.Client
{
    [EntitySystemOf(typeof(ResourcesLoaderComponent))]
    [FriendOf(typeof(ResourcesLoaderComponent))]
    public static partial class ResourcesLoaderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self)
        {
            self.package = YooAssets.GetPackage("DefaultPackage");
        }

        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self, string packageName)
        {
            self.package = YooAssets.GetPackage(packageName);
        }

        [EntitySystem]
        private static void Destroy(this ResourcesLoaderComponent self)
        {
            foreach (var kv in self.handlers)
            {
                switch (kv.Value)
                {
                    case AssetHandle handle:
                        handle.Release();
                        break;
                    case AllAssetsHandle handle:
                        handle.Release();
                        break;
                    case SubAssetsHandle handle:
                        handle.Release();
                        break;
                    case RawFileHandle handle:
                        handle.Release();
                        break;
                    case SceneHandle handle:
                        if (!handle.IsMainScene())
                        {
                            handle.UnloadAsync();
                        }
                        break;
                }
            }
        }

        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetAsync<T>(location);

                await handler.Task;

                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }

        public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAllAssetsAsync<T>(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in ((AllAssetsHandle)handler).AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            return dictionary;
        }

        public static async ETTask LoadSceneAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return;
            }

            handler = self.package.LoadSceneAsync(location);

            await handler.Task;
            self.handlers.Add(location, handler);
        }

        /// <summary>
        /// 异步流式加载场景，支持中断与恢复
        /// </summary>
        /// <param name="self">ResourcesLoaderComponent实例</param>
        /// <param name="location">场景资源路径</param>
        /// <param name="loadSceneMode">场景加载模式</param>
        /// <param name="progressCallback">加载进度回调</param>
        /// <param name="cancellationToken">取消令牌，用于中断加载</param>
        /// <returns></returns>
        public static async ETTask<bool> LoadSceneStreamingAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode, System.Action<float> progressCallback = null, System.Threading.CancellationToken cancellationToken = default)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return true;
            }

            SceneHandle sceneHandle = self.package.LoadSceneAsync(location);
            self.handlers.Add(location, sceneHandle);

            // 等待场景加载完成
            while (!sceneHandle.IsDone)
            {
                // 检查是否需要取消加载
                if (cancellationToken.IsCancellationRequested)
                {
                    sceneHandle.Cancel();
                    self.handlers.Remove(location);
                    return false;
                }

                // 调用进度回调
                if (progressCallback != null)
                {
                    progressCallback(sceneHandle.Progress);
                }

                // 等待一帧
                await ETTask.Yield();
            }

            // 检查加载是否成功
            if (sceneHandle.Status != EOperationStatus.Succeed)
            {
                self.handlers.Remove(location);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 中断正在进行的场景加载
        /// </summary>
        /// <param name="self">ResourcesLoaderComponent实例</param>
        /// <param name="location">场景资源路径</param>
        /// <returns></returns>
        public static bool CancelSceneLoad(this ResourcesLoaderComponent self, string location)
        {
            if (self.handlers.TryGetValue(location, out HandleBase handler))
            {
                if (handler is SceneHandle sceneHandle && !sceneHandle.IsDone)
                {
                    sceneHandle.Cancel();
                    self.handlers.Remove(location);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 异步流式加载资源，支持中断与恢复
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="self">ResourcesLoaderComponent实例</param>
        /// <param name="location">资源路径</param>
        /// <param name="progressCallback">加载进度回调</param>
        /// <param name="cancellationToken">取消令牌，用于中断加载</param>
        /// <returns></returns>
        public static async ETTask<T> LoadAssetStreamingAsync<T>(this ResourcesLoaderComponent self, string location, System.Action<float> progressCallback = null, System.Threading.CancellationToken cancellationToken = default) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return (T)((AssetHandle)handler).AssetObject;
            }

            AssetHandle assetHandle = self.package.LoadAssetAsync<T>(location);
            self.handlers.Add(location, assetHandle);

            // 等待资源加载完成
            while (!assetHandle.IsDone)
            {
                // 检查是否需要取消加载
                if (cancellationToken.IsCancellationRequested)
                {
                    assetHandle.Cancel();
                    self.handlers.Remove(location);
                    return null;
                }

                // 调用进度回调
                if (progressCallback != null)
                {
                    progressCallback(assetHandle.Progress);
                }

                // 等待一帧
                await ETTask.Yield();
            }

            // 检查加载是否成功
            if (assetHandle.Status != EOperationStatus.Succeed)
            {
                self.handlers.Remove(location);
                return null;
            }

            return (T)assetHandle.AssetObject;
        }
    }

    /// <summary>
    /// 用来管理资源，生命周期跟随Parent，比如CurrentScene用到的资源应该用CurrentScene的ResourcesLoaderComponent来加载
    /// 这样CurrentScene释放后，它用到的所有资源都释放了
    /// </summary>
    [ComponentOf]
    public class ResourcesLoaderComponent : Entity, IAwake, IAwake<string>, IDestroy
    {
        public ResourcePackage package;
        public Dictionary<string, HandleBase> handlers = new();
    }
}