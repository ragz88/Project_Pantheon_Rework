using System.IO;


/// <summary>
/// This class is explicitely designed to save information regarding any new and unexpected beat combinations that can be used to 
/// solve a level, found by the player at runtime.
/// These new solutions will be stored in a csv file that we as developers can access and analyse later.
/// </summary>
public class UnexpectedSolutionRecorded
{
    public static void StoreNewSolution()
    {
        // We start by creating a string that represents the current settings of each beat controller and has some information about
        // where the player used this solution (through the currentSong)
        string newSolutionInfo = BeatTimingManager.btmInstance.currentSong.name + " => ";

        // After recording which song was playing, we'll analyse the state of each of our beat managers
        for (int i = 0; i < BeatTimingManager.btmInstance.beatControllers.Length; i++)
        {
            // Record which beat manager we're looking at
            newSolutionInfo = newSolutionInfo + BeatTimingManager.btmInstance.beatControllers[i].controllerColour.ToString() + ": ";

            // Then record it's current ActiveBeats setup
            for (int j = 0; j < BeatTimingManager.btmInstance.beatControllers[i].activeBeats.Length; j++)
            {
                // We'll represent active (true) beats with a T and inactive (false) beats with an F
                if (BeatTimingManager.btmInstance.beatControllers[i].activeBeats[j] == true)
                {
                    newSolutionInfo = newSolutionInfo + "T";
                }
                else
                {
                    newSolutionInfo = newSolutionInfo + "F";
                }

                // Finally, we separate beat array values with a ',' and whole controllers with a ';'
                if (j == BeatTimingManager.btmInstance.beatControllers[i].activeBeats.Length - 1)
                {
                    newSolutionInfo = newSolutionInfo + "; ";
                }
                else
                {
                    newSolutionInfo = newSolutionInfo + ",";
                }
            }
        }


        // We then want to search through our file (if it exists) to see if we've already stored this solution - in which case, there's 
        // no need to store it again
        StreamReader reader = new StreamReader("Assets/Resources/UnexpectedSolutions.txt");

        // Set to true if we find that we've already stored this solution
        bool solutionExistsAlready = false;

        // This will read the entire file, splitting each line into separate array elements
        string[] currentlyKnownSolutions = reader.ReadToEnd().Split(new char[] { '\n' });

        // Loop through every line that currently exists in our text file
        for (int i = 0; i < currentlyKnownSolutions.Length; i++)
        {
            // Check if solution already exists
            if (currentlyKnownSolutions[i].Contains(newSolutionInfo))
            {
                // We've found an exiting copy of this solution, so there's no need to store it.
                solutionExistsAlready = true;
                break;
            }
        }

        // Close file to prevent memory leaks
        reader.Close();


        // If the solution the player just found ISN'T present in the text file, we'll append it to the file's end
        if (!solutionExistsAlready)
        {
            StreamWriter writer = new StreamWriter("Assets/Resources/UnexpectedSolutions.txt", true);
            writer.WriteLine(newSolutionInfo);

            // Close file to prevent memory leaks
            writer.Close();
        }

    }
}
