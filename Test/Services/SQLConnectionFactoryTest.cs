using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using WebApplication1.Services;

namespace WebApplication1.Tests.Services
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SQLConnectionFactory_Test
    {

        [TestMethod]
        public void Works()
        {
            SQLConnectionFactory factory = new SQLConnectionFactory("Server=localhost;Database=Guestlog;User Id=GuestLogApp;Password=****");
            var conn = factory.GetConnection();
            Assert.IsNotNull(conn);
            var comm = conn.CreateCommand();
            Assert.IsNotNull(comm);
        }
    }
}
