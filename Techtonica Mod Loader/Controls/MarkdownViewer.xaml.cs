using Microsoft.Win32.SafeHandles;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Techtonica_Mod_Loader.Controls
{
    /// <summary>
    /// Interaction logic for MarkdownViewer.xaml
    /// </summary>
    public partial class MarkdownViewer : UserControl
    {
        public MarkdownViewer()
        {
            InitializeComponent();
        }

        // Objects & Variables
        private int lastIndentCount = 0;
        private bool isNumberedListOngoing = false;
        private List<int> numberedListsProgressesForIndentCount = new List<int>() { 0 };
        private Dictionary<string, TextBlock> headerTextBlocks = new Dictionary<string, TextBlock>();

        // Public Functions

        public async void ViewMarkdown(string markdown) {
            markdown = markdown.Replace("&nbsp", " ");

            mainPanel.Children.Clear();
            string[] lines = markdown.Replace("\r", "").Split('\n');
            
            for(int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                switch (GetEntryType(line)) {
                    case MarkdownEntryType.Paragraph: AddParagraph(line); break;
                    case MarkdownEntryType.Header1: AddHeader(line, 1); break;
                    case MarkdownEntryType.Header2: AddHeader(line, 2); break;
                    case MarkdownEntryType.Header3: AddHeader(line, 3); break;
                    case MarkdownEntryType.Header4: AddHeader(line, 4); break;
                    case MarkdownEntryType.Header5: AddHeader(line, 5); break;
                    case MarkdownEntryType.Header6: AddHeader(line, 6); break;
                    case MarkdownEntryType.NumberedList: AddNumberedListItem(line); break;
                    case MarkdownEntryType.BulletList: AddBulletListItem(line); break;
                    case MarkdownEntryType.BlockQuote: AddBlockQuote(line); break;
                    case MarkdownEntryType.Image: await AddImage(line); break;
                    case MarkdownEntryType.LinkedImaged: await AddLinkedImage(line); break;
                    case MarkdownEntryType.HorizontalRule: AddHorizontalRule(); break;
                    
                    case MarkdownEntryType.CodeBlock:
                        int endIndex = Array.IndexOf(lines, "```", i + 1);
                        List<string> codeLines = new List<string>();
                        for(int j = i; j < endIndex; j++) {
                            codeLines.Add(lines[j]);
                        }
                        AddCodeBlock(string.Join("\n", codeLines));
                        i = endIndex;
                        break;

                    case MarkdownEntryType.Table:
                        List<string> tableLines = new List<string>() { line };
                        int k = i + 1;
                        while (lines[k].StartsWith("|")) {
                            tableLines.Add(lines[k]);
                            ++k;
                        }

                        i = k;
                        AddTable(string.Join("\n", tableLines));
                        break;
                }

                if(isNumberedListOngoing && !IsLinePartOfNumberedList(line)) {
                    isNumberedListOngoing = false;
                    for(int j = 0; j < numberedListsProgressesForIndentCount.Count; j++) {
                        numberedListsProgressesForIndentCount[j] = 0;
                    }
                }
            }
        }

        public void ViewMarkdownFromFile(string filePath) {
            string markdown = File.ReadAllText(filePath);
            ViewMarkdown(markdown);
        }

        // Private Functions

        private MarkdownEntryType GetEntryType(string line) {
            if (line.StartsWith("# ")) return MarkdownEntryType.Header1;
            else if (line.StartsWith("## ")) return MarkdownEntryType.Header2;
            else if (line.StartsWith("### ")) return MarkdownEntryType.Header3;
            else if (line.StartsWith("#### ")) return MarkdownEntryType.Header4;
            else if (line.StartsWith("##### ")) return MarkdownEntryType.Header5;
            else if (line.StartsWith("###### ")) return MarkdownEntryType.Header6;
            else if (IsLinePartOfBulletList(line)) return MarkdownEntryType.BulletList;
            else if (IsLinePartOfNumberedList(line)) return MarkdownEntryType.NumberedList;
            else if (line.StartsWith("> ")) return MarkdownEntryType.BlockQuote;
            else if (line == "```") return MarkdownEntryType.CodeBlock;
            else if (line.StartsWith("![")) return MarkdownEntryType.Image;
            else if (line.StartsWith("[!")) return MarkdownEntryType.LinkedImaged;
            else if (IsLinePartOfTable(line)) return MarkdownEntryType.Table;
            else if (IsLineHorizontalRule(line)) return MarkdownEntryType.HorizontalRule;
            // ToDo: More Types, Nested Lists, Nested Quotes
            else return MarkdownEntryType.Paragraph;
        }

        private bool IsLinePartOfNumberedList(string line) {
            if (!line.Contains(".")) return false;

            string[] parts = line.Split('.');
            bool isPartOfNumberedList = int.TryParse(parts[0], out int num);
            isNumberedListOngoing = isPartOfNumberedList;
            return isPartOfNumberedList;
        }

        private bool IsLinePartOfBulletList(string line) {
            if (string.IsNullOrEmpty(line)) return false;

            int starCount = line.Length - line.Replace("*", "").Length;
            if (line.StartsWith("*") && starCount % 2 == 0) return false;

            line = line.Trim();
            if (string.IsNullOrEmpty(line)) return false;
            List<char> bulletCharacters = new List<char>() {
                '-',
                '*',
                '+'
            };
            return bulletCharacters.Contains(line[0]);
        }

        private int GetIndentCount(string line) {
            if (!line.StartsWith("\t") && !line.StartsWith("    ")) return 0;

            line = line.Replace("\t", "    ");
            return (line.Length - line.Replace("    ", "").Length) / 4;
        }

        private bool IsLinePartOfTable(string line) {
            return line.StartsWith("|") && line.EndsWith("|");
        }

        private bool IsLineHorizontalRule(string line) {
            List<char> ruleCharacters = new List<char>() { '*', '-', '_' };
            foreach(char character in line) {
                if (!ruleCharacters.Contains(character)) {
                    return false;
                }
            }

            return line.StartsWith("***") || line.StartsWith("---") || line.StartsWith("___");
        }

        // Add Element Functions

        private void AddEmptyLine() {
            mainPanel.Children.Add(new TextBlock() {
                Text = " ",
                FontSize = 14
            });
        }

        private void AddParagraph(string markdownText) {
            mainPanel.Children.Add(FormatMarkdownToTextBlock(markdownText));
        }

        private TextBlock FormatMarkdownToTextBlock(string markdownText) {
            TextBlock textBlock = new TextBlock {
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            string boldPattern = @"\*\*(.*?)\*\*";
            string altBoldPattern = @"__(.*?)__";
            string italicPattern = @"\*(.*?)\*";
            string altItalicPattern = @"_(.*?)_";
            string hyperlinkPattern = @"\[(.*?)\]\((.*?)\)";
            string linkPattern = @"\<(.*?)\>";
            string codePattern = "```(.*?)```";
            string altCodePattern = "`(.*?)`";
            List<string> patterns = new List<string>() {
                boldPattern,
                altBoldPattern,
                italicPattern,
                altItalicPattern,
                hyperlinkPattern,
                linkPattern,
                codePattern,
                altCodePattern
            };
            string allPatterns = string.Join("|", patterns);

            int currentIndex = 0;

            foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(markdownText, allPatterns, System.Text.RegularExpressions.RegexOptions.Singleline)) {
                int matchIndex = match.Index;
                int matchLength = match.Length;

                // Add text before the current match
                textBlock.Inlines.Add(new Run(markdownText.Substring(currentIndex, matchIndex - currentIndex)));

                // Handle bold formatting
                if (match.ToString().StartsWith("**")) {
                    Run boldRun = new Run(match.Groups[1].Value) {
                        FontWeight = FontWeights.Bold
                    };
                    textBlock.Inlines.Add(boldRun);
                }
                // Handle alt bold formatting
                else if (match.ToString().StartsWith("__")) {
                    Run boldRun = new Run(match.Groups[2].Value) {
                        FontWeight = FontWeights.Bold
                    };
                    textBlock.Inlines.Add(boldRun);
                }
                // Handle italic formatting
                else if (match.ToString().StartsWith("*")) {
                    Run italicRun = new Run(match.Groups[3].Value) {
                        FontStyle = FontStyles.Italic
                    };
                    textBlock.Inlines.Add(italicRun);
                }
                // Handle alt italic formatting
                else if (match.ToString().StartsWith("_")) {
                    Run italicRun = new Run(match.Groups[4].Value) {
                        FontStyle = FontStyles.Italic
                    };
                    textBlock.Inlines.Add(italicRun);
                }
                // Handle hyperlink formatting
                else if (match.ToString().StartsWith("[")) {
                    string linkText = match.Groups[5].Value;
                    string url = match.Groups[6].Value;

                    if (!url.StartsWith("#")) {
                        Hyperlink hyperlinkRun = new Hyperlink(new Run(linkText)) {
                            NavigateUri = new Uri(url),
                            TextDecorations = TextDecorations.Underline,
                            Foreground = Brushes.DeepSkyBlue,
                            FontSize = 14
                        };
                        hyperlinkRun.Click += (sender, e) => { GuiUtils.OpenURL(url); };
                        textBlock.Inlines.Add(hyperlinkRun);
                    }
                    else {
                        // textBlock.Inlines.Add(new Run(linkText));
                        Hyperlink hyperlinkRun = new Hyperlink(new Run(linkText)) {
                            TextDecorations = TextDecorations.Underline,
                            Foreground = Brushes.DeepSkyBlue,
                            FontSize = 14
                        };
                        hyperlinkRun.Click += (sender, e) => {
                            string key = linkText.Replace("#", "");
                            if (headerTextBlocks.ContainsKey(key)) {
                                scroller.ScrollToBottom();
                                headerTextBlocks[key].BringIntoView();
                            }
                        };
                        textBlock.Inlines.Add(hyperlinkRun);
                    }

                    
                }
                // Handle link formatting
                else if (match.ToString().StartsWith("<")) {
                    string url = match.Groups[7].Value;
                    Hyperlink hyperlinkRun = new Hyperlink(new Run(url)) {
                        NavigateUri = new Uri(url),
                        TextDecorations = TextDecorations.Underline,
                        Foreground = Brushes.DeepSkyBlue,
                        FontSize = 14
                    };
                    hyperlinkRun.Click += (sender, e) => { GuiUtils.OpenURL(url); };
                    textBlock.Inlines.Add(hyperlinkRun);
                }
                // Handle code block
                else if (match.ToString().StartsWith("```")) {
                    Run codeBlockRun = new Run($" {match.Groups[8].Value} ") {
                        Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                        Foreground = Brushes.White
                    };
                    textBlock.Inlines.Add(codeBlockRun);
                }
                // Hand alt code block
                else if (match.ToString().StartsWith("`")) {
                    Run codeBlockRun = new Run($" {match.Groups[9].Value} ") {
                        Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                        Foreground = Brushes.White
                    };
                    textBlock.Inlines.Add(codeBlockRun);
                }

                currentIndex = matchIndex + matchLength;
            }

            // Add the remaining text
            textBlock.Inlines.Add(new Run(markdownText.Substring(currentIndex)));

            return textBlock;
        }

        private void AddHeader(string markdownText, int size) {
            int fontsize = 12;
            switch (size) {
                case 1: fontsize = 20; break;
                case 2: fontsize = 19; break;
                case 3: fontsize = 18; break;
                case 4: fontsize = 17; break;
                case 5: fontsize = 16; break;
                case 6: fontsize = 15; break;
            }

            string text = markdownText.Trim('#').Trim();
            TextBlock textBlock = new TextBlock() {
                Text = text,
                Foreground = Brushes.White,
                FontSize = fontsize,
                FontWeight = FontWeights.Bold
            };
            mainPanel.Children.Add(textBlock);
            headerTextBlocks.Add(text, textBlock);
            AddHorizontalRule();
        }

        public void AddNumberedListItem(string line) {
            int indentCount = GetIndentCount(line);
            if (numberedListsProgressesForIndentCount.Count <= indentCount) {
                numberedListsProgressesForIndentCount.Add(0);
            }
            
            ++numberedListsProgressesForIndentCount[indentCount];

            if (indentCount < lastIndentCount) {
                for (int i = lastIndentCount; i < numberedListsProgressesForIndentCount.Count; i++) {
                    numberedListsProgressesForIndentCount[i] = 0;
                }
            }

            isNumberedListOngoing = true;
            int start = line.IndexOf(". ") + 2;
            string text = line.Substring(start);

            List<string> numberParts = new List<string>();
            for (int i = 0; i <= indentCount; i++) {
                numberParts.Add(numberedListsProgressesForIndentCount[i].ToString());
            }

            string paragraph = $"{string.Join(".", numberParts)}. {text}";
            for(int i = 0; i < indentCount; i++) {
                paragraph = "    " + paragraph;
            }
            
            AddParagraph(paragraph);
            lastIndentCount = indentCount;
        }

        public void AddBulletListItem(string line) {
            char bulletChar = line.Trim()[0];
            int start = line.IndexOf(bulletChar + " ") + 2;
            string text = line.Substring(start);
            string paragraph = $"• {text}";
            
            int indentCount = GetIndentCount(line);
            for(int i = 0; i < indentCount; i++) {
                paragraph = "    " + paragraph;
            }
            
            AddParagraph(paragraph);
        }

        public void AddBlockQuote(string line) {
            int start = line.IndexOf("> ") + 2;
            string text = line.Substring(start);
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
            panel.Children.Add(new Rectangle() { Fill = Brushes.Gray, Width = 5, Margin = new Thickness(0, 0, 5, 0) });
            panel.Children.Add(FormatMarkdownToTextBlock(text));
            mainPanel.Children.Add(panel);
        }

        private void AddCodeBlock(string code) {
            Border border = new Border() {
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            StackPanel linesPanel = new StackPanel() { Margin = new Thickness(5) };
            string[] lines = code.Replace("\r", "").Split('\n');
            foreach(string line in lines) {
                if (line == "```") continue;
                linesPanel.Children.Add(new TextBlock() {
                    Text = line,
                    Foreground = Brushes.White,
                    FontSize = 14
                });
            }

            border.Child = linesPanel;
            mainPanel.Children.Add(border);
        }

        private async Task<string> AddImage(string line) {
            int start = line.IndexOf("(") + 1;
            int end = line.IndexOf(")");
            string url = line.Substring(start, end - start);

            if (!Settings.userSettings.renderImages) {
                mainPanel.Children.Add(FormatMarkdownToTextBlock($"[Image Link]({url})"));
                return "";
            }

            Image image = await GuiUtils.GetImageFromURL(url);
            if(image != null) {
                image.MaxWidth = mainPanel.ActualWidth;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(0, 5, 0, 5);
                mainPanel.Children.Add(image);
            }
            else {
                mainPanel.Children.Add(FormatMarkdownToTextBlock($"[Image Link]({url})"));
            }

            return "";
        }

        private async Task<string> AddLinkedImage(string line) {
            Regex textRegex = new Regex(@"\!\[(.*?)\]");
            Regex linkRegex = new Regex(@"\((.*?)\)");

            string imageAltText = "";
            string imageLink = "";
            string hyperlinkUrl = "";

            Match textMatch = textRegex.Match(line);
            if (textMatch.Success) {
                imageAltText = textMatch.Groups[1].Value;
            }

            Match linkMatch = linkRegex.Match(line);
            if(linkMatch.Success) {
                imageLink = linkMatch.Groups[1].Value;
            }

            // Extract the hyperlink
            int startIndex = line.IndexOf(")](");
            if (startIndex != -1) {
                hyperlinkUrl = line.Substring(startIndex + 2).Replace("(", "").Replace(")", "");
            }

            if (!Settings.userSettings.renderImages) {
                mainPanel.Children.Add(FormatMarkdownToTextBlock($"[Image Link]({imageLink})"));
                mainPanel.Children.Add(FormatMarkdownToTextBlock($"[Hyperlink]({hyperlinkUrl})"));
                return "";
            }

            HyperlinkImage image = new HyperlinkImage(hyperlinkUrl);
            await image.ShowImage(imageLink);
            mainPanel.Children.Add(image);
            return "";
        }

        private void AddTable(string markdown) {
            // Table Settings
            SolidColorBrush lineColour = Brushes.Gray;
            int lineThickness = 2;
            Thickness cellMargin = new Thickness(6, 4, 6, 4);

            // Process Alignments
            List<string> lines = markdown.Replace("\r", "").Split('\n').ToList();
            string[] alignmentStrings = lines[1].Split("|", StringSplitOptions.RemoveEmptyEntries);
            List<HorizontalAlignment> alignments = new List<HorizontalAlignment>();
            foreach(string alignmentString in alignmentStrings) {
                if(alignmentString.StartsWith(":") && alignmentString.EndsWith(":")) {
                    alignments.Add(HorizontalAlignment.Center);
                }
                else if (alignmentString.EndsWith(":")) {
                    alignments.Add(HorizontalAlignment.Right);
                }
                else {
                    alignments.Add(HorizontalAlignment.Left);
                }
            }

            // Get Table Data
            lines.RemoveAt(1);
            string[] headers = lines[0].Split("|", StringSplitOptions.RemoveEmptyEntries);
            int numColumns = headers.Count();
            int numRows = lines.Count;

            // Create Table
            Border border = new Border() { 
                BorderBrush = lineColour, 
                BorderThickness = new Thickness(lineThickness),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid table = new Grid();
            for (int i = 0; i < numColumns; i++) {
                table.ColumnDefinitions.Add(new ColumnDefinition() { 
                    Width = new GridLength(1, GridUnitType.Auto) 
                });
            }
            for (int i = 0; i < numRows; i++) {
                table.RowDefinitions.Add(new RowDefinition());
            }

            // Add Table Lines
            for (int i = 0; i < numColumns - 1; i++) {
                Rectangle rectangle = new Rectangle() {
                    Width = lineThickness,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Fill = lineColour,
                    Margin = new Thickness(0, 0, -lineThickness / 2.0, 0)
                };
                rectangle.SetValue(Grid.ColumnProperty, i);
                rectangle.SetValue(Grid.RowSpanProperty, numRows);
                table.Children.Add(rectangle);
            }
            for(int i = 0; i < numRows - 1; i++) {
                Rectangle rectangle = new Rectangle() {
                    Height = 1,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Fill = lineColour,
                    Margin = new Thickness(0, 0, -lineThickness / 2.0, 0)
                };
                rectangle.SetValue(Grid.ColumnSpanProperty, numColumns);
                rectangle.SetValue(Grid.RowProperty, i);
                table.Children.Add(rectangle);
            }

            // Add Column Header Labels
            for(int i = 0; i < headers.Count(); i++) {
                TextBlock header = FormatMarkdownToTextBlock(headers[i].Trim());
                header.FontSize = 16;
                header.FontWeight = FontWeights.Bold;
                header.Margin = cellMargin;
                header.HorizontalAlignment = alignments[i];
                header.VerticalAlignment = VerticalAlignment.Center;
                header.SetValue(Grid.ColumnProperty, i);
                table.Children.Add(header);
            }

            // Add Data Labels
            for(int rowNum = 1; rowNum < numRows; rowNum++) {
                string[] cells = lines[rowNum].Split("|", StringSplitOptions.RemoveEmptyEntries);
                for(int columnNum = 0; columnNum < numColumns; columnNum++) {
                    TextBlock cell = FormatMarkdownToTextBlock(cells[columnNum].Trim());
                    cell.FontSize = 14;
                    cell.Margin = cellMargin;
                    cell.HorizontalAlignment = alignments[columnNum];
                    cell.VerticalAlignment = VerticalAlignment.Center;
                    cell.SetValue(Grid.RowProperty, rowNum);
                    cell.SetValue(Grid.ColumnProperty, columnNum);
                    table.Children.Add(cell);
                }
            }

            border.Child = table;
            mainPanel.Children.Add(border);
        }

        private void AddHorizontalRule() {
            mainPanel.Children.Add(new Rectangle() {
                Fill = Brushes.Gray,
                Height = 1,
                Margin = new Thickness(0, 2, 0, 5)
            });
        }
    }

    enum MarkdownEntryType{
        Paragraph,
        Header1,
        Header2,
        Header3,
        Header4,
        Header5,
        Header6,
        BlockQuote,
        NumberedList,
        BulletList,
        CodeBlock,
        Image,
        LinkedImaged,
        HorizontalRule,
        Table,

        // Embedded Types:
        // Bold **Bold** or __Bold__
        // Italic *Italic* or _Italic_
        // Code `Code`
        // Link [Title](Link)
        // NoTitleLink <Link>
    }
}
