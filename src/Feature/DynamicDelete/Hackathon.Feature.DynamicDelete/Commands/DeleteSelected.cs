using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;

namespace Hackathon.Feature.DynamicDelete.Commands
{
    /// <summary>
    /// This class is a command for delete selected items
    /// </summary>
    [Serializable]
    public class DeleteSelected : Command
    {
        private Database _master = Sitecore.Configuration.Factory.GetDatabase("master");
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            // Get selected items ids for the cookies
            string sc_selectedItems = System.Web.HttpContext.Current.Request.Cookies["sc_selectedItems"].Value;

            // Check if there is no item selected
            if (string.IsNullOrEmpty(sc_selectedItems))
            {
                // Return alert to inform the user that no items are selected
                SheerResponse.Alert("No items have been selected, select items using the check box then retry Delete selected items", Array.Empty<string>());
                return;
            }

            List<Item> itemsList = new List<Item>();
            var itemIDs = sc_selectedItems.Split(',');
            foreach (var itemID in itemIDs)
            {
                // Get selected item by id 
                Item contextItem = _master.GetItem(ID.Parse(itemID));
                itemsList.Add(contextItem);
            }

            Items.Delete(itemsList.ToArray());
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
