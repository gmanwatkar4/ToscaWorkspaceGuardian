namespace ToscaWorkspaceGuardian.Core.Upgrade;

public sealed class UpgradeRiskRuleCatalog
{
    public IReadOnlyList<UpgradeRiskRule> GetRules(string sourceVersion, string targetVersion)
    {
        if (!sourceVersion.Equals("2025.1", StringComparison.OrdinalIgnoreCase)
            || !targetVersion.Equals("2026.1", StringComparison.OrdinalIgnoreCase))
        {
            return Array.Empty<UpgradeRiskRule>();
        }

        return new[]
        {
            new UpgradeRiskRule(
                "WG-2026-TQL-001",
                "Warning",
                "Legacy TBox Iterate Array attribute",
                "=>SUBPARTS:XModuleAttribute[(Name==\"Array To Iterate\")OR(Name==\"Target Buffer\")]",
                "A legacy TBox Iterate Array attribute was found. Tosca 2026.1 introduces the Array ModuleAttribute model.",
                "Update the affected module to the 2026.1 Array ModuleAttribute and validate every linked test case."),
            new UpgradeRiskRule(
                "WG-2026-TQL-002",
                "Advisory",
                "SeaLights configuration present",
                "=>SUBPARTS:TCConfiguration[Name==\"SeaLights\"]",
                "A SeaLights configuration was found in the workspace.",
                "Review the 2026.1 integration notes and perform a post-upgrade SeaLights connectivity validation."),
            new UpgradeRiskRule(
                "WG-2026-TQL-003",
                "Advisory",
                "TestCase Design templates present",
                "=>SUBPARTS:TestCaseTemplate",
                "TestCase Design templates were found in the workspace.",
                "If the workflow uploads TestCase Design Classes to Tosca cloud, review the 2026.1.1 support limitations before rollout."),
        };
    }
}
