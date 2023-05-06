using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace MiniDump
{
    class Program
    {
        [DllImport("Dbghelp.dll", SetLastError = true)]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, int ProcessId, SafeHandle hFile, int DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

        enum DumpType : int
        {
            MiniDumpNormal = 0,
            MiniDumpWithDataSegs = 1,
            MiniDumpWithFullMemory = 2,
            MiniDumpWithHandleData = 4,
            MiniDumpFilterMemory = 8,
            MiniDumpScanMemory = 16,
            MiniDumpWithUnloadedModules = 32,
            MiniDumpWithIndirectlyReferencedMemory = 64,
            MiniDumpFilterModulePaths = 128,
            MiniDumpWithProcessThreadData = 256,
            MiniDumpWithPrivateReadWriteMemory = 512,
            MiniDumpWithoutOptionalData = 1024,
            MiniDumpWithFullMemoryInfo = 2048,
            MiniDumpWithThreadInfo = 4096,
            MiniDumpWithCodeSegs = 8192,
            MiniDumpWithoutAuxiliaryState = 16384,
            MiniDumpWithFullAuxiliaryState = 32768,
            MiniDumpWithPrivateWriteCopyMemory = 65536,
            MiniDumpIgnoreInaccessibleMemory = 131072,
            MiniDumpValidTypeFlags = 262143
        }

        static void Main(string[] args)
        {
            // Check if the application is running as administrator
            if (!IsAdministrator())
            {
                Console.WriteLine("Please run this application as an administrator.");
                return;
            }

            // Prompt the user to enter a filter keyword for the process name
            Console.WriteLine("Enter a keyword or character combination to filter the process name (leave empty to list all processes):");
            string filter = Console.ReadLine().ToLower(CultureInfo.InvariantCulture);

            // Display filtered processes
            Console.WriteLine("Filtered Processes:");
            Process[] processList = Process.GetProcesses();
            var filteredProcesses = string.IsNullOrWhiteSpace(filter)
                ? processList
                : processList.Where(p => p.ProcessName.ToLower(CultureInfo.InvariantCulture).Contains(filter));

            if (filteredProcesses.Any())
            {
                // List filtered processes with their IDs and names
                foreach (Process process in filteredProcesses)
                {
                    Console.WriteLine($"ID: {process.Id}\tName: {process.ProcessName}");
                }

                // Prompt the user to enter the ID of the process they want to dump
                Console.WriteLine("\nEnter the ID of the process for which you want to create a memory dump:");
                int processId = Convert.ToInt32(Console.ReadLine());

                // Get the selected process
                Process selectedProcess = filteredProcesses.Where(k => k.Id == processId).FirstOrDefault();

                // Create a dump file for the selected process
                string dumpFilePath = $"{selectedProcess.ProcessName}_{DateTime.Now:yyyyMMdd_HHmmss}.dmp";
                using (FileStream dumpFile = File.Create(dumpFilePath))
                {
                    bool result = MiniDumpWriteDump(selectedProcess.Handle, selectedProcess.Id, dumpFile.SafeFileHandle, (int)DumpType.MiniDumpWithFullMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    if (result)
                    {
                        Console.WriteLine($"Memory dump successfully written to the '{dumpFilePath}' file.");
                    }
                    else
                    {
                        Console.WriteLine("Memory dump could not be created.");
                    }
                }
            }
        }

        // Helper function to check if the application is running as an administrator
        static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}

