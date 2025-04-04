using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using WebApplication1.Services;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests.Services
{
    [TestClass]
    public class GuestRepository_Tests
    {
        private GuestRepository _guestRepository;
        private Mock<ISQLConnectionFactory> _sqlConnectionFactoryMock;
        private Mock<IDbConnection> _connection;
        private Mock<IDbCommand> _command;

        [TestInitialize]
        public void SetUp()
        {
            _sqlConnectionFactoryMock = new Mock<ISQLConnectionFactory>();
            _connection = new Mock<IDbConnection>();
            _command = new Mock<IDbCommand>();
            var parameter = new Mock<IDbDataParameter>();

            _guestRepository = new GuestRepository(_sqlConnectionFactoryMock.Object);
        }

        [TestMethod]
        public void AddGuestInsertsSQL()
        {
            // Arrange
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.ExecuteNonQuery()).Returns(1);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));
            _command.Setup(f => f.Dispose());
            _connection.Setup(f => f.Dispose());

            // Act
            _guestRepository.AddGuest("Name");

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.VerifySet(c => c.CommandText = "INSERT INTO Guests (Name, Date) VALUES (@Name, @Date)");
            _command.Verify(c => c.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
            _command.Verify(c => c.ExecuteNonQuery(), Times.Once);
            _command.Verify(c => c.Dispose(), Times.Once);
            _connection.Verify(c => c.Dispose(), Times.Once);
        }

        [TestMethod]
        public void GetGuests_DefaultParameters_ReturnsExpectedResults()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams();
            var expectedGuests = new List<Guest>
            {
                new Guest("Alice", DateTime.UtcNow),
                new Guest("Bob", DateTime.UtcNow.AddDays(-1))
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));

            // Mock the IDataReader to return the expected data
            _reader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            var nameSequence = _reader.SetupSequence(r => r.GetString(0));
            var dateSequence = _reader.SetupSequence(r => r.GetDateTime(1));
            foreach(var guest in expectedGuests)
            {
                nameSequence.Returns(guest.Name);
                dateSequence.Returns(guest.Date);
            }
            _command.Setup(f => f.ExecuteReader()).Returns(_reader.Object);

            // Act
            var result = _guestRepository.GetGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteReader(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            // Verify the result
            Assert.AreEqual(expectedGuests.Count, result.Count());
            for (int i = 0; i < expectedGuests.Count; i++)
            {
                Assert.AreEqual(expectedGuests[i].Name, result.ElementAt(i).Name);
                Assert.AreEqual(expectedGuests[i].Date, result.ElementAt(i).Date);
            }
        }

        [TestMethod]
        public void GetGuests_SortingByNameAscending_ReturnsSortedResults()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams
            {
                OrderBy = GuestRepository.SortKeys.Name,
                Ascending = true
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));

            // Mock the IDataReader to return the expected data
            _reader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            _reader.SetupSequence(r => r.GetString(0))
                .Returns("Alice")
                .Returns("Bob");
            _reader.SetupSequence(r => r.GetDateTime(1))
                .Returns(DateTime.UtcNow)
                .Returns(DateTime.UtcNow.AddDays(-1));
            _command.Setup(f => f.ExecuteReader()).Returns(_reader.Object);

            // Act
            var result = _guestRepository.GetGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteReader(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            // Verify the result
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Alice", result.ElementAt(0).Name);
            Assert.AreEqual("Bob", result.ElementAt(1).Name);
        }

        [TestMethod]
        public void GetGuests_Pagination_ReturnsPaginatedResults()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams
            {
                Limit = 10,
                Offset = 5
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));

            // Mock the IDataReader to return the expected data
            _reader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            _reader.SetupSequence(r => r.GetString(0))
                .Returns("Alice")
                .Returns("Bob");
            _reader.SetupSequence(r => r.GetDateTime(1))
                .Returns(DateTime.UtcNow)
                .Returns(DateTime.UtcNow.AddDays(-1));
            _command.Setup(f => f.ExecuteReader()).Returns(_reader.Object);

            // Act
            var result = _guestRepository.GetGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteReader(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            // Verify the result
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetGuests_DateFiltering_ReturnsFilteredResults()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.ExecuteNonQuery()).Returns(1);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));


            // Mock the IDataReader to return the expected data
            _reader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            _reader.SetupSequence(r => r.GetString(0))
                .Returns("Alice")
                .Returns("Bob");
            _reader.SetupSequence(r => r.GetDateTime(1))
                .Returns(DateTime.UtcNow)
                .Returns(DateTime.UtcNow.AddDays(-1));
            _command.Setup(f => f.ExecuteReader()).Returns(_reader.Object);

            // Act
            var result = _guestRepository.GetGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteReader(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            // Verify the result
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetGuests_EmptyResult_ReturnsNoGuests()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams
            {
                From = DateTime.UtcNow.AddYears(-1),
                To = DateTime.UtcNow.AddYears(-1)
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));

            // Mock the IDataReader to return no data
            _reader.Setup(r => r.Read()).Returns(false);
            _command.Setup(f => f.ExecuteReader()).Returns(_reader.Object);

            // Act
            var result = _guestRepository.GetGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteReader(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            // Verify the result
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetGuests_InvalidParameters_ThrowsException()
        {
            // Arrange
            var queryParams = new GuestRepository.GetGuestsParams
            {
                Limit = -1,
                Offset = -1
            };

            var _reader = new Mock<IDataReader>();
            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _guestRepository.GetGuests(queryParams));
        }
        [TestMethod]
        public void CountGuests_ReturnsExpectedCount()
        {
            // Arrange
            var queryParams = new GuestRepository.CountGuestsParams
            {
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow
            };

            _sqlConnectionFactoryMock.Setup(f => f.GetConnection()).Returns(_connection.Object);
            _connection.Setup(f => f.CreateCommand()).Returns(_command.Object);
            _command.Setup(f => f.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _command.Setup(f => f.Parameters.Add(It.IsAny<IDbDataParameter>()));
            _command.Setup(f => f.ExecuteScalar()).Returns(2);

            // Act
            var result = _guestRepository.CountGuests(queryParams);

            // Assert
            _sqlConnectionFactoryMock.Verify(f => f.GetConnection(), Times.Once);
            _connection.Verify(f => f.Open(), Times.Once);
            _connection.Verify(f => f.CreateCommand(), Times.Once);
            _command.Verify(f => f.ExecuteScalar(), Times.Once);
            _command.Verify(f => f.Dispose(), Times.Once);
            _connection.Verify(f => f.Dispose(), Times.Once);

            Assert.AreEqual(2, result);
        }


    }
}
