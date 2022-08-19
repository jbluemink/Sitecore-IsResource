using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace IsResource.Commands
{
    [Serializable]
    public class ToggleReadOnly : Sitecore.Shell.Framework.Commands.ToggleReadOnly
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            if (context.Items.Length != 1 || context.Items[0] == null)
                return;
            Item item = context.Items[0];
            if (IsResourceItem(item))
            {
                var clientPipelineArgs = new ClientPipelineArgs();
                clientPipelineArgs.Parameters["items"] = item.ID.ToString();
                Context.ClientPage.Start((object)this, "Run", clientPipelineArgs);
            }
            else
            {
                //no resource item run Sitecore own logic.
                base.Execute(context);
            }
        }


        /// <summary>Runs the pipeline.</summary>
        /// <param name="args">The arguments.</param>
        public void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            string result = args.Result;
            if (result == "yes")
            {
                
                    Item item = Client.ContentDatabase.GetItem(args.Parameters["Items"]);
                    if (item != null)
                    {
                        //toggle the protected a.k.a readonly flag of the item.
                        using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
                        {
                            item.Editing.BeginEdit();
                            item.Appearance.ReadOnly = !item.Appearance.ReadOnly;
                            item.Editing.EndEdit();
                        }
                        Log.Audit((object)this, "Toggle resource item read only: {0}, value: {1} now in database", AuditFormatter.FormatItem(item), MainUtil.BoolToString(item.Appearance.ReadOnly));
                    }
            }
            else
            {
                if (result == "undefined" || result == "no")
                {
                    args.AbortPipeline();
                }
                else
                {
                    SheerResponse.Confirm("item is in a resource files.\n\nAre you sure you want to make changes?");
                    args.WaitForPostBack();    
                }
            } 
        }

        private bool IsResourceItem(Item item)
        {
            var deletepipeline = new Sitecore.Shell.Framework.Pipelines.DeleteItems();
            var deleteargs = new ClientPipelineArgs();
            deleteargs.Parameters["database"] = item.Database.Name;
            deleteargs.Parameters["items"] = item.ID.ToString();

            var OutputEnabled = WebUtil.GetItemsValue("SC_SHEERCOMMANDSENABLED");
            WebUtil.SetItemsValue("SC_SHEERCOMMANDSENABLED", (int)0);
            deletepipeline.FilterResourceItems(deleteargs);
            WebUtil.SetItemsValue("SC_SHEERCOMMANDSENABLED", OutputEnabled);

            if (deleteargs.Aborted)
            {
                return true;
            }

            return false;
        }
    } 
}
