namespace Panther.Commands
{
    internal class RebuildPantherCommon : BuildPantherCommon
    {
        public RebuildPantherCommon() => IsRebuild = true;

        public override string GetText() => MenuTexts.RebuildPantherCommon;
    }
}
