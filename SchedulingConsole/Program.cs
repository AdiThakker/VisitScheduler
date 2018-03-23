using Domain;
using Infrastructure;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var clientResult = Entity.CreateClient("Tony", "Stark", 40);
                if (clientResult.IsFailure)
                {
                    var message = clientResult as Result<Client, string>.Failure;
                    throw new InvalidOperationException(message.ToString());
                }
                var client = clientResult as Result<Client, string>.Success;

                var staffResult = Entity.CreateStaff("Pepper", "Potts", Specialty.General);
                if (staffResult.IsFailure)
                {
                    var message = staffResult as Result<Staff, string>.Failure;
                    throw new InvalidOperationException(message.ToString());
                }
                var staff = staffResult as Result<Staff, string>.Success;

                var visit = Entity.ScheduleVisit(client.Item, staff.Item, DateTime.Now, DateTime.Now.AddHours(1));
                Console.WriteLine(visit.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
