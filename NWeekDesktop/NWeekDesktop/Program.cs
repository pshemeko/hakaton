﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;

namespace NWeekDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
            //MailClient.SendMail("sebastian.florczak@trw.com", "test", "test");
            Program.Welcome();
            OutlookReader outlookReader = new OutlookReader();
            List<AdxCalendarItem> CalendarItems = outlookReader.GetAllCalendarItems();

            DateTime Today = DateTime.Today;
            DateTime Now = DateTime.Now;
            var presentMeeting = new AdxCalendarItem();

            foreach (var ListElement in CalendarItems)
            {
                if (ListElement.StartTime < Now && ListElement.EndTime > Now && ListElement.StartDate == Today)
                {
                    presentMeeting = ListElement;
                    PrintMeetingParameters(ListElement);
                }


                //Console.WriteLine(ListElement.Recipients.ToString());
            }
            
            BaseClass aa = new BaseClass();
            aa.ImportFromDatabase();
            aa.CleanCatalog(aa.PathPutty);
            aa.CleanCatalog(aa.PathData);
            aa.Run();
            Console.WriteLine("Time to close the meeting");
            List<People> ktoByl = aa.CheckWhoFromRFIDIsEmployed();
            

            List<People> WhoShouldbe = aa.AllRequired(presentMeeting.RequiredAttendees);
            List<People> WhoForgetCome = aa.WhoMissing(WhoShouldbe);
            List<People> WhoWas = aa.ListUsersRFID;
            Console.WriteLine("Generated report");
            Console.WriteLine("Schould be on Meeting:");
            foreach (var item in WhoWas)
            {
                string subject = $"Summary of meeting: {presentMeeting.Subject.ToString()}";
                string bodyPresent = "Hello " + item.Name.ToString() + "," + "\n" + "Thank You for participating in a meeting: " + presentMeeting.Subject.ToString() +
                    " Your presence has been confirmed by UAK" + "\n" +
                    "This mail was generated by UAK system, please not respond";
                Console.WriteLine(item.Name.ToString());
                MailClient.SendMail(item.Email.ToString(), subject, bodyPresent);
                Console.WriteLine("Report sent");
            }

            Console.Write(Environment.NewLine);
            Console.WriteLine("Was'n came on Meeting:");
            foreach (var item in WhoForgetCome)
            {
                string subject = $"Summary of meeting: {presentMeeting.Subject.ToString()}";
                string bodyNotPresent = "Hello " + item.Name.ToString() + "," + "\n" + "You were on the list of required Attendees in a meeting: " + presentMeeting.Subject.ToString() +
                " but You were not there, Your unpresence has been confirmed by UAK" + "\n" +
                "This mail was generated by UAK system, please not respond";
                Console.WriteLine(item.Name.ToString());
                MailClient.SendMail(item.Email.ToString(), subject, bodyNotPresent);
                Console.WriteLine("Report sent");
            }
            
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();


        }

        public static void Welcome()
        {
            Console.WriteLine("==== Welcome in first ZF meeting presence registration system: =====");
            Console.WriteLine("========================== UAK =====================================");
            Console.WriteLine("==================== Ufaj Ale Kontroluj ============================");
            Console.WriteLine("====================================================================");
            Console.WriteLine("Wait a second, getting information from Outlook...");
        }
        public static void PrintMeetingParameters(AdxCalendarItem item)
        {
            Console.WriteLine("Present meeting parameters:");
            Console.WriteLine("Meeting subject: {0}", item.Subject);
            Console.WriteLine("Meeting Location: {0}", item.Location);
            Console.WriteLine("Meeting start date: {0}", item.StartDate.ToString("dd/MM/yyyy"));
            Console.WriteLine("Meeting end date: {0}", item.EndDate.Date.ToString("dd/MM/yyyy"));
            Console.WriteLine("Meeting start time: {0}", item.StartTime.ToString("HH:mm"));
            Console.WriteLine("Meeting end time: {0}", item.EndTime.ToString("HH:mm"));
            Console.WriteLine("Required Attendees: {0}", item.RequiredAttendees);

        }




    }
}

