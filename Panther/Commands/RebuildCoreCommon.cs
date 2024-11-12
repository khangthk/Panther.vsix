namespace Panther.Commands
{
    internal class RebuildCoreCommon : BuildCoreCommon
    {
        public RebuildCoreCommon() => IsRebuild = true;

        public override string GetText() => MenuTexts.RebuildCoreCommon;
    }
}
