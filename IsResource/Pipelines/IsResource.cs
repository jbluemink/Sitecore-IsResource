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
            if (IsResourceItem.IsResource(item))
            {
                GetContentEditorWarningsArgs.ContentEditorWarning warning = args.Add();

                warning.Title = "Resource File";
                warning.Text = "This Item comes from a resource file.";
                warning.IsExclusive = true;
            }
        }
      
    }
}

