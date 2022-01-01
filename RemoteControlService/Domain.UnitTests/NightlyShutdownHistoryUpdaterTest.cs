using Domain.NightlyShutdown;
using Moq;
using RemoteControlService.UniTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class NightlyShutdownHistoryUpdaterTest
    {
        readonly IShutdownHistoryStorage shutdownHistoryStorage;

        public NightlyShutdownHistoryUpdaterTest()
        {
            shutdownHistoryStorage = new ShutdownHistoryStorageMock();
        }

        [Theory]
        [MemberData(nameof(GetDayTimeDateTimes))]
        public void UpdateShutdownHistory_WhenLastShutdownOccuredAtDaytime_ThenNotAddedToShutdownHistoryStorage(DateTime lastSystemShutdown)
        {
            var systemInformationMock = new Mock<ISystemInformation>();
            systemInformationMock.Setup(s => s.GetLastSystemShutdown()).Returns(lastSystemShutdown);
            NightlyShutdownHistoryUpdater sut = new NightlyShutdownHistoryUpdater(systemInformationMock.Object, shutdownHistoryStorage);

            sut.UpdateShutdownHistory();

            Assert.Empty(shutdownHistoryStorage.GetAll());
        }

        [Theory]
        [MemberData(nameof(GetNightTimeDateTimes))]
        public void UpdateShutdownHistory_WhenLastShutdownOccuredAtNightTime_ThenAddedToShutdownHistoryStorage(DateTime lastSystemShutdown)
        {
            var systemInformationMock = new Mock<ISystemInformation>();
            systemInformationMock.Setup(s => s.GetLastSystemShutdown()).Returns(lastSystemShutdown);
            NightlyShutdownHistoryUpdater sut = new NightlyShutdownHistoryUpdater(systemInformationMock.Object, shutdownHistoryStorage);

            sut.UpdateShutdownHistory();

            Assert.True(shutdownHistoryStorage.GetAll().Count() == 1);
        }

        [Fact]
        public void UpdateShutdownHistory_WhenShutdownHistoryIsFull_ThenOldestDateTimeRemoved()
        {
            var systemInformationMock = new Mock<ISystemInformation>();
            systemInformationMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 12, 13, 3, 0, 0));
            NightlyShutdownHistoryUpdater sut = new NightlyShutdownHistoryUpdater(systemInformationMock.Object, shutdownHistoryStorage);
            DateTime oldestShutdown = new DateTime(2020, 12, 8, 2, 0, 0);
            shutdownHistoryStorage.Add(oldestShutdown);
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 9, 1, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 10, 0, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 11, 23, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 12, 22, 0, 0));

            sut.UpdateShutdownHistory();

            Assert.True(shutdownHistoryStorage.GetAll().Count() == 5); // 5 is HISTORY_MAX_SIZE
            Assert.DoesNotContain(oldestShutdown, shutdownHistoryStorage.GetAll());
        }

        [Fact]
        public void UpdateShutdownHistory_WhenShutdownHistoryIsFullAndLatestShutdownAlreadyInStorage_ThenNothingChanged()
        {
            var systemInformationMock = new Mock<ISystemInformation>();
            systemInformationMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 12, 12, 22, 0, 0));
            NightlyShutdownHistoryUpdater sut = new NightlyShutdownHistoryUpdater(systemInformationMock.Object, shutdownHistoryStorage);
            DateTime oldestShutdown = new DateTime(2020, 12, 8, 2, 0, 0);
            shutdownHistoryStorage.Add(oldestShutdown);
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 9, 1, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 10, 0, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 11, 23, 0, 0));
            shutdownHistoryStorage.Add(new DateTime(2020, 12, 12, 22, 0, 0));

            sut.UpdateShutdownHistory();

            Assert.True(shutdownHistoryStorage.GetAll().Count() == 5); // 5 is HISTORY_MAX_SIZE
            Assert.Contains(oldestShutdown, shutdownHistoryStorage.GetAll());
        }

        public static IEnumerable<object[]> GetDayTimeDateTimes()
        {
            return new object[][]
            {
                new object[] { new DateTime(2020, 12, 12, 13, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 7, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 21, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 18, 0, 0) }
            };
        }

        public static IEnumerable<object[]> GetNightTimeDateTimes()
        {
            return new object[][]
            {
                new object[] { new DateTime(2020, 12, 12, 22, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 6, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 0, 0, 0) },
                new object[] { new DateTime(2020, 12, 12, 1, 0, 0) }
            };
        }
    }
}
