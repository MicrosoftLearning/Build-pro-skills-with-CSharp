using Microsoft.AspNetCore.Razor.TagHelpers;
using Library.Web;

namespace Library.Web.TagHelpers;
[HtmlTargetElement("ui-operation-status")]
public class UiOperationStatusTagHelper : TagHelper
{
    public UiOperationStatus? Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Status == null)
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = "div";
        string classString = Status.Type switch
        {
            UiResultType.Success => "alert-success",
            UiResultType.Failure => "alert-warning",
            UiResultType.Error => "alert-danger",
            _ => "alert-info",
        };
        output.Attributes.SetAttribute("class", $"alert {classString}");
        output.Content.SetContent(Status.Message);
    }
}