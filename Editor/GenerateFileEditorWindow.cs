#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElRaccoone.EntityComponentSystem.Editor {
  public enum GenerateFileType {
    Controller,
    ComponentAndSystem,
    Service,
    Class
  }

  public class GenerateFileEditorWindow : EditorWindow {
    private static GenerateFileType generateFileType;
    private string fileName;
    private bool shouldOverwriteAllVirtuals;
    private bool shouldImportCommonNamespaces;
    private bool shouldAddFileHeaderComments;

    [MenuItem ("ECS/Generate Controller", false, 100)]
    private static void GenerateNewController () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.Controller;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Controller");
    }

    [MenuItem ("ECS/Generate Component and System", false, 100)]
    private static void GenerateNewComponentAndSystem () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.ComponentAndSystem;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Component and System");
    }

    [MenuItem ("ECS/Generate Service", false, 100)]
    private static void GenerateNewService () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.Service;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Service");
    }

    [MenuItem ("ECS/Generate Class", false, 100)]
    private static void GenerateNewClass () {
      GenerateFileEditorWindow.generateFileType = GenerateFileType.Class;
      EditorWindow.GetWindowWithRect (typeof (GenerateFileEditorWindow), new Rect (0, 0, 400, 180), true, "Generate Class");
    }

    private void OnGUI () {
      var _newFileTypeReadable = "";
      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Controller)
        _newFileTypeReadable = "Controller";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem)
        _newFileTypeReadable = "Component and System";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Service)
        _newFileTypeReadable = "Service";
      else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Class)
        _newFileTypeReadable = "Class";
      GUILayout.BeginHorizontal ();
      GUILayout.Space (20);
      GUILayout.BeginVertical ();
      GUILayout.Space (20);
      GUILayout.Label ("Name your new " + _newFileTypeReadable + "...", EditorStyles.largeLabel);
      this.fileName = GUILayout.TextField (this.fileName);
      GUILayout.FlexibleSpace ();
      this.shouldImportCommonNamespaces = GUILayout.Toggle (this.shouldImportCommonNamespaces, " Import Common Namespaces");
      this.shouldOverwriteAllVirtuals = GUILayout.Toggle (this.shouldOverwriteAllVirtuals, " Overwrite All Virtuals");
      this.shouldAddFileHeaderComments = GUILayout.Toggle (this.shouldAddFileHeaderComments, " Add File Header Comments");
      GUILayout.FlexibleSpace ();
      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Controller) {
        if (GUILayout.Button ("Generate " + this.fileName + "Controller"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem) {
        if (GUILayout.Button ("Generate " + this.fileName + "Component and -System"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Service) {
        if (GUILayout.Button ("Generate " + this.fileName + "Service"))
          this.Generate ();
      } else if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Class)
        if (GUILayout.Button ("Generate " + this.fileName))
          this.Generate ();
      GUILayout.Space (20);
      GUILayout.EndVertical ();
      GUILayout.Space (20);
      GUILayout.EndHorizontal ();
    }

    private void Generate () {
      var _overwriteAllVirtuals = this.shouldOverwriteAllVirtuals;
      var _importCommonNamespaces = this.shouldImportCommonNamespaces;
      var _addFileHeaderComments = this.shouldAddFileHeaderComments;
      var _fileName = this.fileName;
      this.fileName = "";

      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Controller)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Controllers"), _fileName + "Controller", "cs", new string[] {
          "using ElRaccoone.EntityComponentSystem;",
          _importCommonNamespaces ? "using UnityEngine;" : null,
          _importCommonNamespaces ? "using System.Collections.Generic;" : null,
          "",
          _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
          _addFileHeaderComments ? "/// Author: " : null,
          _addFileHeaderComments ? "/// Controller for " + _fileName + "." : null,
          "public class " + _fileName + "Controller : Controller {",
          "\tpublic override void OnInitialize () {",
          "\t\tthis.Register();",
          "\t}",
          _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnUpdate () { }" : null,
          "}"
        });

      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Components"), _fileName + "Component", "cs", new string[] {
          "using ElRaccoone.EntityComponentSystem;",
          _importCommonNamespaces ? "using UnityEngine;" : null,
          _importCommonNamespaces ? "using System.Collections.Generic;" : null,
          "",
          _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
          _addFileHeaderComments ? "/// Author: " : null,
          _addFileHeaderComments ? "/// Entity Component for " + _fileName + "." : null,
          "public class " + _fileName + "Component : EntityComponent<" + _fileName + "Component, " + _fileName + "System> {",
          "}"
        });

      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.ComponentAndSystem)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Systems"), _fileName + "System", "cs", new string[] {
          "using ElRaccoone.EntityComponentSystem;",
          _importCommonNamespaces ? "using UnityEngine;" : null,
          _importCommonNamespaces ? "using System.Collections.Generic;" : null,
          "",
          _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
          _addFileHeaderComments ? "/// Author: " : null,
          _addFileHeaderComments ? "/// Entity System for " + _fileName + "." : null,
          "public class " + _fileName + "System : EntitySystem<" + _fileName + "System, " + _fileName + "Component> {",
          _overwriteAllVirtuals ? "\tpublic override void OnInitialize () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnUpdate () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnDrawGizmos () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnDrawGui () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEnabled () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnDisabled () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEntityInitialize (" + _fileName + "Component entity) { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEntityInitialized (" + _fileName + "Component entity) { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEntityEnabled (" + _fileName + "Component entity) { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEntityDisabled (" + _fileName + "Component entity) { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnEntityWillDestroy (" + _fileName + "Component entity) { }" : null,
          _overwriteAllVirtuals ? "\tpublic override bool ShouldUpdate () { return true; }" : null,
          "}"
        });

      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Service)
        this.WriteContentToFile (this.FindDirectoryWithName (Application.dataPath, "Services"), _fileName + "Service", "cs", new string[] {
          "using ElRaccoone.EntityComponentSystem;",
          _importCommonNamespaces ? "using UnityEngine;" : null,
          _importCommonNamespaces ? "using System.Collections.Generic;" : null,
          "",
          _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
          _addFileHeaderComments ? "/// Author: " : null,
          _addFileHeaderComments ? "/// Service for " + _fileName + "." : null,
          "public class " + _fileName + "Service : Service<" + _fileName + "Service> {",
          _overwriteAllVirtuals ? "\tpublic override void OnInitialize () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnInitialized () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnDrawGizmos () { }" : null,
          _overwriteAllVirtuals ? "\tpublic override void OnDrawGui () { }" : null,
          "}"
        });

      if (GenerateFileEditorWindow.generateFileType == GenerateFileType.Class)
        this.WriteContentToFile (Application.dataPath, _fileName + "", "cs", new string[] {
          _importCommonNamespaces ? "using UnityEngine;" : null,
          _importCommonNamespaces ? "using System.Collections.Generic;" : null,
          _importCommonNamespaces ? "" : null,
          _addFileHeaderComments ? "/// Project: " + PlayerSettings.productName : null,
          _addFileHeaderComments ? "/// Author: " : null,
          _addFileHeaderComments ? "/// " + _fileName + "." : null,
          "public class " + _fileName + " {",
          "}"
        });

      this.Close ();
    }

    private string FindDirectoryWithName (string directory, string name) {
      var _directories = this.WalkDirectory (directory);
      foreach (var _directory in _directories)
        if (_directory.Split ('/').Last ().ToLower () == name.ToLower ())
          return _directory;
      Debug.LogWarning ("There is no directory named '" + name + "', creating in project root.");
      return directory;
    }

    private List<string> WalkDirectory (string directory) {
      var _results = new List<string> ();
      var _directories = System.IO.Directory.GetDirectories (directory);
      foreach (var _directory in _directories) {
        _results.Add (_directory);
        var _directoryDirectories = this.WalkDirectory (_directory);
        foreach (var _directoryDirectory in _directoryDirectories)
          _results.Add (_directoryDirectory);
      }
      return _results;
    }

    private void WriteContentToFile (string filePath, string fileName, string fileType, string[] fileContentLines) {
      using (var outfile = new StreamWriter (filePath + "/" + fileName + "." + fileType)) {
        var _fileContent = "";
        foreach (var _fileContentLine in fileContentLines)
          if (_fileContentLine != null)
            _fileContent += _fileContentLine + "\n";
        outfile.Write (_fileContent);
      }
      Debug.Log ("Creating '" + fileName + "' in '" + filePath + "'.");
      AssetDatabase.Refresh ();
    }
  }
}
#endif
