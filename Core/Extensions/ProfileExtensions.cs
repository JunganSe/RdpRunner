﻿using Core.Configuration;
using Core.Helpers;

namespace Core.Extensions;

internal static class ProfileExtensions
{
    public static void RemoveDisabledProfiles(this List<Profile> profiles) 
        => profiles.RemoveAll(p => !p.Enabled);

    public static void RemoveInvalidProfiles(this List<Profile> profiles) 
        => profiles.RemoveAll(p => !p.IsValid(out _));

    public static bool IsValid(this Profile profile, out string reason) 
        => ProfileHelper.IsProfileValid(profile, out reason);
}
