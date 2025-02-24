﻿
﻿using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string file = "mario.csv";
// make sure movie file exists
if (!File.Exists(file))
{
    logger.Error($"File does not exist: {file}");
    Console.WriteLine($"Error: File {file} not found.");
}
else
{
      // create parallel lists of character details
    // lists are used since we do not know number of lines of data
    List<UInt64> Ids = [];
    List<string> Names = [];
    List<string?> Descriptions = [];
    List<string> Species = [];
    List<string> FirstAppearances = [];
    List<int> YearsCreated = [];
    
     // to populate the lists with data, read from the data file
    try
    {
        StreamReader sr = new(file);
        // first line contains column headers
        
        sr.ReadLine();
        while (!sr.EndOfStream)
        {
            string? line = sr.ReadLine();
            if (line is not null)
             {
            string[] characterDetails = line.Split(',');

            if (characterDetails.Length >= 6) // Ensure all columns exist
            {
                Ids.Add(UInt64.Parse(characterDetails[0]));
                Names.Add(characterDetails[1]);
                Descriptions.Add(characterDetails[2]);
                Species.Add(characterDetails[3]);
                FirstAppearances.Add(characterDetails[4]);
                YearsCreated.Add(int.Parse(characterDetails[5]));
            }
            else
            {
                logger.Warn($"Skipping invalid line: {line}");
            }
        }
    }
    sr.Close();
    }
    catch (Exception ex)
    {
        logger.Error(ex.Message);
    }
    // create user menu
       string? choice;
    do
    {
        // display choices to user
        Console.WriteLine("1) Add Character");
        Console.WriteLine("2) Display All Characters");
        Console.WriteLine("Enter to quit");

        // input selection
        choice = Console.ReadLine();
        logger.Info("User choice: {Choice}", choice);

        if (choice == "1")
        {
            // Add Character
              Console.WriteLine("Enter new character name: ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)){
             // check for duplicate name
                if (Names.ConvertAll(n => n.ToLower()).Contains(name.ToLower()))
                {
                    logger.Info($"Duplicate name {name}");
                }
                else
                {
                    // generate id - use max value in Ids + 1
                    UInt64 Id = Ids.Max() + 1;
                   // input character description
                    Console.WriteLine("Enter description:");
            string? Description = Console.ReadLine();

                    Console.WriteLine("Enter species: ");
            string? species = Console.ReadLine();

            Console.WriteLine("Enter first appearance: ");
            string? firstAppearance = Console.ReadLine();

            Console.WriteLine("Enter year created: ");
            if (!int.TryParse(Console.ReadLine(), out int yearCreated))
            {
                logger.Error("Invalid year format.");
                return;
            }
                    // add new character details to Lists
                    Ids.Add(Id);
                    Names.Add(name);
                    Descriptions.Add(Description ?? "N/A");
                    Species.Add(species ?? "Unknown");
                    FirstAppearances.Add(firstAppearance ?? "Unknown");
                    YearsCreated.Add(yearCreated);
                    // log transaction
                    logger.Info($"Character id {Id} added");
                    //Append to CSV file
                     StreamWriter sw = new(file, true);
            sw.WriteLine($"{Id},{name},{Description},{species},{firstAppearance},{yearCreated}");
            sw.Close();
                }
            } else {
                logger.Error("You must enter a name");
            }
        }
        else if (choice == "2")
        {
            // Display All Characters
            // loop thru Lists
             for (int i = 0; i < Ids.Count; i++)
    {
        Console.WriteLine($"Id: {Ids[i]}");
        Console.WriteLine($"Name: {Names[i]}");
        Console.WriteLine($"Description: {Descriptions[i]}");
        Console.WriteLine($"Species: {Species[i]}");
        Console.WriteLine($"First Appearance: {FirstAppearances[i]}");
        Console.WriteLine($"Year Created: {YearsCreated[i]}");
        Console.WriteLine();
    }
}
    } while (choice == "1" || choice == "2");
}

logger.Info("Program ended");
