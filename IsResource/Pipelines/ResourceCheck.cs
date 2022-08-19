using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Save;
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
                        Item item = Client.ContentDatabase.GetItem(saveargitem.ID, saveargitem.Language, saveargitem.Version);
                        if (item != null)
                        {
                            if (IsResourceItem.IsResource(item))
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
    }
}
