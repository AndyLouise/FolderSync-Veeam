using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FolderSynchronizerVeeam
{
    [TestClass]
    public class FolderSynchronizerTests
    {
        private const string TestSourceFolder = "TestSource";
        private const string TestReplicaFolder = "TestReplica";
        private const string TestLogFilePath = "TestLog.txt";
        private const int TestSynchronizationInterval = 1; // seconds

        [TestMethod]
        public void TestSynchronization()
        {
            // Arrange
            PrepareTestFolders();

            string[] args = { TestSourceFolder, TestReplicaFolder, TestSynchronizationInterval.ToString(), TestLogFilePath };
            FolderSynchronizer.Initialize(args);

            // Act
            FolderSynchronizer.SynchronizeFolders();

            // Assert
            Assert.IsTrue(Directory.Exists(TestReplicaFolder));
            Assert.AreEqual(Directory.GetFiles(TestSourceFolder).Length, Directory.GetFiles(TestReplicaFolder).Length);
        }

        private void PrepareTestFolders()
        {
            // Create test source folder with some files
            Directory.CreateDirectory(TestSourceFolder);
            File.WriteAllText(Path.Combine(TestSourceFolder, "File1.txt"), "Test content 1");
            File.WriteAllText(Path.Combine(TestSourceFolder, "File2.txt"), "Test content 2");

            // Create test replica folder with a different file
            Directory.CreateDirectory(TestReplicaFolder);
            File.WriteAllText(Path.Combine(TestReplicaFolder, "File3.txt"), "Test content 3");
        }

        [TestMethod]
        public void TestRemoveExcessFiles()
        {
            // Arrange
            PrepareTestFolders();

            string[] args = { TestSourceFolder, TestReplicaFolder, TestSynchronizationInterval.ToString(), TestLogFilePath };
            FolderSynchronizer.Initialize(args);

            // Act
            FolderSynchronizer.RemoveExcessFiles();

            // Assert
            Assert.AreEqual(Directory.GetFiles(TestReplicaFolder).Length, Directory.GetFiles(TestSourceFolder).Length);
        }
    }
}
