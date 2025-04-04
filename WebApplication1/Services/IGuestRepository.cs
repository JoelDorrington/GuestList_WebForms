using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Services
{
    public class Guest
    {
        public Guest(string name, DateTime date)
        {
            Name = name;
            Date = date;   
        }
        public Guest(string name)
        {
            Name = name;
            Date = DateTime.Now;
        }
        public string Name { get; }
        public DateTime Date { get; }
    }
    public interface IGuestRepository
    {
        void AddGuest(string name);
        IEnumerable<Guest> GetGuests(GuestRepository.GetGuestsParams queryParams);
        int CountGuests(GuestRepository.CountGuestsParams queryParams);

        IEnumerable<Guest> Select_GuestList(DateTime from, string sortExpression, int startRowIndex, int maximumRows);
        IEnumerable<Guest> Select_GuestList(DateTime from, string sortExpression);
        int SelectCount_GuestList(DateTime from);
    }
}
