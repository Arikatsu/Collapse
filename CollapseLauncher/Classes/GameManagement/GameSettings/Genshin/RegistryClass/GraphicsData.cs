﻿using CollapseLauncher.GameSettings.Genshin.Context;
using CollapseLauncher.GameSettings.Genshin.Enums;
using Hi3Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static Hi3Helper.Logger;

namespace CollapseLauncher.GameSettings.Genshin
{
    internal class GraphicsData
    {
        #region Properties
        // Generate the list of the FPSOption value and order by ascending it.
        public static readonly FPSOption[] FPSOptionsList = Enum.GetValues<FPSOption>().OrderBy(GetFPSOptionNumber).ToArray();
        // Generate the list of the FPS number to be displayed on FPS Combobox
        public static readonly int[] FPSIndex = FPSOptionsList.Select(GetFPSOptionNumber).ToArray();

        private static int GetFPSOptionNumber(FPSOption value)
        {
            // Get the string of the number by trimming the 'f' letter at the beginning
            string fpsStrNum = value.ToString().TrimStart('f');
            // Try parse the fpsStrNum as a number
            _ = int.TryParse(fpsStrNum, out int number);
            // Return the number
            return number;
        }

        public static readonly decimal[] RenderScaleIndex = new decimal[] { 0.6m, 0.8m, 1.0m, 1.1m, 1.2m, 1.3m, 1.4m, 1.5m };
        public int currentVolatielGrade { get; set; } = -1;
        public List<Dictionary<string, int>> customVolatileGrades { get; set; } = new();
        public string volatileVersion { get; set; } = "";
        #endregion

        #region Settings
        /// <summary>
        /// This defines "<c>FPS</c>" combobox In-game settings. <br/>
        /// Options: 30, 60, 45
        /// Default: 60 [1]
        /// </summary>
        public FPSOption FPS = FPSOption.f60;

        /// <summary>
        /// This defines "<c>Render Resolution</c>" combobox In-game settings. <br/>
        /// Options: 0.6, 0.8, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5
        /// Default: 1.0 [3]
        /// </summary>
        public RenderResolutionOption RenderResolution = RenderResolutionOption.x10;

        /// <summary>
        /// This defines "<c>Shadow Quality</c>" combobox In-game settings. <br/>
        /// Options: Lowest, Low, Medium, High
        /// Default: High [4]
        /// </summary>
        public ShadowQualityOption ShadowQuality = ShadowQualityOption.High;

        /// <summary>
        /// This defines "<c>Visual Effects</c>" combobox In-game settings. <br/>
        /// Options: Lowest, Low, Medium, High
        /// Default: High [4]
        /// </summary>
        public VisualEffectsOption VisualEffects = VisualEffectsOption.High;

        /// <summary>
        /// This defines "<c>SFX Quality</c>" combobox In-game settings. <br/>
        /// Options: Lowest, Low, Medium, High
        /// Default: High [4]
        /// </summary>
        public SFXQualityOption SFXQuality = SFXQualityOption.High;

        /// <summary>
        /// This defines "<c>Environment Detail</c>" combobox In-game settings. <br/>
        /// Options: Lowest, Low, Medium, High, Highest
        /// Default: High [4]
        /// </summary>
        public EnvironmentDetailOption EnvironmentDetail = EnvironmentDetailOption.High;

        /// <summary>
        /// This defines "<c>Vertical Sync</c>" combobox In-game settings. <br/>
        /// Options: Off, On
        /// Default: On [2]
        /// </summary>
        public VerticalSyncOption VerticalSync = VerticalSyncOption.On;

        /// <summary>
        /// This defines "<c>Antialiasing</c>" combobox In-game settings. <br/>
        /// Options: Off, FSR 2, SMAA
        /// Default: FSR 2 [2]
        /// </summary>
        public AntialiasingOption Antialiasing = AntialiasingOption.FSR2;

        /// <summary>
        /// This defines "<c>Volumetric Fog</c>" combobox In-game settings. <br/>
        /// Options: Off, On
        /// Default: On [2]
        /// Game prohibits enabling this if "<c>Shadow Quality</c>" on Low or Lowest
        /// </summary>
        public VolumetricFogOption VolumetricFog = VolumetricFogOption.On;

        /// <summary>
        /// This defines "<c>Reflections</c>" combobox In-game settings. <br/>
        /// Options: Off, On
        /// Default: On [2]
        /// </summary>
        public ReflectionsOption Reflections = ReflectionsOption.On;

        /// <summary>
        /// This defines "<c>Motion Blur</c>" combobox In-game settings. <br/>
        /// Options: Off, Low, High, Extreme
        /// Default: Extreme [4]
        /// </summary>
        public MotionBlurOption MotionBlur = MotionBlurOption.Extreme;

        /// <summary>
        /// This defines "<c>Bloom</c>" combobox In-game settings. <br/>
        /// Options: Off, On
        /// Default: On [2]
        /// </summary>
        public BloomOption Bloom = BloomOption.On;

        /// <summary>
        /// This defines "<c>Crowd Density</c>" combobox In-game settings. <br/>
        /// Options: Low, High
        /// Default: High [2]
        /// </summary>
        public CrowdDensityOption CrowdDensity = CrowdDensityOption.High;

        /// <summary>
        /// This defines "<c>Subsurface Scattering</c>" combobox In-game settings. <br/>
        /// Options: Off, Medium, High
        /// Default: High [3]
        /// </summary>
        public SubsurfaceScatteringOption SubsurfaceScattering = SubsurfaceScatteringOption.High;

