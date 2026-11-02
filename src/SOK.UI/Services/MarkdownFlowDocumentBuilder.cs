using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MdBlock = Markdig.Syntax.Block;
using MdInline = Markdig.Syntax.Inlines.Inline;
using MdTable = Markdig.Extensions.Tables.Table;
using MdTableCell = Markdig.Extensions.Tables.TableCell;
using MdTableRow = Markdig.Extensions.Tables.TableRow;
using WpfTable = System.Windows.Documents.Table;
using WpfTableCell = System.Windows.Documents.TableCell;
using WpfTableRow = System.Windows.Documents.TableRow;

namespace SOK.UI.Services;

/// <summary>
/// Convierte el Markdown de los pasos del tutorial en un <see cref="FlowDocument"/>
/// apto para <c>FlowDocumentScrollViewer</c>. Se admiten encabezados,
/// párrafos, listas, tablas, citas, código, énfasis y enlaces.
/// </summary>
public static class MarkdownFlowDocumentBuilder
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private static readonly FontFamily BodyFont = new("Segoe UI");
    private static readonly FontFamily CodeFont = new("Consolas");
    private static readonly Brush TextBrush = CreateBrush("#0F172A");
    private static readonly Brush SecondaryTextBrush = CreateBrush("#475569");
    private static readonly Brush AccentBrush = CreateBrush("#2563EB");
    private static readonly Brush BorderBrush = CreateBrush("#E2E8F0");
    private static readonly Brush CodeBackground = CreateBrush("#F1F5F9");
    private static readonly Brush QuoteBackground = CreateBrush("#F8FAFC");
    private static readonly Brush TableHeaderBackground = CreateBrush("#EFF6FF");

    /// <summary>Construye el documento WPF a partir del Markdown indicado.</summary>
    public static FlowDocument Build(string markdown)
    {
        var document = new FlowDocument
        {
            FontFamily = BodyFont,
            FontSize = 13,
            Foreground = TextBrush,
            PagePadding = new Thickness(0),
            ColumnWidth = double.NaN,
            LineHeight = 20
        };

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return document;
        }

        var parsed = Markdig.Markdown.Parse(markdown, Pipeline);
        AppendBlocks(parsed, document.Blocks);
        return document;
    }

    private static void AppendBlocks(ContainerBlock container, BlockCollection target)
    {
        foreach (var block in container)
        {
            AppendBlock(block, target);
        }
    }

    private static void AppendBlock(MdBlock block, BlockCollection target)
    {
        switch (block)
        {
            case HeadingBlock heading:
                target.Add(BuildHeading(heading));
                break;

            case ParagraphBlock paragraph:
                target.Add(BuildParagraph(paragraph));
                break;

            case ListBlock list:
                target.Add(BuildList(list));
                break;

            case QuoteBlock quote:
                target.Add(BuildQuote(quote));
                break;

            case FencedCodeBlock fenced:
                target.Add(BuildCode(GetCode(fenced)));
                break;

            case CodeBlock code:
                target.Add(BuildCode(GetCode(code)));
                break;

            case MdTable table:
                target.Add(BuildTable(table));
                break;

            case ThematicBreakBlock:
                target.Add(new Paragraph
                {
                    BorderBrush = BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Margin = new Thickness(0, 8, 0, 14)
                });
                break;

            case HtmlBlock html:
                target.Add(new Paragraph(new Run(html.Lines.ToString()))
                {
                    FontFamily = CodeFont,
                    FontSize = 11,
                    Foreground = SecondaryTextBrush,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                break;

            case ContainerBlock container:
                AppendBlocks(container, target);
                break;
        }
    }

    private static Paragraph BuildHeading(HeadingBlock heading)
    {
        var paragraph = new Paragraph
        {
            FontWeight = FontWeights.SemiBold,
            Foreground = TextBrush,
            Margin = new Thickness(0, heading.Level == 1 ? 0 : 16, 0, 8)
        };

        paragraph.FontSize = heading.Level switch
        {
            1 => 24,
            2 => 18,
            3 => 15,
            _ => 13
        };

        if (heading.Level == 1)
        {
            paragraph.Margin = new Thickness(0, 0, 0, 14);
        }

        paragraph.Inlines.Add(BuildSpan(heading.Inline));
        return paragraph;
    }

    private static Paragraph BuildParagraph(ParagraphBlock block)
    {
        var paragraph = new Paragraph
        {
            Margin = new Thickness(0, 0, 0, 10),
            LineHeight = 20
        };

        paragraph.Inlines.Add(BuildSpan(block.Inline));
        return paragraph;
    }

    private static List BuildList(ListBlock listBlock)
    {
        var list = new List
        {
            MarkerStyle = listBlock.IsOrdered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc,
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(22, 0, 0, 0)
        };

        if (listBlock.IsOrdered &&
            int.TryParse(listBlock.OrderedStart, out var start) &&
            start > 0)
        {
            list.StartIndex = start;
        }

        foreach (var item in listBlock.OfType<ListItemBlock>())
        {
            var listItem = new ListItem();
            foreach (var child in item)
            {
                AppendBlock(child, listItem.Blocks);
            }

            list.ListItems.Add(listItem);
        }

        return list;
    }

    private static Section BuildQuote(QuoteBlock quote)
    {
        var section = new Section
        {
            Background = QuoteBackground,
            BorderBrush = AccentBrush,
            BorderThickness = new Thickness(3, 0, 0, 0),
            Padding = new Thickness(12, 6, 12, 2),
            Margin = new Thickness(0, 2, 0, 12)
        };

        AppendBlocks(quote, section.Blocks);
        return section;
    }

    private static Paragraph BuildCode(string code)
    {
        return new Paragraph(new Run(code))
        {
            FontFamily = CodeFont,
            FontSize = 12,
            Background = CodeBackground,
            BorderBrush = BorderBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Margin = new Thickness(0, 2, 0, 12)
        };
    }

    private static WpfTable BuildTable(MdTable table)
    {
        var flowTable = new WpfTable
        {
            CellSpacing = 0,
            Margin = new Thickness(0, 4, 0, 14)
        };

        var columnCount = table
            .OfType<MdTableRow>()
            .Select(row => row.Count())
            .DefaultIfEmpty(0)
            .Max();

        if (columnCount <= 0)
        {
            columnCount = table.ColumnDefinitions.Count;
        }

        columnCount = Math.Max(1, columnCount);

        for (var index = 0; index < columnCount; index++)
        {
            flowTable.Columns.Add(new TableColumn());
        }

        var group = new TableRowGroup();

        foreach (var row in table.OfType<MdTableRow>())
        {
            var flowRow = new WpfTableRow();
            if (row.IsHeader)
            {
                flowRow.Background = TableHeaderBackground;
            }

            foreach (var cell in row.OfType<MdTableCell>())
            {
                var flowCell = new WpfTableCell
                {
                    BorderBrush = BorderBrush,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(8, 5, 8, 5)
                };

                if (row.IsHeader)
                {
                    flowCell.FontWeight = FontWeights.SemiBold;
                }

                foreach (var child in cell)
                {
                    AppendBlock(child, flowCell.Blocks);
                }

                if (flowCell.Blocks.Count == 0)
                {
                    flowCell.Blocks.Add(new Paragraph());
                }

                flowRow.Cells.Add(flowCell);
            }

            group.Rows.Add(flowRow);
        }

        flowTable.RowGroups.Add(group);
        return flowTable;
    }

    private static Span BuildSpan(ContainerInline? inline)
    {
        var span = new Span();
        AppendInlines(inline?.FirstChild, span);
        return span;
    }

    private static void AppendInlines(
        MdInline? inline,
        Span target,
        bool bold = false,
        bool italic = false)
    {
        while (inline is not null)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    target.Inlines.Add(new Run(literal.Content.ToString())
                    {
                        FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                        FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
                    });
                    break;

                case EmphasisInline emphasis:
                    var isBold = emphasis.DelimiterCount >= 2;
                    var isItalic = emphasis.DelimiterCount is 1 or 3;
                    AppendInlines(
                        emphasis.FirstChild,
                        target,
                        bold || isBold,
                        italic || isItalic);
                    break;

                case CodeInline code:
                    target.Inlines.Add(new Run(code.Content)
                    {
                        FontFamily = CodeFont,
                        FontSize = 12,
                        Background = CodeBackground,
                        Foreground = AccentBrush
                    });
                    break;

                case LinkInline link:
                    target.Inlines.Add(BuildHyperlink(link, bold, italic));
                    break;

                case AutolinkInline autolink:
                    target.Inlines.Add(BuildHyperlink(autolink.Url, autolink.Url, bold, italic));
                    break;

                case LineBreakInline lineBreak:
                    target.Inlines.Add(lineBreak.IsHard ? new LineBreak() : new Run(" "));
                    break;

                case HtmlEntityInline entity:
                    target.Inlines.Add(new Run(entity.Transcoded.ToString()));
                    break;

                case HtmlInline html:
                    target.Inlines.Add(new Run(html.Tag)
                    {
                        FontFamily = CodeFont,
                        FontSize = 11,
                        Foreground = SecondaryTextBrush
                    });
                    break;

                case ContainerInline container:
                    AppendInlines(container.FirstChild, target, bold, italic);
                    break;
            }

            inline = inline.NextSibling;
        }
    }

    private static Hyperlink BuildHyperlink(LinkInline link, bool bold, bool italic)
    {
        var hyperlink = new Hyperlink
        {
            Foreground = AccentBrush,
            TextDecorations = TextDecorations.Underline,
            FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
            FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
        };

        if (!link.IsImage && Uri.TryCreate(link.Url, UriKind.Absolute, out var uri))
        {
            hyperlink.NavigateUri = uri;
            hyperlink.RequestNavigate += (_, args) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri)
                    {
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Un enlace inválido no debe interrumpir la lectura del paso.
                }

                args.Handled = true;
            };
        }

        AppendInlines(link.FirstChild, hyperlink, bold, italic);

        if (hyperlink.Inlines.Count == 0)
        {
            hyperlink.Inlines.Add(new Run(link.Url ?? string.Empty));
        }

        return hyperlink;
    }

    private static Hyperlink BuildHyperlink(string url, string text, bool bold, bool italic)
    {
        var hyperlink = new Hyperlink(new Run(text))
        {
            Foreground = AccentBrush,
            TextDecorations = TextDecorations.Underline,
            FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
            FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
        };

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            hyperlink.NavigateUri = uri;
            hyperlink.RequestNavigate += (_, args) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri)
                    {
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Un enlace inválido no debe interrumpir la lectura del paso.
                }

                args.Handled = true;
            };
        }

        return hyperlink;
    }

    private static string GetCode(LeafBlock block)
    {
        var lines = block.Lines.Lines;
        if (lines is null || block.Lines.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var index = 0; index < block.Lines.Count; index++)
        {
            if (index > 0)
            {
                builder.AppendLine();
            }

            builder.Append(lines[index].Slice.ToString());
        }

        return builder.ToString();
    }

    private static Brush CreateBrush(string hex)
    {
        var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
        brush.Freeze();
        return brush;
    }
}
