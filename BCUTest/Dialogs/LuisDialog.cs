using System;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Web;

namespace BCUTest.Dialogs
{
    [Serializable]
    public class LuisDialog : IDialog<object>
    {
        //load the LUIS service
        private LuisService _luisService = new LuisService(
                ConfigurationManager.AppSettings["LuisAPIHostName"],
                ConfigurationManager.AppSettings["LuisAppId"],
                ConfigurationManager.AppSettings["LuisAPIKey"]);

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedASync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedASync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = context.Activity as IMessageActivity;
            string answer = _luisService.GetIntent(activity.Text);
            context.Done(answer);
        }
    }
}