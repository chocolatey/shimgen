Shim Generator
==============

A readme and issues list for Shim Generator (shimgen). 

## What is Shim Generator (shimgen)?

Shimgen is a tool that makes batch redirection not suck so much by generating shims that point to target executable files.

* Provides an exe file that calls a target executable
* The exe can be called from powershell, bash, cmd.exe, or other shells just like you would call the target.
* Blocks and waits for command line apps to finish running, exits immediately when running a GUI app
* Uses the icon of the target if the target exists on creation

## Args

 * shimgen-help - shows this help menu and exits without running the target
 * shimgen-log - logging is shown on command line
 * shimgen-waitforexit - explicitly tell the shim to wait for target to exit - useful when something is calling a gui and wanting to block - command line programs explicitly have waitforexit already set.
 * shimgen-exit - explicitly tell the shim to exit immediately.
 * shimgen-gui - explicitly behave as if the target is a GUI application. This is helpful in situations where the package did not have a proper .gui file.
 * shimgen-usetargetworkingdirectory - set the working directory to the target path. Useful when programs need to be running from where they are located (usually indicates programs that have issues being run globally).
 * shimgen-noop - Do not actually call the target. Useful to see what would happen if you ran the command.

### Licence

Shimgen is a commercial tool provided by RealDimensions Software, LLC (RDS) for free use with the Chocolatey client. It has a commercial license for other applications, but may also grant a free license for FOSS projects. See the FAQs.

### FAQs

##### I want to use Shimgen outside of Chocolatey.
If your project is FOSS, please contact us for a grant of a free license to do so. If your project is commercial, please contact RDS for a quote.

##### Why is ShimGen Closed Source?

Honestly RDS gives much away for free, and many things are also FOSS, but at the end of the day some things need to be commercially supported. If one day there is a hope to support folks working full time on the Chocolatey ecosystem, then not everything can be FOSS.

##### Will Shimgen ever be open-sourced? 
It's possible. Eventually the source code may be opened up as a library and/or rolled into Chocolatey itself. It really depends on how well other things regarding RDS and the Chocolatey ecosystem pan out.

##### The Chocolatey client isn't really FOSS if Shimgen isn't FOSS.
The argument can be made that nothing on Windows is truly FOSS because it has to run on a system that isn't also open. It also uses PowerShell, which is free but I've never seen the source code for.

##### I won't support Chocolatey if Shimgen isn't FOSS.
We can't make you use Chocolatey. You also probably shouldn't use it because it takes advantage of PowerShell and Windows, which also isn't FOSS, among other tools it makes use of that are not open sourced (e.g. WebPi and DISM).
