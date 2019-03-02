using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Hackathon.Feature.DynamicPublish.Commands
{
    /// <summary>
    /// This class is a command for publish selected items
    /// </summary>
    [Serializable]
    public class PublishSelected : Command
    {
        private Sitecore.Data.Database _master = Sitecore.Configuration.Factory.GetDatabase("master");
        private Sitecore.Data.Database _web = Sitecore.Configuration.Factory.GetDatabase("web");

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            // Get selected items ids for the cookies
            System.Web.HttpContext itemContext = System.Web.HttpContext.Current;         
            string sc_selectedItems = itemContext.Request.Cookies["sc_selectedItems"].Value;

            // Check if there is item selected
            if (string.IsNullOrEmpty(sc_selectedItems))
            {
                // Return alert to inform the user that no items are selected
                SheerResponse.Alert("No items have been selected, please select items using the check box and then click on 'Publish Selected Items'", Array.Empty<string>());
                return;
            }



            List<NameValueCollection> NameValueCollectionList = new List<NameValueCollection>();
            var itemIDs = sc_selectedItems.Split(',');

            //Start collecting the items to be published
            foreach (var itemID in itemIDs)
            {
                Item item = _master.GetItem(ID.Parse(itemID));
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection["id"] = item.ID.ToString();
                nameValueCollection["language"] = item.Language.ToString();
                nameValueCollection["version"] = item.Version.ToString();
                nameValueCollection["workflow"] = "0";
                nameValueCollection["related"] = context.Parameters["related"];
                nameValueCollection["subitems"] = context.Parameters["subitems"];
                nameValueCollection["smart"] = context.Parameters["smart"];
                NameValueCollectionList.Add(nameValueCollection);
            }
            PublishMultiItems(NameValueCollectionList);  
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length != 1 || !Settings.Publishing.Enabled)
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }

        /// <summary>
        /// Publish the multiple items
        /// </summary>
        /// <param name="args">The arguments is a list of items need to be publish</param>
        protected void PublishMultiItems(List<NameValueCollection> args)
        {
            Assert.ArgumentNotNull(args, "args");

            Database[] databases = new Database[1] { _web };
            int counter = 0;
            foreach (var Parameters in args)
            {
                try
                {
                    string itemPath = Parameters["id"];
                    string name = Parameters["language"];
                    string value = Parameters["version"];

                    Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];

                    if (item == null)
                    {
                        continue;
                    }
                    Sitecore.Publishing.PublishManager.PublishItem(item, databases, _web.Languages, true, false);
                    counter++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, this);
                }
            }
            SheerResponse.Alert(string.Format("Publish Completed. Items processed: {0} items", counter), new string[0]);

        }

    }
}
