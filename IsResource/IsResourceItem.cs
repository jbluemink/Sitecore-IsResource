using Sitecore.Web.UI.Sheer;
using Sitecore.Web;
using Sitecore.Data.Items;

namespace IsResource
{
    public static class IsResourceItem
    {
        public static bool IsResource(Item item)
        {
            var deletepipeline = new Sitecore.Shell.Framework.Pipelines.DeleteItems();
            var deleteargs = new ClientPipelineArgs();
            deleteargs.Parameters["database"] = item.Database.Name;
            deleteargs.Parameters["items"] = item.ID.ToString();

            //temporarily suspending alerts, to ignore alerts from the FilterResourceItems
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
