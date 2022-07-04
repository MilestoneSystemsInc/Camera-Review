Words from the author

Author : Justin Butterworth - jub@milestone.dk

This plugin is provided as is and with minimal documentation and testing. The plugin is designed to be installed in the management client, it loads 2 documents into memory
1, the hardware list from the milestone supported list 
https://www.milestonesys.com/community/business-partner-tools/supported-devices/xprotect-corporate-and-xprotect-expert/
then export the csv once loaded.
2, a list of passwords to test the setup against, I've inclued a list of 10,000,000 passwords from 
https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/10-million-password-list-top-1000000.txt
please include the header "passwordlist" to the top of the list to correctly load the list.

Installation (installed on the management Client)

Create a new directory under C:\Program Files\Milestone\MIPPlugins for example cameraReview.

Paste the plugin.def and the accompanying dlls into that folder. Start the management Client

Uninstall

Incase of issues please stop the mangement client enter the folder you created in C:\Program Files\Milestone\MIPPlugins
and rename the plugin.def to plugin.def.disabled to stop the plugin from being loaded.

Instructions
This plugin checks the firmware of the device against the latest supported firmware by Milestone. To do this is takes input of a csv downloaded from the supported hardware page.
It also takes a list of passwords to check the current devices password against to see how "guessable" it might be.
once you load both the lists then you can select run tests, to assess each device.
Along with the above 2 comparative tests (for the firmware we are limited to assess firmware with the "standard" x.y.z format for comparison). We also check the complexity of the password to the following list
No password set = Blank
Password of less the 4 characters = Very Weak
We then assess any of the below be true increasing the security level by one per item it finds, going weak, Medium, Strong and VeryStrong.
*Mixed upper and lower case
*password lencth of 9 or more
*Password length of 12 or more
*Inclusion of special characters
*One or more numeric characters

You can filter the list using the listbox and combo bot to set the minimal password complexity and the tests to filter on. 
You can then export the resultant list into csv to process later.


Logging

There is performance logging in the MIPTrace.log, usually located in C:\ProgramData\Milestone\XProtect Management Client\Logs

Troubleshooting

You may find the the dlls are blocked by windows, in this case, right click on the dll, select properties and uncheck the "blocked" check box.
