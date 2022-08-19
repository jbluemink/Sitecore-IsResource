using Sitecore.Data.Items;
using Sitecore.Pipelines.GetContentEditorWarnings;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;


namespace IsResource.Pipelines
{
    public class IsResource
    {
        public void Process(GetContentEditorWarningsArgs args)
        {
            Item item = args.Item;
            if (item == null)
            {
                return;
            }
            Check(item, args);
        }

        private static void Check(Item item, GetContentEditorWarningsArgs args)
        {
            var deletepipeline = new Sitecore.Shell.Framework.Pipelines.DeleteItems();
            var deleteargs = new ClientPipelineArgs();
            deleteargs.Parameters["database"] = item.Database.Name;
            deleteargs.Parameters["items"] = item.ID.ToString();

            //temporarily suspending alerts, to ignore alerts from the FilterResourceItems
            var OutputEnabled = WebUtil.GetItemsValue("SC_SHEERCOMMANDSENABLED");
            WebUtil.SetItemsValue("SC_SHEERCOMMANDSENABLED",(int)0);
            deletepipeline.FilterResourceItems(deleteargs);
            WebUtil.SetItemsValue("SC_SHEERCOMMANDSENABLED", OutputEnabled);

            if (deleteargs.Aborted)
            {
                GetContentEditorWarningsArgs.ContentEditorWarning warning = args.Add();

                warning.Title = "Resource File";
                warning.Text = "This Item comes from a resource file.";
                warning.IsExclusive = true;
            }
        }
      
    }
}

