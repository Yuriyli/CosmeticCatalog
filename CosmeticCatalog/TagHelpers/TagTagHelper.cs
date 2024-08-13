using CosmeticCatalog.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CosmeticCatalog.TagHelpers
{
    public class TagTagHelper : TagHelper
    {

        public Tag TagModel { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "a";
            output.Attributes.Add("class", "tag");
            output.Content.SetContent(TagModel.Name);
        }
    }
}
