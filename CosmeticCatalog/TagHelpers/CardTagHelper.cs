using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CosmeticCatalog.TagHelpers
{
    public class CardTagHelper : TagHelper
    {
        public string? Title { get; set; }
        public int MaxWidth { get; set; }
        public int MinWidth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = output.GetChildContentAsync().Result.GetContent(); ;
            output.TagName = "div";
            output.Attributes.Add("class", "card card-m-auto");

            var titleContent = String.Empty;
            if (Title != null)
            {
                titleContent = $"<h5 class=\"card-title\">{Title}</h5><br />";
            }

            if (MaxWidth > 0)
            {
                output.Attributes.Add("style", $"max-width:{MaxWidth}px");
            }
            if (MinWidth > 0)
            {
                output.Attributes.Add("style", $"min-width:{MinWidth}px");
            }
            var content = $"<div class=\"card-body\">{titleContent}{childContent}</div>";
            output.Content.SetHtmlContent(content);
        }
    }
}