using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;

public class PostBuilder
{
    [PostProcessBuildAttribute()]
    public static void OnPostProcessBuild( BuildTarget i_buildTarget, string i_pathToBuiltProject )
    {
        if( i_buildTarget == BuildTarget.iOS )
        {
            OnPostProcessBuild_iOS( i_buildTarget, i_pathToBuiltProject );
        }
    }

    private static void OnPostProcessBuild_iOS( BuildTarget i_buildTarget, string i_pathToBuiltProject )
    {
        var plistPath   = System.IO.Path.Combine( i_pathToBuiltProject, "Info.plist" );
        var plist       = new PlistDocument();
        plist.ReadFromFile( plistPath );

        plist.root.SetString( "NSCameraUsageDescription", "" );

        plist.WriteToFile( plistPath );
    }
} // class PostBuilder