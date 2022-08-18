using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetContentEditorWarnings;
using Sitecore.Web.UI.Sheer;
using System;

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
                GetContentEditorWarningsArgs.ContentEditorWarning warning = args.Add();

                warning.Title = "Resource File";
                warning.Text = "This Item comes from a resource file.";
                warning.IsExclusive = true;
            }
        }
      
    }
}

