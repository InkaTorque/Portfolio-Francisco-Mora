﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class CheckDirectories : Editor
{
    #region Load Variables
    //stage of fileCheckClear
    public int fileCheckClear = 0;
    private int missingFile = 0, skipFileCheck = 0;
    public string[] resourcesPath, resourceFile;
    private string[] directoryPath = new string[9], fileName = new string[4]; 
    #endregion
    
    public void BeginDirectoryCheck()
    {
        #region Start Lookup
        AssignDirectories();
        CheckPathsAndFiles();
        if (fileCheckClear == 1)
        {
            ResourceFiles();
            fileCheckClear = 2;
        }
        #endregion
    }

    private void AssignDirectories()
    {

        #region Setup Folder Paths
        directoryPath[0] = "Assets/";
        directoryPath[1] = directoryPath[0] + "Resources/";
        directoryPath[2] = directoryPath[1] + "EditorSprites/"; // resourcePath[0]
        directoryPath[3] = directoryPath[1] + "Prefabs/";
        directoryPath[4] = directoryPath[1] + "Sprites/";
        directoryPath[5] = directoryPath[0] + "Editor/";
        directoryPath[6] = directoryPath[3] + "BackGroundItems/";
        directoryPath[7] = directoryPath[3] + "GroundItems/";
        directoryPath[8] = directoryPath[3] + "ForegroundItems/";
        #endregion


        #region Setup File Names
        fileName[0] = directoryPath[2] + "GUIBox.PNG";
        fileName[1] = directoryPath[2] + "GUIBox2.PNG";
        fileName[2] = directoryPath[2] + "GUITilePointerSMALL.PNG";
        fileName[3] = directoryPath[1] + "TilePointerGizmo.prefab";
        #endregion
    }

    private void CheckPathsAndFiles()
    {
        #region Folder Check
        //Checks if Resources folder exists
        if (!Directory.Exists(directoryPath[1]))
        {
            Debug.LogError("'Resources' Folder Not Found. Folders will be created automatically.");
            skipFileCheck = 1;
            fileCheckClear = -1;
        }
        for (int a = 1; a < directoryPath.Length; a++)
        {
            //Checks if the required directories exist . if not , create them.
            if (!Directory.Exists(directoryPath[a]))
            {
                Debug.LogWarning("["+a+"] Missing " + directoryPath[a] + ". Creating Folder. Please refresh Unity (Alt + Tab) to see results.");
                Directory.CreateDirectory(directoryPath[a]);
            }
        }
        #endregion

        #region File Check
        if (skipFileCheck == 0)
        {
            for (int a = 0; a < fileName.Length; a++)
            {
                if (!File.Exists(fileName[a]))
                {
                    missingFile += 1;
                    if (a == 0)
                    {
                        Debug.LogError("File Not Found. Please create a 64 x 64 PNG image named 'GUIBox.png' and place it in the Images Folder!");
                    }
                    if (a == 1)
                    {
                        Debug.LogError("File Not Found. Please create a 64 x 64 PNG image named 'GUIBox2.png' and place it in the Images Folder!");
                    }
                    if (a == 2)
                    {
                        Debug.LogError("File Not Found. Please create a 64 x 64 PNG image named 'GUITilePointer.png' and place it in the Images Folder!");
                    }
                    if (a == 3)
                    {
                        Debug.LogError("File Not Found. Please create a GameObject named 'TilePointerGizmo.prefab' and place it in the Tile Prefabs Folder!");
                    }
                    /*if (a == 4)
                    {
                        Debug.LogError("File Not Found. Please create a GameObject named 'TilePointerGizmo.prefab' and place it in the Tile Prefabs Folder!");
                    }
                    if (a == 5)
                    {
                        Debug.LogError("File Not Found. Please create a Font named 'TileLabelFont.ttf' and place it in the Fonts Folder!");
                    }
                    if (a == 6)
                    {
                        Debug.LogError("CheckDirectories.cs not Found. Cannot proceed. Please refer to Read Me!");
                    }
                    if (a == 7)
                    {
                        Debug.LogError("LayersAndTags.cs not Found. Cannot proceed. Please refer to Read Me!");
                    }
                    if (a == 8)
                    {
                        Debug.LogError("SetTileTextures.cs not Found. Cannot proceed. Please refer to Read Me!");
                    }*/
                }
                if (a == fileName.Length - 1)
                {
                    if (missingFile == 0)
                    {
                        //All Files exist.
                        fileCheckClear = 1;
                    }
                    else
                    {
                        //File Missing.
                        fileCheckClear = -1;
                    }
                }
            }
        }
        #endregion
    }

    private void ResourceFiles()
    {
        #region Obtain Resource Paths
        resourcesPath = new string[directoryPath.Length];
        for (int a = 2; a < resourcesPath.Length; a++)
        {
            resourcesPath[a - 2] = directoryPath[a].Replace(directoryPath[1], "");
        }
        #endregion

        //Gathers the filepaths of the resources necessary to start up the tile map level creator
        #region Obtain Resource Files
        resourceFile = new string[fileName.Length];
        for (int a = 0; a < resourceFile.Length; a++)
        {
            resourceFile[a] = fileName[a].Replace(directoryPath[1], "");
            resourceFile[a] = resourceFile[a].Replace(".PNG", "");
            resourceFile[a] = resourceFile[a].Replace(".prefab", "");
            resourceFile[a] = resourceFile[a].Replace(".ttf", "");
            resourceFile[a] = resourceFile[a].Replace(".cs", "");
        }
        #endregion
    }
}
