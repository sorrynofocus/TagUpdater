using System.IO;

namespace Update_UpdateInfo
{
    partial class Program
    {
        static private System.Xml.XmlDocument xmlAppConfigDoc = null;
        static private string sXMLFileName = "UPDATEINFO.XML";
        static private string sTag = "";
        static private string sValue = "";

        private static void ShowIntro(bool IsRun = false)
        {
            string sCompileDate = "[07-24-2020]";

            if (IsRun)
                System.Console.Write($"Running TagUpdater  -- Compile date: {sCompileDate}\n");
            else
                System.Console.Write($"TagUpdater  -- Compile date:  {sCompileDate}\n");

            System.Console.Write("XML/UpdateInfo update utility tool.\n");
            System.Console.Write("By C. Winters [thechriswinters@gmail.com]\n\n");
        }

        private static void ShowHelp()
        {
            System.Console.WriteLine("Usage: TagUpdater [options] <path to UPDATEINFO.XML or XML file>");
            System.Console.WriteLine("Options: ");
            System.Console.WriteLine("  -tag <tag>      - The XML element tag to search for.");
            System.Console.WriteLine("  -value <value>  - The value to add/replace into the source XML element tag");
        }

        /// <summary>
        /// Load an XML config into memory to process later.
        /// </summary>
        /// <returns>true, if successful, false otherise.</returns>
        static public bool LoadAppConfig()
        {
            FileStream fsXMLAppConfig = null;

            if (File.Exists(sXMLFileName))
            {
                try
                {
                    fsXMLAppConfig = new FileStream(sXMLFileName,
                                                    FileMode.Open,
                                                    FileAccess.Read);
                    // Open the XML file
                    xmlAppConfigDoc.Load(fsXMLAppConfig);
                }
                catch (System.Xml.XmlException ex)
                {
                    System.Console.WriteLine("Error in configuration: " + ex.Message);
                    fsXMLAppConfig.Close();
                    System.Environment.Exit(-255);
                }
                finally
                {
                    fsXMLAppConfig.Close();
                }
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// Closes XML config, clears memory
        /// </summary>
        static public void ClosedAppConfig()
        {
            xmlAppConfigDoc = null;
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Save current XML config previously loaded from memory, and possibly modifed. 
        /// </summary>
        static public void SaveAppConfig()
        {
            xmlAppConfigDoc.Save(sXMLFileName);
        }

        /// <summary>
        /// Set the config tag name to search and replace. 
        /// </summary>
        /// <param name="sElement">The element of the XML to search into</param>
        /// <param name="sAttr">The attrib to search for</param>
        /// <param name="sNewValue">New string to replace the value of the config element (sElement), or the aatribute (sAttr) of the config element (sElement)</param>
        /// <returns>true if successful, falsie otherwise.</returns>
        static public bool SetConfigbyTagName(string sElement, string sAttr, string sNewValue)
        {
            // Get a list of elements 
            System.Xml.XmlNodeList lstConfigElements = xmlAppConfigDoc.GetElementsByTagName(sElement);

            foreach (System.Xml.XmlNode nodeAttr in lstConfigElements)
            {
                //Did we find the first attribute? If so, add
                if (nodeAttr.Attributes.Count != 0)
                {
                    //Are there any attributes more than one and we have not asked for a specific attrib??
                    if (nodeAttr.Attributes.Count > 1)
                    {
                        //if we didn't pass in a specific attrib to look for, then get all attribs
                        if ((sAttr == "") || (sAttr == null))
                        {
                            for (int iIndex = 0; iIndex < nodeAttr.Attributes.Count; iIndex++)
                                nodeAttr.Attributes[iIndex].InnerText = sNewValue;
                        }
                        else
                        {
                            //Did we specify a search for attrib/element node?
                            for (int iIndex = 0; iIndex < nodeAttr.Attributes.Count; iIndex++)
                            {
                                if (nodeAttr.Attributes[iIndex].Name == sAttr)
                                {
                                    nodeAttr.Attributes[iIndex].InnerText = sNewValue;
                                    return (true);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((sAttr == "") || (sAttr == null))
                        {
                            nodeAttr.Attributes[0].InnerText = sNewValue;
                            return (true);
                        }
                        else//Find more than one attrib/element? Then let's find the attrib/element based on single search
                            if (nodeAttr.Attributes[0].Name == sAttr)
                        {
                            nodeAttr.Attributes[0].InnerText = sNewValue;
                            return (true);
                        }
                    }
                }

                //If we did not have any attribs at first, we might have an element as we dig deeper...
                if (nodeAttr.HasChildNodes == true && nodeAttr.Attributes.Count != 0)
                {
                    foreach (System.Xml.XmlNode nodeChild in nodeAttr.ChildNodes)
                    {
                        //Skip possible comment
                        if (nodeChild.Name != "#comment")
                        {
                            //Get any child element nodes and data
                            foreach (System.Xml.XmlNode nodeChildInChild in nodeChild)
                            {
                                if (nodeChildInChild.NodeType != System.Xml.XmlNodeType.Comment)
                                {
                                    nodeChildInChild.InnerText = sNewValue;
                                    return (true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Then let's just add regular attr without child nodes
                    if ((sAttr == "") || (sAttr == null))
                    {
                        //We might iterate all the attrib elements without specific search, and that's good,
                        //but let's not add the element we originally searched under
                        if ((nodeAttr.Name != sElement) && (nodeAttr.InnerText == ""))
                        {
                            nodeAttr.Attributes[0].InnerText = sNewValue;
                            return (true);
                        }
                        else
                        {
                            // If a single element was found with node data inside
                            // Example: <AppEnabled>true</AppEnabled> within the root
                            if ((nodeAttr.Name == sElement) && (nodeAttr.InnerText != ""))
                            {
                                nodeAttr.InnerText = sNewValue;
                                return (true);
                            }
                        }
                    }
                    else //Find more than one attrib/element? Then let's find the attrib/element based on single search
                        if (nodeAttr.InnerText == sAttr)
                    {
                        nodeAttr.Attributes[0].InnerText = sNewValue;
                        return (true);
                    }
                    ;
                }
            }
            //Code, laugh, pray. Return with results. 
            return (false);
        } //end of SetConfigbyTagName

    } //End of class Program
}
