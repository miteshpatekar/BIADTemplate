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
            string safeText = HttpUtility.UrlEncode(((IMessageActivity)context.Activity).Text);
            string answer = _luisService.GetIntent(safeText);
            context.Done(answer);
        }
    }
}