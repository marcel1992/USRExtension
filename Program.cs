﻿using System;
using EnvDTE80;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    class Program
    {
        /*
         * STAThreadAttribute indicates that the COM threading model for the application is single-threaded apartment. 
         * This attribute must be present on the entry point of any application that uses Windows Forms; if it is omitted, the Windows components might not work correctly. 
         * If the attribute is not present, the application uses the multithreaded apartment model, which is not supported for Windows Forms.
         * 
         */
        [STAThread]
        static void Main(string[] args)
        {
            DTE2 dte;
            object obj = null;
            Type t = null;

            // Get the ProgID for DTE 8.0.
            t = Type.GetTypeFromProgID("VisualStudio.DTE.15.0",true);
            // Create a new instance of the IDE.
            obj = Activator.CreateInstance(t, true);
            // Cast the instance to DTE2 and assign to variable dte.
            dte = (DTE2)obj;

            // Register the IOleMessageFilter to handle any threading 
            // errors.
            MessageFilter.Register();
            // Display the Visual Studio IDE.
            dte.MainWindow.Activate();

            // =====================================
            // ==Insert your automation code here.==
            // =====================================
            // For example, get a reference to the solution2 object
            // and do what you like with it.

            var updateServiceReference = dte.Commands.Item("Project.UPDATESERVICEREFERENCE");
            if(updateServiceReference != null)
            {
                updateServiceReference.DTE.Events.CommandEvents.BeforeExecute += CommandEvents_BeforeExecute;
            }

            // Solution2 soln = (Solution2)dte.Solution;
            // System.Windows.Forms.MessageBox.Show("Solution count: " + soln.Count);
            // =====================================

            // All done, so shut down the IDE...
            // dte.Quit();
            // and turn off the IOleMessageFilter.
            // MessageFilter.Revoke();
            Console.Read();

        }

        private static void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            if(Guid == "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}" || ID == 1131)//update service references
            {

            }
        }
    }

    public class MessageFilter : IOleMessageFilter
    {
        //
        // Class containing the IOleMessageFilter
        // thread error-handling functions.

        // Start the filter.
        public static void Register()
        {
            IOleMessageFilter newFilter = new MessageFilter();
            IOleMessageFilter oldFilter = null;
            int hr = CoRegisterMessageFilter(newFilter, out oldFilter);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
        }

        // Done with the filter, close it.
        public static void Revoke()
        {
            IOleMessageFilter oldFilter = null;
            CoRegisterMessageFilter(null, out oldFilter);
        }

        //
        // IOleMessageFilter functions.
        // Handle incoming thread requests.
        int IOleMessageFilter.HandleInComingCall(int dwCallType,
          System.IntPtr hTaskCaller, int dwTickCount, System.IntPtr
          lpInterfaceInfo)
        {
            //Return the flag SERVERCALL_ISHANDLED.
            return 0;
        }

        // Thread call was rejected, so try again.
        int IOleMessageFilter.RetryRejectedCall(System.IntPtr
          hTaskCallee, int dwTickCount, int dwRejectType)
        {
            if (dwRejectType == 2)
            // flag = SERVERCALL_RETRYLATER.
            {
                // Retry the thread call immediately if return >=0 & 
                // <100.
                return 99;
            }
            // Too busy; cancel call.
            return -1;
        }

        int IOleMessageFilter.MessagePending(System.IntPtr hTaskCallee,
          int dwTickCount, int dwPendingType)
        {
            //Return the flag PENDINGMSG_WAITDEFPROCESS.
            return 2;
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int
          CoRegisterMessageFilter(IOleMessageFilter newFilter, out
          IOleMessageFilter oldFilter);
    }

    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleMessageFilter
    {
        [PreserveSig]
        int HandleInComingCall(
            int dwCallType,
            IntPtr hTaskCaller,
            int dwTickCount,
            IntPtr lpInterfaceInfo);

        [PreserveSig]
        int RetryRejectedCall(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwRejectType);

        [PreserveSig]
        int MessagePending(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwPendingType);
    }


}
