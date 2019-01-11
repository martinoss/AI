﻿using Autofac;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Solutions.Dialogs;
using Microsoft.Bot.Solutions.Dialogs.BotResponseFormatters;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillTemplateTest.Dialogs.Fakes;
using System.Threading;
using TestFramework;
using TestSkill1.ServiceClients;

namespace SkillTemplateTest.Dialogs
{
    public class SkillDialogTestBase : BotTestBase
    {
        public ConversationState ConversationState { get; set; }

        public UserState UserState { get; set; }

        public IBotTelemetryClient TelemetryClient { get; set; }

        public IServiceManager ServiceManager { get; set; }

        public SkillConfigurationBase Services { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            var builder = new ContainerBuilder();

            ConversationState = new ConversationState(new MemoryStorage());
            UserState = new UserState(new MemoryStorage());
            TelemetryClient = new NullBotTelemetryClient();
            Services = new MockSkillConfiguration();

            builder.RegisterInstance(new BotStateSet(UserState, ConversationState));
            Container = builder.Build();

            BotResponseBuilder = new BotResponseBuilder();
            BotResponseBuilder.AddFormatter(new TextBotResponseFormatter());
        }

        public TestFlow GetTestFlow()
        {
            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(ConversationState));

            var testFlow = new TestFlow(adapter, async (context, token) =>
            {
                var bot = BuildBot() as TestSkill1.TestSkill1;
                await bot.OnTurnAsync(context, CancellationToken.None);
            });

            return testFlow;
        }

        public override IBot BuildBot()
        {
            return new TestSkill1.TestSkill1(Services, ConversationState, UserState, TelemetryClient, null, true);
        }
    }
}
