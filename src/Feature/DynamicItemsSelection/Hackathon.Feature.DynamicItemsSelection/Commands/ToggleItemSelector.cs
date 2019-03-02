
using Sitecore.Shell;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using System;
using System.Web;
using System.Web.UI;

namespace Hackathon.Feature.DynamicItemsSelection.Commands
{
  
    [Serializable]
    public class ToggleItemSelector : Command
    {
        public override void Execute(CommandContext context)
        {
           
        }

        public override string GetClick(CommandContext context, string click)
        {
          
            var script = "javascript:ItemSelector_Click();";                                                        

            return script;
        }

        

        public override CommandState QueryState(CommandContext context)
        {
            return CheckboxState ? CommandState.Down : CommandState.Enabled;
        }

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