        /// <summary>
        /// This defines "<c>Co-Op Teammate Effects</c>" combobox In-game settings. <br/>
        /// Options: Off, Partially Off, On
        /// Default: On [3]
        /// </summary>
        public CoOpTeammateEffectsOption CoOpTeammateEffects = CoOpTeammateEffectsOption.On;

        /// <summary>
        /// This defines "<c>Anisotropic Filtering</c>" combobox In-game settings. <br/>
        /// Options: 1x, 2x, 4x, 8x, 16x
        /// Default: 8x [4]
        /// </summary>
        public AnisotropicFilteringOption AnisotropicFiltering = AnisotropicFilteringOption.x8;
        #endregion

        #region Methods
        public static GraphicsData Load(string graphicsJson)
        {
            GraphicsData graphics = (GraphicsData?)JsonSerializer.Deserialize(graphicsJson, typeof(GraphicsData), GraphicsDataContext.Default) ?? new GraphicsData();
            foreach (Dictionary<string, int> setting in graphics.customVolatileGrades)
            {
                switch (setting["key"])
                {
                    case 1:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - FPS: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.FPS = (FPSOption)setting["value"];
                        break;

                    case 2:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Render Resolution: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.RenderResolution = (RenderResolutionOption)setting["value"];
                        break;

                    case 3:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Shadow Quality: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.ShadowQuality = (ShadowQualityOption)setting["value"];
                        break;

                    case 4:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Visual Effects: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.VisualEffects = (VisualEffectsOption)setting["value"];
                        break;

                    case 5:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - SFX Quality: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.SFXQuality = (SFXQualityOption)setting["value"];
                        break;

                    case 6:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Environment Detail: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.EnvironmentDetail = (EnvironmentDetailOption)setting["value"];
                        break;

                    case 7:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Vertical Sync: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.VerticalSync = (VerticalSyncOption)setting["value"];
                        break;

                    case 8:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Antialiasing: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.Antialiasing = (AntialiasingOption)setting["value"];
                        break;

                    case 9:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Volumetric Fog: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.VolumetricFog = (VolumetricFogOption)setting["value"];
                        break;

                    case 10:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Reflections: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.Reflections = (ReflectionsOption)setting["value"];
                        break;

                    case 11:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Motion Blur: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.MotionBlur = (MotionBlurOption)setting["value"];
                        break;

                    case 12:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Bloom: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.Bloom = (BloomOption)setting["value"];
                        break;

                    case 13:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Crowd Density: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.CrowdDensity = (CrowdDensityOption)setting["value"];
                        break;

                    // 14 is missing from settings
                    // And yes, do not reorder this unless the game order finally changes
                    // It is meant to be like this because miyoyo
                    case 16:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Co-Op Teammate Effects: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.CoOpTeammateEffects = (CoOpTeammateEffectsOption)setting["value"];
                        break;

                    case 15:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Subsurface Scattering: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.SubsurfaceScattering = (SubsurfaceScatteringOption)setting["value"];
                        break;

                    case 17:
#if DEBUG
                        LogWriteLine($"Loaded Genshin Settings: Graphics - Anisotropic Filtering: {setting["value"]}", LogType.Debug, true);
#endif
                        graphics.AnisotropicFiltering = (AnisotropicFilteringOption)setting["value"];
                        break;
                }
            }
            return graphics;
        }

        public string Save()
        {
            customVolatileGrades = new()
            {
                new Dictionary<string, int>() { { "key", 1 }, { "value", (int)FPS } },
                new Dictionary<string, int>() { { "key", 2 }, { "value", (int)RenderResolution } },
                new Dictionary<string, int>() { { "key", 3 }, { "value", (int)ShadowQuality } },
                new Dictionary<string, int>() { { "key", 4 }, { "value", (int)VisualEffects } },
                new Dictionary<string, int>() { { "key", 5 }, { "value", (int)SFXQuality } },
                new Dictionary<string, int>() { { "key", 6 }, { "value", (int)EnvironmentDetail } },
                new Dictionary<string, int>() { { "key", 7 }, { "value", (int)VerticalSync } },
                new Dictionary<string, int>() { { "key", 8 }, { "value", (int)Antialiasing } },
                new Dictionary<string, int>() { { "key", 9 }, { "value", (int)VolumetricFog } },
                new Dictionary<string, int>() { { "key", 10 }, { "value", (int)Reflections } },
                new Dictionary<string, int>() { { "key", 11 }, { "value", (int)MotionBlur } },
                new Dictionary<string, int>() { { "key", 12 }, { "value", (int)Bloom } },
                new Dictionary<string, int>() { { "key", 13 }, { "value", (int)CrowdDensity } },
                new Dictionary<string, int>() { { "key", 16 }, { "value", (int)CoOpTeammateEffects } },
                new Dictionary<string, int>() { { "key", 15 }, { "value", (int)SubsurfaceScattering } },
                new Dictionary<string, int>() { { "key", 17 }, { "value", (int)AnisotropicFiltering } },
                };
            string data = JsonSerializer.Serialize(this, typeof(GraphicsData), GraphicsDataContext.Default);
#if DEBUG
            LogWriteLine($"Saved Genshin GraphicsData\r\n{data}", LogType.Debug, true);
#endif
            return data;
        }
        #endregion
    }
}
