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
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Hackathon.Feature.DynamicPublish.Commands
{

    [Serializable]
    public class PublishSelected : Command
    {
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {

            System.Web.HttpContext itemContext = System.Web.HttpContext.Current;

            //string sc_selectedItems = "{1BAB6C8F-6442-4A8E-867B-725C6A4C98F8},{CD3EAF80-AE0D-460C-91B4-BDBF9FD88340}"; 
            string sc_selectedItems = itemContext.Request.Cookies["sc_selectedItems"].Value;
            if (string.IsNullOrEmpty(sc_selectedItems))
            {
                SheerResponse.Alert("No items have been selected, select items using the check box then retry publish selected items", Array.Empty<string>());
                return;
            }

            var itemIDs = sc_selectedItems.Split(',');
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length == 1)
            {
                foreach (var itemID in itemIDs)
                {
                    Sitecore.Data.Database master =
                     Sitecore.Configuration.Factory.GetDatabase("master");
                    Guid itemGuid;
                    Guid.TryParse(itemID, out itemGuid);
                    Item item = master.GetItem(ID.Parse(itemGuid));

                    // Item item = context.Items[0];
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection["id"] = item.ID.ToString();
                    nameValueCollection["language"] = item.Language.ToString();
                    nameValueCollection["version"] = item.Version.ToString();
                    nameValueCollection["workflow"] = "0";
                    nameValueCollection["related"] = context.Parameters["related"];
                    nameValueCollection["subitems"] = context.Parameters["subitems"];
                    nameValueCollection["smart"] = context.Parameters["smart"];
                    Context.ClientPage.Start(this, "Run", nameValueCollection);
                }

            }
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
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string itemPath = args.Parameters["id"];
            string name = args.Parameters["language"];
            string value = args.Parameters["version"];
            if (!SheerResponse.CheckModified(new CheckModifiedParameters
            {
                ResumePreviousPipeline = true
            }))
            {
                return;
            }
            Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];
            if (item == null)
            {
                SheerResponse.Alert("Item not found.", Array.Empty<string>());
                return;
            }
            if (!PublishSelected.CheckWorkflow(args, item))
            {
                return;
            }
            Log.Audit(this, "Publish item: {0}", new string[]
            {
                AuditFormatter.FormatItem(item)
            });
            Items.Publish(item);
        }

        /// <summary>
        /// Checks the workflow.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="item">The item.</param>
        /// <returns>The workflow.</returns>
        private static bool CheckWorkflow(ClientPipelineArgs args, Item item)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(item, "item");
            if (args.Parameters["workflow"] == "1")
            {
                return true;
            }
            args.Parameters["workflow"] = "1";
            if (args.IsPostBack)
            {
                if (args.Result == "yes")
                {
                    args.IsPostBack = false;
                    return true;
                }
                args.AbortPipeline();
                return false;
            }
            else
            {
                SiteContext site = Factory.GetSite("publisher");
                if (site != null && !site.EnableWorkflow)
                {
                    return true;
                }
                IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
                if (workflowProvider == null || workflowProvider.GetWorkflows().Length == 0)
                {
                    return true;
                }
                IWorkflow workflow = workflowProvider.GetWorkflow(item);
                if (workflow == null)
                {
                    return true;
                }
                WorkflowState state = workflow.GetState(item);
                if (state == null)
                {
                    return true;
                }
                if (state.FinalState)
                {
                    return true;
                }
                args.Parameters["workflow"] = "0";
                if (state.PreviewPublishingTargets.Any<string>())
                {
                    return true;
                }
                SheerResponse.Confirm(Translate.Text("The current item \"{0}\" is in the workflow state \"{1}\"\nand will not be published.\n\nAre you sure you want to publish?", new object[]
                {
                    item.DisplayName,
                    state.DisplayName
                }));
                args.WaitForPostBack();
                return false;
            }
        }
    }
}
