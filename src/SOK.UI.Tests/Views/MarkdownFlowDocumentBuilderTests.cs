using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using SOK.UI.Services;
using Xunit;

namespace SOK.UI.Tests.Views;

public sealed class MarkdownFlowDocumentBuilderTests
{
    [Fact]
    public void Build_GeneraEncabezadosParrafosYEnlaces() => RunInSta(() =>
    {
        var document = MarkdownFlowDocumentBuilder.Build(
            """
            # Título principal

            Texto con **negrita**, *cursiva*, `código` y un [enlace](https://example.com).
            """);

        var paragraphs = document.Blocks.OfType<Paragraph>().ToList();
        Assert.True(paragraphs.Count >= 2);
        Assert.Equal(24, paragraphs[0].FontSize);

        var runs = paragraphs
            .SelectMany(paragraph => Descendants<Run>(paragraph.Inlines))
            .ToList();
        Assert.Contains(runs, run => run.FontWeight == FontWeights.Bold && run.Text == "negrita");
        Assert.Contains(runs, run => run.FontStyle == FontStyles.Italic && run.Text == "cursiva");
        Assert.Contains(runs, run => run.FontFamily.Source == "Consolas" && run.Text == "código");

        var hyperlink = paragraphs
            .SelectMany(paragraph => Descendants<Hyperlink>(paragraph.Inlines))
            .Single();
        Assert.Equal("https://example.com/", hyperlink.NavigateUri?.AbsoluteUri);
    });

    [Fact]
    public void Build_GeneraListasConViñetasYNumeradas() => RunInSta(() =>
    {
        var document = MarkdownFlowDocumentBuilder.Build(
            """
            - Primer elemento
            - Segundo elemento

            1. Uno
            2. Dos
            """);

        var lists = document.Blocks.OfType<List>().ToList();
        Assert.Equal(2, lists.Count);
        Assert.Equal(TextMarkerStyle.Disc, lists[0].MarkerStyle);
        Assert.Equal(2, lists[0].ListItems.Count);
        Assert.Equal(TextMarkerStyle.Decimal, lists[1].MarkerStyle);
        Assert.Equal(2, lists[1].ListItems.Count);
    });

    [Fact]
    public void Build_GeneraTablasConEncabezado() => RunInSta(() =>
    {
        var document = MarkdownFlowDocumentBuilder.Build(
            """
            | Columna A | Columna B |
            | --- | --- |
            | Valor 1 | Valor 2 |
            | Valor 3 | Valor 4 |
            """);

        var table = document.Blocks.OfType<Table>().Single();
        Assert.Equal(2, table.Columns.Count);
        Assert.Single(table.RowGroups);
        Assert.Equal(3, table.RowGroups[0].Rows.Count);

        var header = table.RowGroups[0].Rows[0];
        Assert.Equal(FontWeights.SemiBold, header.Cells[0].FontWeight);
    });

    [Fact]
    public void Build_GeneraBloquesDeCodigoYCitas() => RunInSta(() =>
    {
        var document = MarkdownFlowDocumentBuilder.Build(
            """
            > Esta es una cita.

            ```csharp
            var x = 1;
            ```
            """);

        Assert.Single(document.Blocks.OfType<Section>());

        var code = document
            .Blocks
            .OfType<Paragraph>()
            .Single(paragraph => paragraph.FontFamily.Source == "Consolas");

        Assert.Contains("var x = 1;", new TextRange(code.ContentStart, code.ContentEnd).Text);
    });

    [Fact]
    public void Build_ConMarkdownVacio_DevuelveUnDocumentoVacio() => RunInSta(() =>
    {
        var document = MarkdownFlowDocumentBuilder.Build(string.Empty);

        Assert.Empty(document.Blocks);
    });

    private static IEnumerable<T> Descendants<T>(InlineCollection inlines)
        where T : Inline
    {
        foreach (var inline in inlines)
        {
            if (inline is T match)
            {
                yield return match;
            }

            if (inline is Span span)
            {
                foreach (var child in Descendants<T>(span.Inlines))
                {
                    yield return child;
                }
            }
        }
    }

    private static void RunInSta(Action action)
    {
        Exception? error = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (error is not null)
        {
            ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
