﻿using Core.Components;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _configDirectory;
    private readonly string _configFileName;
    private readonly ConfigManager _configManager;
    private readonly List<FileWatcher> _fileWatchers = [];
    private List<Config> _configs = [];

    public Controller()
    {
        _configDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _configFileName = "config.json";
        _configManager = new ConfigManager(_configDirectory + _configFileName);
    }



    public void Start()
    {
        var configWatcher = new ConfigWatcher(_configDirectory, _configFileName, OnConfigChanged);
        _logger.Debug($"Watching for config file '{_configFileName}' in {_configDirectory}");
        UpdateConfigs();
        SetFileWatchers();
        LogWatchersSummary();

        while (true)
            Thread.Sleep(1000);
    }

    internal void OnConfigChanged()
    {
        UpdateConfigs();
        SetFileWatchers();
        LogWatchersSummary();
    }

    internal void OnFileDetected(string fullPath)
    {
        throw new NotImplementedException(); // TODO: OnFileDetected
    }



    private void UpdateConfigs()
    {
        var configs = _configManager.GetConfigs();
        if (configs is null)
        {
            _logger.Warn("Failed to update configs.");
            _configs.Clear();
            return;
        }

        _configs = configs;
        _logger.Info("Configs updated.");
    }

    private void ClearFileWatchers()
    {
        for (int i = _fileWatchers.Count - 1; i >= 0; i--)
        {
            var watcher = _fileWatchers[i];
            _fileWatchers.Remove(watcher);
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }

    private void SetFileWatchers()
    {
        ClearFileWatchers();
        foreach (var config in _configs)
        {
            bool success = TryAddFileWatcher(config);
            if (!success)
            {
                // TODO: Lägg bevakning på parent-mappen och skapa filewatcher om mappen dyker upp.
            }
        }
    }

    private bool TryAddFileWatcher(Config config)
    {
        string folder = config.WatchFolder;
        if (!Directory.Exists(folder))
        {
            _logger.Warn($"Config '{config.Name}' tried to watch folder that does not exist: {folder}");
            return false;
        }

        var watcher = new FileWatcher(folder, config.Filter, OnFileDetected);
        _fileWatchers.Add(watcher);
        return true;
    }

    private void LogWatchersSummary()
    {
        string filterList = (_configs.Count > 0)
            ? string.Join("", _configs.Select(c => $"\n  '{c.Filter}' in {c.WatchFolder}"))
            : "(nothing)";
        _logger.Info($"Currently watching for: {filterList}");
    }
}
