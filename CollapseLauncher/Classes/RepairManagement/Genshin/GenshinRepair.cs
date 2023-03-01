﻿using CollapseLauncher.Interfaces;
using Hi3Helper.Preset;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using static Hi3Helper.Data.ConverterTool;
using static Hi3Helper.Locale;

namespace CollapseLauncher
{
    public enum GenshinAudioLanguage : int
    {
        English = 0,
        Chinese = 1,
        Japanese = 2,
        Korean = 3
    }

    internal partial class GenshinRepair :
        ProgressBase<RepairAssetType, PkgVersionProperties>, IRepair
    {
        #region ExtensionProperties
        private protected string _execPrefix { get => Path.GetFileNameWithoutExtension(_gamePreset.GameExecutableName); }
        private protected int _dispatcherRegionID { get; init; }
        private protected string _dispatcherURL { get => _gamePreset.GameDispatchURL ?? ""; }
        private protected string _dispatcherKey { get => _gamePreset.DispatcherKey ?? ""; }
        private protected int _dispatcherKeyLength { get => _gamePreset.DispatcherKeyBitLength ?? 0x100; }
        private protected GenshinAudioLanguage _audioLanguage { get; init; }
        #endregion

        public GenshinRepair(UIElement parentUI, string gameVersion, string gamePath,
            string gameRepoURL, PresetConfigV2 gamePreset, byte repairThread, byte downloadThread)
            : base(parentUI, gameVersion, gamePath, gameRepoURL, gamePreset, repairThread, downloadThread)
        {
            _audioLanguage = (GenshinAudioLanguage)_gamePreset.GetVoiceLanguageID();
            _dispatcherRegionID = _gamePreset.GetRegServerNameID();
        }

        ~GenshinRepair() => Dispose();

        public async Task<bool> StartCheckRoutine()
        {
            return await TryRunExamineThrow(CheckRoutine());
        }

        public async Task StartRepairRoutine(bool showInteractivePrompt = false)
        {
            if (_assetIndex.Count == 0) throw new InvalidOperationException("There's no broken file being reported! You can't do the repair process!");

            _ = await TryRunExamineThrow(RepairRoutine());
        }

        private async Task<bool> CheckRoutine()
        {
            // Reset status and progress
            ResetStatusAndProgress();
            _assetIndex.Clear();

            // Step 1: Ensure that every files are not read-only
            TryUnassignReadOnlyFiles();

            // Step 2: Fetch asset index
            await Fetch(_assetIndex, _token.Token);

            // Step 3: Calculate all the size and count in total
            CountAssetIndex(_assetIndex);

            // Step 4: Check for the asset indexes integrity
            await Check(_assetIndex, _token.Token);

            // Step 5: Summarize and returns true if the assetIndex count != 0 indicates broken file was found.
            //         either way, returns false.
            return SummarizeStatusAndProgress(
                _assetIndex,
                string.Format(Lang._GameRepairPage.Status3, _progressTotalCountFound, SummarizeSizeSimple(_progressTotalSizeFound)),
                Lang._GameRepairPage.Status4);
        }

        private async Task<bool> RepairRoutine()
        {
            // Restart Stopwatch
            RestartStopwatch();

            // Assign repair task
            Task<bool> repairTask = Repair(_assetIndex, _token.Token);

            // Run repair process
            bool repairTaskSuccess = await TryRunExamineThrow(repairTask);

            // Reset status and progress
            ResetStatusAndProgress();

            // Set as completed
            _status.IsCompleted = true;
            _status.IsCanceled = false;
            _status.ActivityStatus = Lang._GameRepairPage.Status7;

            // Update status and progress
            UpdateAll();

            return repairTaskSuccess;
        }

        public void CancelRoutine()
        {
            // Trigger token cancellation
            _token.Cancel();
        }

        public void Dispose()
        {
            CancelRoutine();
        }
    }
}
