using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Features.Ingredient.Editor
{
    public static class IngredientImageGenerator
    {
        const string PrefabFolder = "Assets/ThirdParty/Unity/Toony Kitchen Ingredients Free/Prefabs/Ingredients";
        const string OutputFolder = "Assets/Features/Ingredients/Art";
        const int Resolution = 512;
        const float Margin = 0.08f;    // extra space around the fitted bounds, fraction of size
        const float PitchDegrees = 30f; // camera pitch, like Unity's prefab preview
        const float YawDegrees = 30f;
        const float FieldOfView = 30f;

        [MenuItem("Tools/Generate Ingredient Images")]
        public static void GenerateAll()
        {
            Directory.CreateDirectory(OutputFolder);

            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No prefabs found in {PrefabFolder}");
                return;
            }

            // Fog and scene lighting would bleed into the transparent render;
            // save and restore the bits we change.
            bool fogWasEnabled = RenderSettings.fog;
            RenderSettings.fog = false;

            try
            {
                for (var i = 0; i < guids.Length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab == null)
                        continue;

                    EditorUtility.DisplayProgressBar("Ingredient Images", prefab.name, (float)i / guids.Length);
                    RenderPrefab(prefab, Path.Combine(OutputFolder, prefab.name + ".png"));
                }
            }
            finally
            {
                RenderSettings.fog = fogWasEnabled;
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            Debug.Log($"Generated {guids.Length} ingredient images into {OutputFolder}");
        }

        static void RenderPrefab(GameObject prefab, string outputPath)
        {
            // Park the instance far from the scene content so the isolated
            // camera only sees the ingredient.
            const float parkingDistance = -1000f;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = new Vector3(0f, parkingDistance, 0f);

            var cameraGo = new GameObject("IngredientCamera");
            var camera = cameraGo.AddComponent<Camera>();
            cameraGo.AddComponent<IngredientLightRig>();

            try
            {
                var bounds = ComputeBounds(instance);
                var center = bounds.center;

                camera.fieldOfView = FieldOfView;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 1000f;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.clear;
                camera.useOcclusionCulling = false;

                var rotation = Quaternion.Euler(PitchDegrees, YawDegrees, 0f);
                cameraGo.transform.rotation = rotation;
                // Fit the bounding sphere: distance so the whole object fits the FOV.
                var distance = bounds.extents.magnitude / Mathf.Tan(FieldOfView * 0.5f * Mathf.Deg2Rad) * (1f + Margin);
                cameraGo.transform.position = center + rotation * Vector3.back * distance;

                var rt = new RenderTexture(Resolution, Resolution, 24, RenderTextureFormat.ARGB32) { antiAliasing = 8 };
                var tex = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
                try
                {
                    // URP does not support Camera.Render(); SubmitRenderRequest is
                    // the supported way to capture a camera outside the frame loop.
                    var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
                    RenderPipeline.SubmitRenderRequest(camera, request);

                    RenderTexture.active = rt;
                    tex.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
                    tex.Apply();
                    File.WriteAllBytes(outputPath, tex.EncodeToPNG());
                }
                finally
                {
                    RenderTexture.active = null;
                    Object.DestroyImmediate(rt);
                    Object.DestroyImmediate(tex);
                }
            }
            finally
            {
                Object.DestroyImmediate(instance);
                Object.DestroyImmediate(cameraGo);
            }

            ImportAsUiTexture(outputPath);
        }

        static Bounds ComputeBounds(GameObject root)
        {
            var bounds = new Bounds();
            var hasBounds = false;
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!hasBounds)
                throw new System.InvalidOperationException($"{root.name} has no renderers to capture.");
            return bounds;
        }

        static void ImportAsUiTexture(string assetPath)
        {
            AssetDatabase.ImportAsset(assetPath);
            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureType = TextureImporterType.Default;
            importer.SaveAndReimport();
        }
    }

    /// Adds a standalone directional light so the render does not depend on
    /// whatever lights the open scene happens to have.
    class IngredientLightRig : MonoBehaviour
    {
        Light _light;

        void OnEnable()
        {
            var lightGo = new GameObject("IngredientLight");
            lightGo.transform.SetParent(transform, false);
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            _light = lightGo.AddComponent<Light>();
            _light.type = LightType.Directional;
            _light.intensity = 1f;
            _light.shadows = LightShadows.None;
        }

        void OnDisable()
        {
            if (_light != null)
                Object.DestroyImmediate(_light.gameObject);
        }
    }
}
