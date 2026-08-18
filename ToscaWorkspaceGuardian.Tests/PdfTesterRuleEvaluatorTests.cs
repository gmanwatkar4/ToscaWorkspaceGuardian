using PdfTester;
using PdfTester.Models;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace ToscaWorkspaceGuardian.Tests;

public sealed class PdfTesterRuleEvaluatorTests
{
    [Fact]
    public void Evaluate_SupportedRules_ReturnsExpectedResults()
    {
        var pdf = new ExtractedPdf(["Invoice INV-12345", "Payment complete"]);
        var rules = new RuleSet
        {
            Rules =
            [
                new() { Id = "heading", Type = "contains", Value = "invoice", Page = 1, IgnoreCase = true },
                new() { Id = "number", Type = "regex", Value = "INV-[0-9]+" },
                new() { Id = "draft", Type = "notContains", Value = "DRAFT" },
                new() { Id = "pages", Type = "pageCount", Min = 2, Max = 2 },
            ],
        };

        var results = new RuleEvaluator().Evaluate(rules, pdf);

        Assert.All(results, result => Assert.True(result.Passed));
    }

    [Fact]
    public void Evaluate_PageOutsideDocument_ThrowsInvalidDataException()
    {
        var pdf = new ExtractedPdf(["Page one"]);
        var rules = new RuleSet
        {
            Rules = [new() { Id = "missing-page", Type = "contains", Value = "text", Page = 2 }],
        };

        var exception = Assert.Throws<InvalidDataException>(() => new RuleEvaluator().Evaluate(rules, pdf));

        Assert.Contains("page 2", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Evaluate_AdvancedTextRules_ReturnsExpectedCountsWithoutTextDisclosure()
    {
        var pdf = new ExtractedPdf(["Invoice   Total Invoice", "Payment Date"]);
        var rules = new RuleSet
        {
            Rules =
            [
                new() { Id = "any", Type = "containsAny", Values = ["Missing", "Total"] },
                new() { Id = "all", Type = "containsAll", Values = ["Invoice", "Payment", "Date"] },
                new() { Id = "count", Type = "textCount", Value = "invoice", Min = 2, Max = 2, IgnoreCase = true },
                new() { Id = "length", Type = "pageTextLength", Page = 1, Min = 20, NormalizeWhitespace = true },
            ],
        };

        var results = new RuleEvaluator().Evaluate(rules, pdf);

        Assert.All(results, result => Assert.True(result.Passed));
        Assert.All(results, result => Assert.DoesNotContain("Payment Date", result.Message, StringComparison.Ordinal));
    }

    [Fact]
    public void Evaluate_ContainsWithWhitespaceNormalization_Passes()
    {
        var pdf = new ExtractedPdf(["Invoice\r\n    Number"]);
        var rules = new RuleSet
        {
            Rules = [new() { Id = "normalized", Type = "contains", Value = "Invoice Number", NormalizeWhitespace = true }],
        };

        var result = Assert.Single(new RuleEvaluator().Evaluate(rules, pdf));

        Assert.True(result.Passed);
    }

    [Fact]
    public void Evaluate_RegionAndCaptureRules_RespectCoordinatesAndRedaction()
    {
        var page = new PdfPageContent(
            "Invoice INV-12345 Total 99.00",
            600,
            800,
            [
                new PdfWord("Invoice", 50, 700, 100, 715),
                new PdfWord("INV-12345", 110, 700, 180, 715),
                new PdfWord("Total", 400, 100, 440, 115),
                new PdfWord("99.00", 450, 100, 490, 115),
            ]);
        var pdf = new ExtractedPdf([page]);
        var rules = new RuleSet
        {
            Rules =
            [
                new()
                {
                    Id = "header",
                    Type = "regionContains",
                    Value = "Invoice",
                    Page = 1,
                    Region = new PdfRegion { X = 0, Y = 650, Width = 300, Height = 100 },
                },
                new()
                {
                    Id = "invoice-number",
                    Type = "captureRegex",
                    Value = "INV-([0-9]+)",
                    CaptureGroup = 1,
                    Page = 1,
                    Region = new PdfRegion { X = 0, Y = 650, Width = 300, Height = 100 },
                },
            ],
        };

        var results = new RuleEvaluator().Evaluate(rules, pdf);

        Assert.All(results, result => Assert.True(result.Passed));
        Assert.True(results[1].ValueRedacted);
        Assert.Null(results[1].CapturedValue);
        Assert.DoesNotContain("12345", results[1].Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Evaluate_CaptureRegex_ExposesValueOnlyWhenRequested()
    {
        var pdf = new ExtractedPdf(["Invoice INV-12345"]);
        var rules = new RuleSet
        {
            Rules =
            [
                new()
                {
                    Id = "invoice-number",
                    Type = "captureRegex",
                    Value = "INV-([0-9]+)",
                    CaptureGroup = 1,
                    ExposeValue = true,
                },
            ],
        };

        var result = Assert.Single(new RuleEvaluator().Evaluate(rules, pdf));

        Assert.Equal("12345", result.CapturedValue);
        Assert.False(result.ValueRedacted);
    }

    [Fact]
    public void Evaluate_RepetitiveRegion_HonorsPageExclusions()
    {
        var words = new[] { new PdfWord("CONFIDENTIAL", 10, 10, 100, 25) };
        var pdf = new ExtractedPdf(
        [
            new PdfPageContent("CONFIDENTIAL", 200, 200, words),
            new PdfPageContent("", 200, 200, []),
            new PdfPageContent("CONFIDENTIAL", 200, 200, words),
        ]);
        var rules = new RuleSet
        {
            Rules =
            [
                new()
                {
                    Id = "repeated-footer",
                    Type = "repetitiveRegionContains",
                    Value = "CONFIDENTIAL",
                    Pages = "all",
                    ExcludedPages = "2",
                    Region = new PdfRegion { X = 0, Y = 0, Width = 150, Height = 50 },
                },
            ],
        };

        var result = Assert.Single(new RuleEvaluator().Evaluate(rules, pdf));

        Assert.True(result.Passed);
        Assert.Contains("2 of 2", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Evaluate_AnchorRelativeRegion_FindsUniqueAnchorAndTarget()
    {
        var pdf = new ExtractedPdf(
        [
            new PdfPageContent(
                "Total 99.00",
                600,
                800,
                [
                    new PdfWord("Total", 100, 200, 140, 215),
                    new PdfWord("99.00", 160, 200, 200, 215),
                ]),
        ]);
        var rules = new RuleSet
        {
            Rules =
            [
                new()
                {
                    Id = "relative-total",
                    Type = "anchorRegionContains",
                    Page = 1,
                    Anchor = new TextAnchor { Text = "Total", Accuracy = 100 },
                    RelativeRegion = new RelativeRegion { OffsetX = 50, OffsetY = -10, Width = 100, Height = 40 },
                    Value = "99.00",
                },
            ],
        };

        var result = Assert.Single(new RuleEvaluator().Evaluate(rules, pdf));

        Assert.True(result.Passed);
    }

    [Fact]
    public void Evaluate_DuplicateAnchor_IsRejected()
    {
        var pdf = new ExtractedPdf(
        [
            new PdfPageContent(
                "Total Total",
                600,
                800,
                [
                    new PdfWord("Total", 100, 200, 140, 215),
                    new PdfWord("Total", 300, 200, 340, 215),
                ]),
        ]);
        var rules = new RuleSet
        {
            Rules =
            [
                new()
                {
                    Id = "duplicate",
                    Type = "anchorRegionContains",
                    Page = 1,
                    Anchor = new TextAnchor { Text = "Total", Accuracy = 100 },
                    RelativeRegion = new RelativeRegion { Width = 50, Height = 20 },
                    Value = "x",
                },
            ],
        };

        var exception = Assert.Throws<InvalidDataException>(() => new RuleEvaluator().Evaluate(rules, pdf));

        Assert.Contains("not unique", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("odd", null, 5, new[] { 1, 3, 5 })]
    [InlineData("all", "2;4-5", 5, new[] { 1, 3 })]
    [InlineData("1;3-4", null, 5, new[] { 1, 3, 4 })]
    public void PageSelectionParser_ValidSelections_ReturnExpectedPages(
        string selection,
        string? exclusions,
        int pageCount,
        int[] expected)
    {
        var actual = PageSelectionParser.Select(selection, exclusions, pageCount);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Parse_HttpPdfUrl_IsRejected()
    {
        var args = new[] { "verify", "--pdf", "https://example.test/file.pdf", "--rules", "rules.json" };

        var exception = Assert.Throws<ArgumentException>(() => CommandLineOptions.Parse(args));

        Assert.Contains("URLs are not accepted", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_OutputOverRulesFile_IsRejectedBeforeProcessing()
    {
        var options = new CommandLineOptions("input.pdf", "rules.json", "rules.json", CommandLineOptions.DefaultMaxFileBytes);

        var exception = Assert.Throws<InvalidDataException>(() => new PdfVerificationRunner().Run(options));

        Assert.Contains("must not overwrite", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_RealPdf_WritesPassingReport()
    {
        var testDirectory = Path.Combine(Path.GetTempPath(), $"pdf-tester-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        try
        {
            var pdfPath = Path.Combine(testDirectory, "invoice.pdf");
            var rulesPath = Path.Combine(testDirectory, "rules.json");
            var outputPath = Path.Combine(testDirectory, "result.json");
            var builder = new PdfDocumentBuilder();
            var font = builder.AddStandard14Font(Standard14Font.Helvetica);
            var page = builder.AddPage(PageSize.A4);
            page.AddText("Invoice INV-12345", 12, new PdfPoint(50, 750), font);
            File.WriteAllBytes(pdfPath, builder.Build());
            File.WriteAllText(rulesPath, """
                {
                  "rules": [
                    { "id": "heading", "type": "contains", "value": "Invoice", "page": 1 },
                    { "id": "number", "type": "regex", "value": "INV-[0-9]+" }
                  ]
                }
                """);
            var options = new CommandLineOptions(pdfPath, rulesPath, outputPath, CommandLineOptions.DefaultMaxFileBytes);

            var report = new PdfVerificationRunner().Run(options);
            PdfVerificationRunner.WriteReport(report, outputPath);

            Assert.True(report.Passed);
            Assert.Equal(1, report.PageCount);
            Assert.True(File.Exists(outputPath));
            Assert.DoesNotContain("Invoice INV-12345", File.ReadAllText(outputPath), StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }
}
