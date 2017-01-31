using F2Core.Compatibility;

namespace Forestual2CS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 2;
        public override int Patch { get; set; } = 41;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.none;
        public override string ReleaseDate { get; set; } = "17w05";
        public override string Commit { get; set; } = "cf4640d";
        public override string SupportedVersion { get; set; } = "2.2.0-alpha [b236067]";
    }
}
