using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Hackathon.Feature.DynamicPublish.Commands
{

    [Serializable]
    public class PublishSelected : Command
    {
        private Sitecore.Data.Database _master =
                    Sitecore.Configuration.Factory.GetDatabase("master");
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {

            //Retun selected items from cookies
            System.Web.HttpContext itemContext = System.Web.HttpContext.Current;         
            string sc_selectedItems = itemContext.Request.Cookies["sc_selectedItems"].Value;

            //Validate existing items
            if (string.IsNullOrEmpty(sc_selectedItems))
            {
                SheerResponse.Alert("No items have been selected, select items using the check box then retry publish selected items", Array.Empty<string>());
                return;
            }
            var itemIDs = sc_selectedItems.Split(',');
            List<NameValueCollection> NameValueCollectionList = new List<NameValueCollection>();

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
            Run(NameValueCollectionList);
            
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
        /// Runs the specified args.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void Run(List<NameValueCollection> args)
        {
            Assert.ArgumentNotNull(args, "args");
            UrlString urlString = new UrlString("/sitecore/shell/Applications/Publish.aspx");
            foreach (var Parameters in args)
            {
                string itemPath = Parameters["id"];
                string name = Parameters["language"];
                string value = Parameters["version"];
                Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];
                if (item == null)
                {
                    SheerResponse.Alert("Item not found.", Array.Empty<string>());
                    return;
                }
                    Assert.ArgumentNotNull(item, "item");
                    SheerResponse.CheckModified(false);
                   
                    urlString.Append("id", item.ID.ToString());
            }
            SheerResponse.Broadcast(SheerResponse.ShowModalDialog(urlString.ToString()), "Shell");
        }
      
    }
}
