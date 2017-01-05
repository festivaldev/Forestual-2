﻿using F2Core.Compatibility;

namespace Forestual2CS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 1;
        public override int Patch { get; set; } = 13;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.beta;
        public override string ReleaseDate { get; set; } = "17w01";
        public override string Commit { get; set; } = "13182df";
        public override string SupportedVersion { get; set; } = "2.0.3 [7c89075]";
    }
}
