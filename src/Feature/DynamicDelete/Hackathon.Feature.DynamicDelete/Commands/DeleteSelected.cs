using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
namespace Hackathon.Feature.DynamicDelete.Commands
{
    [Serializable]
    public class DeleteSelected : Command
    {
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 0)
            {
                SheerResponse.Alert("The selected item could not be found.\n\nIt may have been deleted by another user.\n\nSelect another item.", Array.Empty<string>());
                return;
            }
            if (context.Items.Length == 1)
            {
                SheerResponse.Eval("if(this.Content && this.Content.loadNextSearchedItem){{this.Content.loadNextSearchedItem('{0}');}}", new object[]
                {
                    context.Items[0].ID
                });
            }
            Items.Delete(context.Items);
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length == 0)
            {
                return CommandState.Disabled;
            }
            Item[] items = context.Items;
            for (int i = 0; i < items.Length; i++)
            {
                Item item = items[i];
                if (!item.Access.CanDelete())
                {
                    return CommandState.Disabled;
                }
                if (item.Appearance.ReadOnly)
                {
                    return CommandState.Disabled;
                }
                if (Command.IsLockedByOther(item))
                {
                    return CommandState.Disabled;
                }
            }
            return base.QueryState(context);
        }
    }
}
