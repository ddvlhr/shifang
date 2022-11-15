using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Helper;

public class MarkdownHelper
{
    public StringBuilder MarkdownBuilder;

    public MarkdownHelper()
    {
        MarkdownBuilder = new StringBuilder();
    }

    /// <summary>
    ///     添加 Markdown 标题
    /// </summary>
    /// <param name="index">标题级数 1 - 6</param>
    /// <param name="title">标题内容</param>
    /// <param name="quote">是否添加引用标签</param>
    public void AddTitle(int index, string title, bool quote = false)
    {
        var titleType = "";
        for (var i = 0; i < index; i++) titleType += "#";

        var text = "";
        if (quote) text += "> ";

        text += $"{titleType} {title}";
        MarkdownBuilder.Append(text);
    }

    public void AddQuote(string text)
    {
        MarkdownBuilder.Append($"> {text}");
    }

    public void AddLink(string text)
    {
        MarkdownBuilder.Append($"[this is a link]({text})");
    }

    public void AddImage(string imageUrl)
    {
        MarkdownBuilder.Append($"![]({imageUrl})");
    }

    public void AddUnorderedList(List<string> list)
    {
        var text = "";
        foreach (var item in list) text += $"- {item}";

        MarkdownBuilder.Append(text);
    }

    public void AddOrderedList(List<string> list)
    {
        var text = "";
        var index = 1;
        foreach (var item in list)
        {
            text += $"{index}. {item}";
            index++;
        }

        MarkdownBuilder.Append(text);
    }
}