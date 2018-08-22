using System;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Web;

namespace BCUTest.Dialogs
{
    [Serializable]
    public class QnADialog : IDialog<object>
    {
        private QnAMakerService _QnAService = new QnAMakerService(
                ConfigurationManager.AppSettings["QnAHost"],
                ConfigurationManager.AppSettings["QnAKnowledgebaseId"],
                ConfigurationManager.AppSettings["QnAEndPointKey"]);

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedASync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedASync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = context.Activity as IMessageActivity;
            string answer = _QnAService.GetAnswer(activity.Text);
            context.Done(answer);
        }
    }
}