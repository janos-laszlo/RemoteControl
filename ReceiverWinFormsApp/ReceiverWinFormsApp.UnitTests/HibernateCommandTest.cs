using Domain.CommandFactories;
using Domain.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RemoteControlService.UniTests.Mocks;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class HibernateCommandTest
    {
        [Fact]
        public void Execute_Hibernation_WillHibernate()
        {
            CmdLinePowerControllerMock powerController = new(
                Substitute.For<ILogger<CmdLinePowerControllerMock>>());
            ICommand cmd = new HibernateCommand(powerController);

            cmd.Execute();

            Assert.Equal(1, powerController.NumOfHibernations);
        }
    }
}
