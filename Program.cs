//  C. Winters / US / California / Culver City
//  Update-UpdateInfo updates the UPDATEINFO.XML (or any other XML file) by a specific tag.
//
// Example usage:
// We want to change the value of "LastVersion" tag with value "2.3.0.651" 
//
// File contents: UPDATEINFO.XML (cut down example)
//
//  <? xml version = "1.0" ?>
//  <root>
//    <LatestVersion>2.3.0.650</LatestVersion>
//    <LatestConfVersion>06/20/2020</LatestConfVersion>
//  </root>
//
// CMD LINE: TagUpdater.exe -tag:LatestVersion -value:2.3.0.661 UPDATEINFO.XML
//
using System;

namespace Update_UpdateInfo
{
    partial class Program
    {
        [STAThread]
        static int  Main(string[] args)
        {
            //Cheap command line processing
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-tag:"))
                {
                    string s = args[i].Substring("-tag:".Length);
                    sTag = s;
                }

                if (args[i].StartsWith("-value:"))
                {
                    sValue = args[i].Substring("-value:".Length);
                }

                if ((args[i].StartsWith("-help")) || (args[i].StartsWith("-?")) || (args[i].StartsWith("/?")))
                {
                    //Help!
                    ShowIntro();
                    ShowHelp();
                    return (0);
                }
               sXMLFileName = args[i];
            } //eNd of cheap command line processing.

            //Program ran, first time show intro
            ShowIntro(true);

            //Checks.
            if (sTag == "")
                sTag = "NULLTag";

            if (sValue == "")
                sValue = "NULL";

            //Set XML document object for our config
            xmlAppConfigDoc = new System.Xml.XmlDocument();

            //Load XML config
            if (!LoadAppConfig())
            {
                System.Console.WriteLine($"{sXMLFileName} failed to load. Does not exist., or invalid filename.\n\n");
                return (-2);
            }
            else
                System.Console.WriteLine($"{sXMLFileName} loaded.\n\n");

            if ( SetConfigbyTagName(sTag, "", sValue) )
            {
                SaveAppConfig();
                System.Console.WriteLine($"{sXMLFileName} file updated.\n\n");
            }
            else
            {
                System.Console.WriteLine($"{sXMLFileName} failed, file not updated. Could not find tag to change value.\n\n");
                return (-1);
            }

            //Everything good? Let's gracefully exit.
            return (0);
        }

    } //End of Class program
}
