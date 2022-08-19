using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Save;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

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
