using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using System;
namespace Hackathon.Feature.DynamicItemsSelection.Commands
{
    /// <summary>
    /// This class toggle item seclector
    /// </summary>
    [Serializable]
    public class ToggleItemSelector : Command
    {
        public override void Execute(CommandContext context)
        {

        }

        /// <summary>
        /// The following overriden function is called when the "Item Selector" checkbox is checked or unchecked
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="click"> The action</param>
        /// <returns>Return string</returns>
        public override string GetClick(CommandContext context, string click)
        {
            if (string.IsNullOrEmpty(WebUtil.GetCookieValue("scItemCheckboxState")))
            {
                WebUtil.SetCookieValue("scItemCheckboxState", "0");
            }
            return "javascript:ItemSelector_Click();";
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            return CheckboxState ? CommandState.Down : CommandState.Enabled;
        }

        /// <summary>
        /// Gets or sets the CheckboxState cookie
        /// The cookie value for the "Item Selector" checkbox in the ribbon, and sets the value to 1 if the cookie value is there..otherwise returns 0
        /// </summary>
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