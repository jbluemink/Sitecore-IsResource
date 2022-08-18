using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ItemProvider.SaveItem;
using Sitecore.Pipelines.Save;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sitecore.Pipelines.Save.SaveArgs;

namespace IsResource.Pipelines
{
    public class ResourceCheck
    {
        public void Process(SaveArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            string result = args.Result;
            if (!(result == "yes"))
            {
                if (result == "undefined" || result == "no")
                {
                    args.AbortPipeline();
                }
                else
                {
                    if (args.Items == null)
                        return;
                    int count = 0;
                    foreach (var saveargitem in args.Items)
                    {
                        Item item = Sitecore.Context.Database.GetItem(saveargitem.ID, saveargitem.Language, saveargitem.Version);
                        if (item != null)
                        {
                            if (IsResourceItem(item))
                            {
                                count++;
                            }
                        }
                    }
                    if (count > 0)
                    {
                        SheerResponse.Confirm("Some of your items are in resource files.\n\nDo you want to save the changes?");
                        args.WaitForPostBack();
                    }
                }
                
            }
        }

        private bool IsResourceItem(Item item)
        {
            var deletepipeline = new Sitecore.Shell.Framework.Pipelines.DeleteItems();
            var deleteargs = new ClientPipelineArgs();
            deleteargs.Parameters["database"] = item.Database.Name;
            deleteargs.Parameters["items"] = item.ID.ToString();
            var orgClientPage = Context.ClientPage;
            
            Context.ClientPage = new ClientPage();
            
            bool aborted = false;
            try
            {
                deletepipeline.FilterResourceItems(deleteargs);
            }
            catch (NullReferenceException)
            {
                aborted = true;
            }

            Context.ClientPage = orgClientPage;
            if (deleteargs.Aborted || aborted)
            {
                return true;
            }

            return false;
        }

    }
}
