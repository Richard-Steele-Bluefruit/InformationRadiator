using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Collections.Generic;

using Moq;
using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

namespace LeanKitQuery.Tests
{
    [TestClass]
    public class DayLaneTotalsQueryTests
    {
        MemoryStream outputStream;
        StreamWriter output;
        MemoryStream errorStream;
        StreamWriter error;
        string[] parameters;

        DayLaneTotalsQuery targetConcrete;
        ILeanKitQuery target;

        Mock<ILeanKitApi> mockApi;
        Board mockBoard;

        List<CardView> mockArchiveCards;

        [TestInitialize]
        public void Setup()
        {
            outputStream = new MemoryStream();
            output = new StreamWriter(outputStream);

            errorStream = new MemoryStream();
            error = new StreamWriter(errorStream);

            parameters = new string[1];
            parameters[0] = "100";

            targetConcrete = new DayLaneTotalsQuery();
            target = targetConcrete;

            mockApi = new Mock<ILeanKitApi>(MockBehavior.Strict);
            mockBoard = new Board();
            mockBoard.Backlog = new List<Lane>();
            mockBoard.Lanes = new List<Lane>();
            mockBoard.Archive = new List<Lane>();

            mockArchiveCards = new List<CardView>();
        }

        [TestCleanup]
        public void TearDown()
        {
            mockApi.VerifyAll();
        }

        private int RunQuery()
        {
            return target.RunQuery(mockApi.Object, parameters, output, error);
        }

        [TestMethod]
        public void Querying_a_board_with_no_lanes()
        {
            mockApi.Setup(m => m.GetBoard(100)).Returns(mockBoard);
            mockApi.SetupSequence(m => m.SearchCards(100, It.IsAny<SearchOptions>()))
                .Returns(mockArchiveCards)
                .Returns(new List<CardView>());

            Assert.AreEqual(0, RunQuery());
        }
    }
}
