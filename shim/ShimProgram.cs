/*
MICROSOFT REFERENCE SOURCE LICENSE (MS-RSL)

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions

The terms "reproduce," "reproduction" and "distribution" have the same meaning here as under U.S. copyright law.

"You" means the licensee of the software.

"Your company" means the company you worked for when you downloaded the software.

"Reference use" means use of the software within your company as a reference, in read only form, for the sole purposes of debugging your products, maintaining your products, or enhancing the interoperability of your products with the software, and specifically excludes the right to distribute the software outside of your company.

"Licensed patents" means any Licensor patent claims which read directly on the software as distributed by the Licensor under this license.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, the Licensor grants you a non-transferable, non-exclusive, worldwide, royalty-free copyright license to reproduce the software for reference use.

(B) Patent Grant- Subject to the terms of this license, the Licensor grants you a non-transferable, non-exclusive, worldwide, royalty-free patent license under licensed patents for reference use.

3. Limitations

(A) No Trademark License- This license does not grant you any rights to use the Licensor's name, logo, or trademarks.

(B) If you begin patent litigation against the Licensor over patents that you think may apply to the software (including a cross-claim or counterclaim in a lawsuit), your license to the software ends automatically.

(C) The software is licensed "as-is." You bear the risk of using it. The Licensor gives no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the Licensor excludes the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

 */

namespace shim
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class ShimProgram
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms681388(v=vs.85).aspx
        internal const uint ERROR_ELEVATION_REQUIRED = 740; // 740 (0x2E4)

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms681383(v=vs.85).aspx
        internal const uint ERROR_CANCELLED = 1223; // 1223 (0x4C7)

        private static void Main(string[] args)
        {
            if (args.Any(argument => argument.Contains("shimgen-log")) || args.Any(argument => argument.Contains("shimgen-noop")))
            {
                "shim".initialize_logger();
            }

            const string path = @"{{PathToExecutable}}";
            const string command = @"{{ExecutableCommand}}";
            const string is_gui_string = "{{IsGui}}";

            bool is_gui = is_gui_string.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            string this_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path_to_exe = Path.GetFullPath(Path.Combine(this_directory, path));
            string working_directory = Environment.CurrentDirectory;
            bool wait_for_exit = !is_gui;

            "shim".log(() => "Set up Shim to run with the following parameters:{0}  path to executable: {1}{0}  working directory: {2}{0}  is gui? {3}{0}  wait for exit? {4}{0}  command (optional): {5}".format_with(
                Environment.NewLine,
                path_to_exe,
                working_directory,
                is_gui,
                wait_for_exit,
                command
                ));

            // grab the bare arguments as they were passed to the command line
            var environment_commandLine = Environment.CommandLine;
            var original_commandline_length = environment_commandLine.Length;
            "shim".log(() => "Command line '{0}'".format_with(environment_commandLine));
            // remove the current executing application from them including spaces
            var current_process = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\""; //powershell
            "shim".log(() => "Current process '{0}'".format_with(current_process));
            environment_commandLine = environment_commandLine.Replace(current_process, string.Empty);
            "shim".log(() => "Command line after removing process '{0}'".format_with(environment_commandLine));

            var shift_arguments_off = 0;
            if (original_commandline_length == environment_commandLine.Length)
            {
                "shim".log(() => "Shifting off the first argument since process didn't catch it");
                //cmd.exe and the rest must shift off the first argument
                shift_arguments_off = 1;
            }

            string arguments = string.Join(" ", environment_commandLine.Split(new[] { " " }, StringSplitOptions.None).Skip(shift_arguments_off).Where(a => !a.Contains("-shimgen-"))).TrimStart();
            "shim".log(() => "Arguments after removing shimgen args - '{0}'".format_with(arguments));
            //string arguments = string.Join(" ", quote_arg_value_if_required(strip_shim_gen_args(args)));

            if (!string.IsNullOrWhiteSpace(command))
            {
                "shim".log(() => "Put command '{0}' at the front of the arguments".format_with(command));
                arguments = command + " " + arguments;
            }
            "shim".log(() => "Arguments are '{0}'".format_with(arguments));

            if (args.Any(argument => argument.Contains("shimgen-usetargetworkingdirectory")))
            {
                working_directory = Path.GetDirectoryName(path_to_exe);
                "shim".log(() => "Due to shimgen-usetargetworkingdirectory switch, working directory has been updated to '{0}'".format_with(working_directory));
            }

            if (args.Any(argument => argument.Contains("shimgen-gui")))
            {
                is_gui = true;
                wait_for_exit = false;
                "shim".log(() => "Due to shimgen-gui switch, we are calling target as a GUI app.");
            }

            if (args.Any(argument => argument.Contains("shimgen-waitforexit")))
            {
                wait_for_exit = true;
                "shim".log(() => "Due to shimgen-waitforexit switch, we are forcing the shim to wait for exit.");
            }

            if (args.Any(argument => argument.Contains("shimgen-exit")))
            {
                wait_for_exit = false;
                "shim".log(() => "Due to shimgen-exit switch, we are exiting the shim immediately.");
            }

            if (args.Any(argument => argument.Contains("shimgen-help")))
            {
                "shim".initialize_logger();

                "shim".log(() => @"
=========
Shim Help
=========

This is a shim, generated by Chocolatey's ShimGenerator (shimgen).
This shim calls a target with the following settings:
 Target: '{0}'
 Target exists: '{5}'
 Working directory: '{1}'
 GUI: '{2}'
 Wait for exit: '{3}'
 command (optional): '{4}'

==============
Shim Arguments
==============

You can pass the following arguments to a shim:

 * shimgen-help - shows this help menu and exits without running the
   target
 * shimgen-log - logging is shown on command line
 * shimgen-waitforexit - explicitly tell the shim to wait for target to
   exit - useful when something is calling a gui and wanting to block
   - command line programs explicitly have waitforexit already set
 * shimgen-exit - explicitly tell the shim to exit immediately.
 * shimgen-gui - explicitly behave as if the target is a GUI
   application. This is helpful in situations where the package did not
   have a proper .gui file.
 * shimgen-usetargetworkingdirectory - set the working directory to the
   target path. Useful when programs need to be running from where they
   are located (usually indicates programs that have issues being run
   globally)
 * shimgen-noop - Do not actually call the target. Useful to see what
   would happen if you ran the command.
".format_with(
                path_to_exe,
                working_directory,
                is_gui,
                wait_for_exit,
                command,
                File.Exists(path_to_exe)
                ));

            }

            if (args.Any(argument => argument.Contains("shimgen-help")) || args.Any(argument => argument.Contains("shimgen-noop")))
            {
                Environment.Exit(-1);
            }

            if (!File.Exists(path_to_exe))
            {
                Console.WriteLine("Cannot find file at '" + path + "' (" + path_to_exe + "). This usually indicates a missing or moved file.");
                Environment.Exit(-1);
            }

            SetHandler();
            try
            {
                Environment.ExitCode = CommandExecutor.execute(path_to_exe, arguments, working_directory, is_gui, wait_for_exit: wait_for_exit, requires_elevation: false);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.NativeErrorCode == ERROR_ELEVATION_REQUIRED)
                {
                    "shim".log(() => "Error starting process: Looks like we need elevation. Let's give that a shot");
                    try
                    {
                        Environment.ExitCode = CommandExecutor.execute(path_to_exe, arguments, working_directory, is_gui, wait_for_exit: wait_for_exit, requires_elevation: true);
                    }
                    catch (System.ComponentModel.Win32Exception exElevation)
                    {
                        if (exElevation.NativeErrorCode == ERROR_CANCELLED)
                        {
                            "shim".log(() => "Caught signal: User cancelled elevation");
                            //swallow - user cancelled
                        }
                        else
                        {
                            "shim".log(() => "Received error: '{0}'.".format_with(exElevation.ToString()));
                            throw;
                        }
                    }
                }
                else
                {
                    "shim".log(() => "Received error: '{0}'.".format_with(ex.ToString()));
                    throw;
                }
            }

            "shim".log(() => "Exiting with '{0}'.".format_with(Environment.ExitCode));

