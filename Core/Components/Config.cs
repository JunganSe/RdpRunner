﻿namespace Core.Components;

internal class Config
{
    public string Name { get; set; }
    public string WatchFolder { get; init; }
    public string Filter { get; init; }
    public string MoveToFolder { get; init; }
    public bool Launch { get; set; }
    public bool Delete { get; init; }
    public Dictionary<string, string> Settings { get; init; }

    public Config()
    {
        Name = "";
        WatchFolder = "";
        Filter = "";
        MoveToFolder = "";
        Launch = false;
        Delete = false;
        Settings = [];
    }
}
