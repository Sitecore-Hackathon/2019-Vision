
using Sitecore.Shell;
using Sitecore.Shell.Framework.Commands;
using System;

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
            return "ItemSelector_Click";
        }

        public override CommandState QueryState(CommandContext context)
        {
            //if (!UserOptions.ContentEditor.ShowRawValues)
           // {
                return CommandState.Enabled;
           // }
           // return CommandState.Down;
        }
    }
}