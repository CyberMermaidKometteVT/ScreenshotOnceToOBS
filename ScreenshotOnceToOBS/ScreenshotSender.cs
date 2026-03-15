using System.IO;

using Newtonsoft.Json.Linq;

namespace ScreenshotOnceToOBS
{
    internal class ScreenshotSender
    {
        // Provided by StreamerBot at runtime
        private readonly dynamic CPH;

        #region JObject property names
        private const string ArgConnection = "connection";
        private const string ArgDefaultScale = "defaultScale";
        private const string ArgFullPath = "fullPath";
        private const string ArgFileName = "fileName";

        private const string ReqSetInputSettings = "SetInputSettings";
        private const string ReqGetSceneItemId = "GetSceneItemId";
        private const string ReqSetSceneItemTransform = "SetSceneItemTransform";
        private const string ReqCreateInput = "CreateInput";

        private const string PropInputName = "inputName";
        private const string PropInputKind = "inputKind";
        private const string PropInputSettings = "inputSettings";
        private const string PropFile = "file";
        private const string PropUnload = "unload";
        private const string PropSceneName = "sceneName";
        private const string PropSourceName = "sourceName";
        private const string PropSceneItemId = "sceneItemId";
        private const string PropSceneItemTransform = "sceneItemTransform";

        private const string PropAlignment = "alignment";
        private const string PropPositionX = "positionX";
        private const string PropPositionY = "positionY";
        private const string PropScaleX = "scaleX";
        private const string PropScaleY = "scaleY";

        private const string InputKindImage = "image_source";
        #endregion

        public ScreenshotSender(dynamic cph)
        {
            CPH = cph;
        }

        public bool Execute(string fullPath)
        {
            // Get arguments
            CPH.LogInfo("ScreenshotSender: Getting arguments...");
            CPH.TryGetArg(ArgConnection, out int connection);
            CPH.TryGetArg(ArgDefaultScale, out decimal defaultScale);

            CPH.LogDebug($"ScreenshotSender: Arguments were: connection={connection}, defaultScale={defaultScale}");
            string fileName = Path.GetFileName(fullPath);

            CPH.Wait(250);

            // Create an image source on the current scene
            CreateSource(connection, fileName);

            // Set the path of the image for the created image source
            SetSourcePath(connection, fileName, fullPath);

            //// Find the sceneItemId of the source within the current scene
            //// (but do not, because I don't need it if I'm not setting the transform)
            //int sceneItemId = GetSourceSceneId(connection, fileName);

            //// Set the transform of the image
            //// (but do not, because I actually prefer it in the top left corner)
            //SetSceneItemTransform(connection, defaultScale, sceneItemId);

            return true;
        }

        private void CreateSource(int connection, string fileName)
        {
            CPH.LogInfo("ScreenshotSender: Creating image source...");
            JObject request = new JObject
            {
                [PropInputName] = fileName,
                [PropInputKind] = InputKindImage,
                [PropSceneName] = CPH.ObsGetCurrentScene(connection)
            };

            CPH.ObsSendRaw(ReqCreateInput, request.ToString(), connection);
        }

        private void SetSourcePath(int connection, string fileName, string fullPath)
        {
            CPH.LogInfo("ScreenshotSender: Setting its path...");
            JObject request = new JObject
            {
                [PropInputName] = fileName,
                [PropInputSettings] = new JObject
                {
                    [PropFile] = fullPath,
                    [PropUnload] = true
                }
            };
            CPH.ObsSendRaw(ReqSetInputSettings, request.ToString(), connection);
        }

        private int GetSourceSceneId(int connection, string fileName)
        {
            CPH.LogDebug("ScreenshotSender: Getting its ID...");
            JObject request = new JObject
            {
                [PropSceneName] = CPH.ObsGetCurrentScene(connection),
                [PropSourceName] = fileName
            };
            var sceneItemIdResponse = CPH.ObsSendRaw(ReqGetSceneItemId, request.ToString(), connection);
            JObject response = JObject.Parse(sceneItemIdResponse);

            var sceneItemId = response.Value<int>(PropSceneItemId);

            CPH.LogDebug($"ScreenshotSender: ...it's {sceneItemId}");
            return sceneItemId;
        }

        private void SetSceneItemTransform(int connection, decimal defaultScale, int sceneItemId)
        {
            CPH.LogDebug("ScreenshotSender: Setting its transform...");
            JObject request = new JObject
            {
                [PropSceneName] = CPH.ObsGetCurrentScene(connection),
                [PropSceneItemId] = sceneItemId,
                [PropSceneItemTransform] = new JObject
                {
                    [PropAlignment] = 0,
                    [PropPositionX] = 0,
                    [PropPositionY] = 0,
                    [PropScaleX] = defaultScale,
                    [PropScaleY] = defaultScale
                }
            };

            CPH.ObsSendRaw(ReqSetSceneItemTransform, request.ToString(), connection);
        }
    }
}