#if DEBUG
            Console.WriteLine("Exiting with {0}", Environment.ExitCode);
#endif
        }

        private static IEnumerable<string> strip_shim_gen_args(IEnumerable<string> arguments)
        {
            "shim".log(() => "Removing any shimgen specific args");
            return arguments.Where(argument => !argument.Contains("shimgen-")).ToList();
        }

        private static IEnumerable<string> quote_arg_value_if_required(IEnumerable<string> arguments)
        {
            "shim".log(() => "Quoting args with spaces");
            IList<string> returnArgs = new List<string>();

            foreach (var argument in arguments)
            {
                if (argument.Contains(" "))
                {
                    returnArgs.Add("\"" + argument + "\"");
                    "shim".log(() => "Quoted '{0}'".format_with(argument));
                }
                else
                {
                    returnArgs.Add(argument);
                }
            }

            return returnArgs;
        }

        #region SigTerm
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(SignalControlType sig);

        private static EventHandler _handler;

        private enum SignalControlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        public static void SetHandler()
        {
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

        }

        private static bool Handler(SignalControlType signal)
        {
            "shim".log(() => "Caught signal. Stopping running process");
            var process = CommandExecutor.RunningProcess;

            //sigint before kill - https://gist.github.com/jvshahid/6fb2f91fa7fb1db23599
            // http://stackoverflow.com/a/15281070/18475
            // http://stanislavs.org/stopping-command-line-applications-programatically-with-ctrl-c-events-from-net/

            var exitCode = -1;
            try
            {
                //if (process != null && !process.HasExited) process.CloseMainWindow();
                if (process != null && !process.HasExited) process.Kill();
                exitCode = process != null && process.HasExited ? process.ExitCode : exitCode;
            }
            catch
            {
                //move on
            }

            Environment.Exit(exitCode);

            return true;
        }
        #endregion
    }

    public static class StringExtensions
    {
        private static bool _allowLogging = false;

        public static void initialize_logger(this string input)
        {
            _allowLogging = true;
        }

        public static void log(this string input, Func<string> message)
        {
            if (_allowLogging)
            {
                Console.WriteLine("[" + input + "]: " + message());
            }
        }

        public static string format_with(this string input, params object[] formatting)
        {
            return string.Format(input, formatting);
        }
    }
}
