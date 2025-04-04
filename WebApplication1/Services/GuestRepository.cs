using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace WebApplication1.Services
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ISQLConnectionFactory _sqlConnectionFactory;

        public GuestRepository(ISQLConnectionFactory connectionFactory)
        {
            _sqlConnectionFactory = connectionFactory;
        }
        public GuestRepository()
        {
            _sqlConnectionFactory = new SQLConnectionFactory();
        }

        public void AddGuest(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("Name cannot be empty");
            Guest data = new Guest(name);
            using (var connection = _sqlConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Guests (Name, Date) VALUES (@Name, @Date)";
                    AddParameter(command, "@Name", data.Name);
                    AddParameter(command, "@Date", data.Date);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameter(IDbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
        }
        private void AddParameter(IDbCommand command, string name, DateTime value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            command.Parameters.Add(param);
        }



        private void LogCommand(IDbCommand command)
        {
            var commandText = command.CommandText;
            foreach (IDbDataParameter param in command.Parameters)
            {
                var value = param.Value is string ? $"'{param.Value}'" : param.Value;
                commandText = commandText.Replace(param.ParameterName, value.ToString());
            }
            Debug.WriteLine(commandText);
        }

        public IEnumerable<Guest> GetGuests(GetGuestsParams args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.OrderBy != SortKeys.Name && args.OrderBy != SortKeys.Date)
                throw new ArgumentException("Invalid OrderBy value.", nameof(args.OrderBy));
            if (args.Limit <= 0) throw new ArgumentException("Limit must be greater than zero.", nameof(args.Limit));
            if (args.Offset < 0) throw new ArgumentException("Offset cannot be negative.", nameof(args.Offset));
            if (args.From.HasValue && args.To.HasValue && args.From.Value > args.To.Value)
                throw new ArgumentException("From date cannot be later than To date.", nameof(args.From));

            var guests = new List<Guest>();
            using (var connection = _sqlConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var query = "SELECT Name, Date FROM Guests";
                    if (args.From.HasValue || args.To.HasValue)
                    {
                        query += " WHERE";
                    }
                    if (args.From.HasValue)
                    {
                        query += " Date >= @From";
                        AddParameter(command, "@From", args.From.Value);
                        if (args.To.HasValue)
                        {
                            query += " AND";
                        }
                    }
                    if (args.To.HasValue)
                    {
                        query += " Date <= @To";
                        AddParameter(command, "@To", args.To.Value);
                    }
                    query += $" ORDER BY {args.OrderBy} {(args.Ascending ? "ASC" : "DESC")}";
                    query += " OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
                    AddParameter(command, "@Offset", args.Offset);
                    AddParameter(command, "@Limit", args.Limit);
                    command.CommandText = query;
                    LogCommand(command);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);
                            var date = reader.GetDateTime(1);
                            guests.Add(new Guest(name, date));
                        }
                    }
                }
            }
            return guests;
        }
        public int CountGuests(CountGuestsParams queryParams)
        {
            if (queryParams == null) throw new ArgumentNullException(nameof(queryParams));
            if (queryParams.From.HasValue && queryParams.To.HasValue && queryParams.From.Value > queryParams.To.Value)
                throw new ArgumentException("From date cannot be later than To date.", nameof(queryParams.From));

            using (var connection = _sqlConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var query = "SELECT COUNT(*) FROM Guests";
                    if (queryParams.From.HasValue || queryParams.To.HasValue)
                    {
                        query += " WHERE";
                    }
                    if (queryParams.From.HasValue)
                    {
                        query += " Date >= @From";
                        AddParameter(command, "@From", queryParams.From.Value);
                        if (queryParams.To.HasValue)
                        {
                            query += " AND";
                        }
                    }
                    if (queryParams.To.HasValue)
                    {
                        query += " Date <= @To";
                        AddParameter(command, "@To", queryParams.To.Value);
                    }
                    command.CommandText = query;

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }


        public enum SortKeys
        {
            Name,
            Date
        }

        public class CountGuestsParams
        {
            public DateTime? From { get; set; } = null;
            public DateTime? To { get; set; } = null;
        }

        public class GetGuestsParams : CountGuestsParams
        {
            public SortKeys OrderBy { get; set; } = SortKeys.Date;
            public bool Ascending { get; set; } = false;
            public int Limit { get; set; } = 10;
            public int Offset { get; set; } = 0;

            public static GetGuestsParams FromJson(string json)
            {
                return JsonConvert.DeserializeObject<GetGuestsParams>(json);
            }

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
        public IEnumerable<Guest> Select_GuestList(DateTime from, string sortExpression, int startRowIndex = 0, int maximumRows = 10)
        {
            // Determine if sortExpression ends with " ASC" or " DESC"
            bool ascending = true;
            if (!string.IsNullOrEmpty(sortExpression))
            {
                if (sortExpression.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
                {
                    ascending = false;
                    sortExpression = sortExpression.Substring(0, sortExpression.Length - 5);
                }
                else if (sortExpression.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                {
                    ascending = true;
                    sortExpression = sortExpression.Substring(0, sortExpression.Length - 4);
                }
            }

            SortKeys orderBy;
            if (!Enum.TryParse(sortExpression, true, out orderBy))
            {
                orderBy = SortKeys.Date;
            }

            var queryParams = new GetGuestsParams
            {
                Offset = startRowIndex,
                Limit = maximumRows,
                OrderBy = string.IsNullOrEmpty(sortExpression) ? SortKeys.Date : (SortKeys)Enum.Parse(typeof(SortKeys), sortExpression),
                Ascending = ascending,
                From = from,
                To = from.AddDays(1).AddTicks(-1)
            };
            return GetGuests(queryParams);
        }

        public int SelectCount_GuestList(DateTime from)
        {
            var queryParams = new CountGuestsParams
            {
                From = from,
                To = from.AddDays(1).AddTicks(-1)
            };
            return CountGuests(queryParams);
        }
    }
}