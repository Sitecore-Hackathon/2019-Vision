
using Sitecore.Shell;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using System;
namespace Hackathon.Feature.DynamicItemsSelection.Commands
{

    [Serializable]
    public class ToggleItemSelector : Command
    {
        public override void Execute(CommandContext context)
        {

        }

        //The following overriden function is called when the "Item Selector" checkbox is checked or unchecked
        public override string GetClick(CommandContext context, string click)
        {
            if (string.IsNullOrEmpty(WebUtil.GetCookieValue("scItemCheckboxState")))
            {
                WebUtil.SetCookieValue("scItemCheckboxState", "0");
            }
            return "javascript:ItemSelector_Click();";
        }

        public override CommandState QueryState(CommandContext context)
        {
            return CheckboxState ? CommandState.Down : CommandState.Enabled;
        }

        //The property gets the cookie value for the "Item Selector" checkbox in the ribbon, and sets the value to 1 if the cookie value is there..otherwise returns 0
        private static bool CheckboxState
        {
            get
            {
                return WebUtil.GetCookieValue("scItemCheckboxState") != "0";
            }
            set
            {
                WebUtil.SetCookieValue("scItemCheckboxState", value ? "1" : "0");
            }
        }
    }
}