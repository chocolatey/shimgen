Shim Generator
==============

A readme and issues list for Shim Generator (shimgen).

## What is Shim Generator (shimgen)?

Shimgen is a tool that makes batch redirection not suck so much by generating shims that point to target executable files.

* Provides an exe file that calls a target executable
* The exe can be called from powershell, bash, cmd.exe, or other shells just like you would call the target.
* Blocks and waits for command line apps to finish running, exits immediately when running a GUI app
* Uses the icon of the target if the target exists on creation
* Works better than symlinks. Symlinks on Windows fall down at file dependencies. So if your file depends on other files and DLLs, all of those need to also be linked.

## Shim Arguments

You pass these arguments to an executable that is a shim (e.g. executables in the bin directory of your choco install):

 * `--shimgen-help` - shows this help menu and exits without running the target
 * `--shimgen-log` - logging is shown on command line
 * `--shimgen-waitforexit` - explicitly tell the shim to wait for target to exit - useful when something is calling a gui and wanting to block - command line programs explicitly have waitforexit already set.
 * `--shimgen-exit` - explicitly tell the shim to exit immediately.
 * `--shimgen-gui` - explicitly behave as if the target is a GUI application. This is helpful in situations where the package did not have a proper .gui file.
 * `--shimgen-usetargetworkingdirectory` - set the working directory to the target path. Useful when programs need to be running from where they are located (usually indicates programs that have issues being run globally).
 * `--shimgen-noop` - Do not actually call the target. Useful to see what would happen if you ran the command.

### Licence
Shimgen is a commercial tool provided by Chocolatey Software for free use with the Chocolatey client. It has a commercial license for other applications, but may also grant a free license for FOSS projects. See the FAQs.

### FAQs
##### Is shimgen free for use?
Only in the context of using it with Chocolatey. It has a specific license granted to distribution with Chocolatey that is should be used only in the context of Chocolatey and not separately. 

##### I want to use shimgen outside of Chocolatey.
If your project is FOSS, please contact us for a grant of a free license to do so. If your project is commercial, please contact Chocolatey Software for a quote.

##### Why is shimGen closed source?
It's a long story...one that is best talked about over coffee or drinks. We have open sourced [the shims themselves](shim/) so you can see what the exe shims are made of.

##### Can I view the source of a created shim?
Yes, absolutely! See [Shim Source Readme](shim/).

##### The Chocolatey client isn't really FOSS if Shimgen isn't FOSS.
The argument can be made (and has been made) that nothing on Windows is truly FOSS because it has to run on a system that isn't also open. Chocolatey uses tools as part of what it does, Shimgen is one of those tools.

##### I won't support Chocolatey if Shimgen isn't FOSS.
That's fine - that decision rests with you and we can respect that.
