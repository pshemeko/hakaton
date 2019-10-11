using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Permissions;

namespace NWeekDesktop
{
    class BaseClass
    {

        public List<People> ListUsersRFID; // only UID no names

        public List<People> ListUsersDatabase; // list with names
        public string PathData { get; set; }
        string PathDatabase { get; set; }
        public string PathPutty { get; set; }

        bool stillRun;

        public BaseClass()
        {
            this.ListUsersRFID = new List<People>();
            this.ListUsersDatabase = new List<People>();
            PathData = "../../Data/";
            PathDatabase = "../../Database/";
            PathPutty = "../../Putty/";
            stillRun = true;
        }



        static public void CopyFiles(string from, string where)
        {
            //delete from catalog
            System.IO.DirectoryInfo di = new DirectoryInfo(where);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            var paths = Directory.GetFiles(from);
            foreach (var fileToCopy in paths)
            {
                File.Copy(fileToCopy, where + Path.GetFileName(fileToCopy));
            }
        }
        public void CloseMeeting()
        {
            ListUsersRFID.Clear();
        }

        public void CleanCatalog(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);
                         
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception error)
                    {
                        //Console.WriteLine("BLAD KASOWANIA");
                        //Console.WriteLine(error);
                    }
                }            
        }

        public List<People> ReadFromCatalog(string path)
        {
            List<People> returnList = new List<People>();
            string uidText = "UID:";
            
            var paths = Directory.GetFiles(path);
            foreach (var file in paths)
            { 
                string[] lines = System.IO.File.ReadAllLines(file);

                // Display the file contents by using a foreach loop.
                //System.Console.WriteLine("Contents of putty.txt = ");
                //bool nextLine = true;
                string allInfo = "";
                string nrUUID = "";
                string koniecMeetingu = "KONIEC MEETINGU";
                foreach (string line in lines)
                {
                    // Use a tab to indent each line of the file.
                    //Console.WriteLine("\t" + line);
                    /*
                    if (line.Contains(startText))
                    {
                        nextLine = false;
                        if (nrUUID != "")
                        { 
                            People pp = new People(nrUUID, allInfo);
                            returnList.Add(pp);
                        }
                    }
                    */

                    if(line.Contains(uidText))
                    {
                        nrUUID = line.Substring(10);

                        //Console.WriteLine("\n" + "DUPA" + "\t" + nrUUID);
                        People pp = new People(nrUUID,"","",allInfo);
                        if(!returnList.Contains(pp))
                        {
                            returnList.Add(pp);
                        }
                    }
                    if(line.Contains(koniecMeetingu))
                    {
                        stillRun = false;
                    }
                }                
            }
            return returnList;           
        }

        public List<People> ReadFromCatalogDatabase(string path)
        {
            List<People> returnList = new List<People>();
            string uidText = "UID:";

            var paths = Directory.GetFiles(path);
            foreach (var file in paths)
            {
                string[] lines = System.IO.File.ReadAllLines(file);

                string allInfo = "";
                string nrUUID = "";
                foreach (string line in lines)
                {
                    // Use a tab to indent each line of the file.
                    //Console.WriteLine("\t" + line);
                    /*
                    if (line.Contains(startText))
                    {
                        nextLine = false;
                        if (nrUUID != "")
                        { 
                            People pp = new People(nrUUID, allInfo);
                            returnList.Add(pp);
                        }
                    }
                    */

                    if (line.Contains(uidText))
                    {
                        nrUUID = line.Substring(10,11);
                        Char beginName = '=';
                        int Startname = line.IndexOf(beginName);
                        //string name = line.Substring(Startname+1);
                        //Console.WriteLine("\n" + "DUPA" + "\t" + nrUUID);
                        string nam ="", mail = "";
                        var subStrings = line.Split(';');
                        foreach (var item in subStrings)
                        {
                            if(item.Contains("name"))
                            {
                                Char charRange = '"';
                                int startIndex = item.IndexOf(charRange);
                                int endIndex = item.LastIndexOf(charRange);
                                int length = endIndex - startIndex + 1;
                                nam = item.Substring(startIndex+1, length - 2);
                                //nam.Replace("\"", "");
                            }
                            if (item.Contains("e-mail"))
                            {
                                Char charRange = '"';
                                int startIndex = item.IndexOf(charRange);
                                int endIndex = item.LastIndexOf(charRange);
                                int length = endIndex - startIndex + 1;
                                mail = item.Substring(startIndex + 1, length - 2);
                                //mail = item.Substring(8);
                                mail.Replace("\"", "");
                            }
                            
                        }
                        int a = 5;


                        People pp = new People(nrUUID, nam, mail,  allInfo);
                        if (!returnList.Contains(pp))
                        {
                            returnList.Add(pp);
                        }
                    }
                }
            }


            return returnList;

        }
        
        public void ImportFromDatabase()
        {
            List<People> listP = ReadFromCatalogDatabase(PathDatabase);

            foreach (var people in listP)
            {
                if(!ListUsersDatabase.Contains(people))
                {
                    ListUsersDatabase.Add(people);
                }
            }
        }

        public int ImportFromRFIDFile()
        {
            List<People> listP = ReadFromCatalog(PathData);
            int HowmanyChanges = 0;

            foreach (var people in listP)
            {
                if (!ListUsersRFID.Contains(people))
                {
                    // warunek czy ten czlowiek jest w bazie czyli zatrudniony


                    // find person in database and add
                    var usr = ListUsersDatabase.Find(x => x.UID.Contains(people.UID));
                    if(usr !=null)
                    {
                        ListUsersRFID.Add(usr);
                        HowmanyChanges++;
                    }
                    else
                    {
                        Console.Write(Environment.NewLine);
                        Console.WriteLine("CAUTION:");
                        Console.WriteLine("User With UID:" + people.UID + " not working in Company");
                        Console.Write(Environment.NewLine);
                    }
                    
                }
            }
            return HowmanyChanges;
        }

        public List<People> CheckWhoFromRFIDIsEmployed()
        {
            List<People> listmatch = new List<People>();

            foreach (var people in ListUsersRFID)
            {
                if(ListUsersDatabase.Contains(people))
                {
                    var us = ListUsersDatabase.Find(x => x.UID.Contains(people.UID));
                    listmatch.Add(us);
                }
                else
                {
                    Console.WriteLine("Nie ma takiego uzytkownika w Bazie:" + people.ToString());
                    //TODO okienko zrob wyskakujace
                }
                int a = 9;
            }
            return listmatch;
        }

         public List<People> CheckWhoFromListIsOnMeeting(List<String> names) // sprawdzaj czy te nazwiska sa w liscie RFID
        {
            List<People> returnList = new List<People>();
            foreach (var person in ListUsersDatabase)
            {
                if(names.Contains(person.Name))
                {
                    returnList.Add(person);
                }                
            }
            return returnList;
        }
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public  void Run()
        {
            /*
            string[] args = Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            if (args.Length != 2)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }
            */
            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = PathPutty;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Size;

                // Only watch text files.
                watcher.Filter = "*.txt";

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;
                //watcher.NotifyFilter = NotifyFilters.FileName;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                //Console.WriteLine("Press 'q' to quit the sample.");
                //while (Console.Read() != 'q') ;
                while (stillRun)
                {
                    Update();
                };
            }
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            /*
            // Specify what is done when a file is changed, created, or deleted.
            // Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            System.Threading.Thread.Sleep(5000);
            CleanCatalog(PathData);

            BaseClass.CopyFiles("../../Putty/", "../../Data/");
            
            int howManyChanges = ImportFromRFIDFile();

            if(howManyChanges>0)
            {
                Console.WriteLine("Welcome on Meeting:");

                int size = ListUsersRFID.Count();
                for (int i = ListUsersRFID.Count() - howManyChanges; i < ListUsersRFID.Count(); i++)
                {

                    Console.WriteLine(ListUsersRFID.ElementAt(i));
                }
            }
            */
        }
        private void Update()
        {
            // Specify what is done when a file is changed, created, or deleted.
            // Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            System.Threading.Thread.Sleep(3000);
            CleanCatalog(PathData);

            BaseClass.CopyFiles("../../Putty/", "../../Data/");

            int howManyChanges = ImportFromRFIDFile();

            if (howManyChanges > 0)
            {
                Console.WriteLine("Welcome on Meeting:");

                int size = ListUsersRFID.Count();
                for (int i = ListUsersRFID.Count() - howManyChanges; i < ListUsersRFID.Count(); i++)
                {

                    Console.WriteLine(ListUsersRFID.ElementAt(i));
                }
            }

        }


        private static void OnRenamed(object source, RenamedEventArgs e) =>
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

        public List<People> AllRequired(string wszyscy)
        {
            var members = wszyscy.Split(';');
            List<People> returnList = new List<People>();

            foreach (var member in members)
            {
                foreach (var person in ListUsersDatabase)
                {
                    if (person.Name.Contains(member))
                    {
                        returnList.Add(person);
                    }
                    else if (member.Contains(person.Name))
                    {
                        returnList.Add(person);
                    }
                }
            }
            return returnList;
        }

        // kto powinien byc a go nie bylo
        public List<People> WhoMissing(List<People> whoSchould)
        {
            List<People> returnList = new List<People>();
            //wszyscy co powinni byc minus wszyscy z RFID
            foreach (var item in whoSchould)
            {
                if(!ListUsersRFID.Contains(item))
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }


    }

}
