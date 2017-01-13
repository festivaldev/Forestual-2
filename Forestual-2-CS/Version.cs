using F2Core.Compatibility;

namespace Forestual2CS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 2;
        public override int Patch { get; set; } = 34;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.none;
        public override string ReleaseDate { get; set; } = "17w01";
        public override string Commit { get; set; } = "b506f75";
        public override string SupportedVersion { get; set; } = "2.1.16 [e2c2e08]";
    }
}
