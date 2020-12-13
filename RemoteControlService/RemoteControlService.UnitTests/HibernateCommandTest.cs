using Domain.Commands;
using RemoteControlService.UniTests.Mocks;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class HibernateCommandTest
    {
        [Fact]
        public void Execute_Hibernation_WillHibernate()
        {
            CmdLinePowerControllerMock powerController = new CmdLinePowerControllerMock();
            ICommand cmd = new HibernateCommand(powerController);

            cmd.Execute();

            Assert.Equal(1, powerController.NumOfHibernations);
        }
    }
}
