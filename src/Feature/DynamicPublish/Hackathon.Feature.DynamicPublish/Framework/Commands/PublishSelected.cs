using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Hackathon.Feature.DynamicPublish.Framework.Commands
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

            string sc_selectedItems = "{6FD0AAC1-1ADE-4E3B-9EF2-7C8E0E419C0D},{CD3EAF80-AE0D-460C-91B4-BDBF9FD88340}"; //itemContext.Request.Cookies["sc_selectedItems"].Value;

            var itemIDs = sc_selectedItems.Split(',');
            foreach (var itemID in itemIDs)
            {
                Sitecore.Data.Database master =
                     Sitecore.Configuration.Factory.GetDatabase("master");
                Item item = master.GetItem(ID.Parse(itemID));
                Assert.ArgumentNotNull(context, "context");
                if (context.Items.Length != 1)
                {
                    return;
                }
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

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1 || !Settings.Publishing.Enabled)
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }

        /// <summary>
        /// Runs the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string itemPath = args.Parameters["id"];
            string name = args.Parameters["language"];
            string value = args.Parameters["version"];
            Item item = Client.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];
            if (item == null)
            {
                SheerResponse.Alert("Item not found.", Array.Empty<string>());
                return;
            }
            if (!PublishSelected.CheckWorkflow(args, item))
            {
                return;
            }
            if (!SheerResponse.CheckModified())
            {
                return;
            }
            if (args.IsPostBack)
            {
                if (args.Result == "yes")
                {
                    Database[] targets = PublishSelected.GetTargets();
                    if (targets.Length == 0)
                    {
                        SheerResponse.Alert("No target databases were found for publishing.", Array.Empty<string>());
                        return;
                    }
                    LanguageCollection languages = LanguageManager.GetLanguages(Context.ContentDatabase);
                    if (languages == null || languages.Count == 0)
                    {
                        SheerResponse.Alert("No languages were found for publishing.", Array.Empty<string>());
                        return;
                    }
                    bool publishRelatedItems = args.Parameters["related"] != "0";
                    bool deep = args.Parameters["subitems"] == "1";
                    bool compareRevisions = args.Parameters["smart"] != "0";
                    string message = string.Format("Publish item now: {0}", AuditFormatter.FormatItem(item));
                    Log.Audit(message, this);
                    PublishManager.PublishItem(item, targets, languages.ToArray(), deep, compareRevisions, publishRelatedItems);
                    SheerResponse.Alert("The item is being published.", Array.Empty<string>());
                    return;
                }
            }
            else
            {
                SheerResponse.Confirm(Translate.Text("Are you sure you want to publish \"{0}\"\nin every language to every publishing target?", new object[]
                {
                    item.DisplayName
                }));
                args.WaitForPostBack();
            }
        }

        /// <summary>
        /// Gets the targets.
        /// </summary>
        /// <returns></returns>
        private static Database[] GetTargets()
        {
            Item itemNotNull = Client.GetItemNotNull("/sitecore/system/publishing targets");
            ArrayList arrayList = new ArrayList();
            ChildList children = itemNotNull.Children;
            foreach (Item item in children)
            {
                string name = item["Target database"];
                Database database = Factory.GetDatabase(name, false);
                if (database != null)
                {
                    arrayList.Add(database);
                }
            }
            return Assert.ResultNotNull<Database[]>(arrayList.ToArray(typeof(Database)) as Database[]);
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
            if (Context.ClientPage.Modified)
            {
                args.Parameters["__modified"] = "1";
            }
            if (args.IsPostBack)
            {
                if (args.Parameters["__modified"] == "1")
                {
                    Context.ClientPage.Modified = true;
                }
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
                IWorkflowProvider workflowProvider = Client.ContentDatabase.WorkflowProvider;
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
