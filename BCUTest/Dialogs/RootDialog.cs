using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace BCUTest.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            await context.Forward(new QnADialog(), AfterQnA, activity, CancellationToken.None);

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            //// return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            //context.Wait(MessageReceivedAsync);
        }

        private async Task AfterQnA(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string message = null;

            try
            {
                message = (string)await result;
            }
            catch (Exception e)
            {
                await context.PostAsync($"QnAMaker: {e.Message}");
                // Wait for the next message
                context.Wait(MessageReceivedAsync);
            }

            // Display the answer from QnA Maker Service
            var answer = message;

            if (!string.IsNullOrEmpty(answer))
            {
                Activity reply = ((Activity)context.Activity).CreateReply();

                //if you want to add card
                //string[] qnaAnswerData = answer.Split('|');
                //string title = qnaAnswerData[0];
                //string description = qnaAnswerData[1];
                //string url = qnaAnswerData[2];
                //string imageURL = qnaAnswerData[3];

                //HeroCard card = new HeroCard
                //{
                //    Title = title,
                //    Subtitle = description,
                //};
                //card.Buttons = new List<CardAction>
                //{
                //    new CardAction(ActionTypes.OpenUrl, "Learn More", value: url)
                //};
                //card.Images = new List<CardImage>
                //{
                //    new CardImage( url = imageURL)
                //};
                //reply.Attachments.Add(card.ToAttachment());


                reply.Text = answer;
                await context.PostAsync(reply);
            }
            else
            {
                await context.Forward(new LuisDialog(), AfterLuis, activity, CancellationToken.None);
            }
        }

        private async Task AfterLuis(IDialogContext context, IAwaitable<object> result)
        {
            string luisAnswer = null;
            try
            {
                luisAnswer = (string)await result;
            }
            catch (Exception e)
            {
                await context.PostAsync($"Luisexception: {e.Message}");
                // Wait for the next message
                context.Wait(MessageReceivedAsync);
            }

            // handle intent
            await HandleLuisResponse(context, luisAnswer);
        }

        

        private async Task HandleLuisResponse(IDialogContext context,string intent)
        {
            //(context, AfterAskingAboutSurvey, "Would you like to take a survey?");
            switch (intent)
            {
                case "FindBranch":  PromptDialog.Number(context, GetNumber, "What is your zipcode ?");
                                    break;
                case "FindAtm":     PromptDialog.Number(context, GetNumber, "What is your zipcode ?");
                                    break;

                case "None":        await context.PostAsync("Sorry ! cannot find an answer for that");
                                    break;
                default:break;

            }
        }

        private async Task GetNumber(IDialogContext context, IAwaitable<double> result)
        {
            var number = await result;
            await context.PostAsync("Finding results for zipcode : "+number);
            // call external service and get the result
        }

        private async Task AfterAskingFollowup(IDialogContext context, IAwaitable<bool> result)
        {
            var takeSurvey = await result;
            if (!takeSurvey)
            {
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                var survey = new FormDialog<SurveyDialog>(new SurveyDialog(), SurveyDialog.BuildForm, FormOptions.PromptInStart, null);
                context.Call<SurveyDialog>(survey, AfterSurvey);
            }
        }

        private async Task AfterSurvey(IDialogContext context, IAwaitable<SurveyDialog> result)
        {

            SurveyDialog survey = await result;
            await context.PostAsync("Thanks for filling out the form! One last question. Please, describe how the game made you feel.");
            // context.Call(new AskLuis(), AfterAskLuis);
        }
    }
}
