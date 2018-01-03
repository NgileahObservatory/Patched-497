# Patched-497

## Introduction.
Patches are available for Meade #497 handboxes. These add some new capabilities to the Meade LX90 and Etx model telescopes. Stock ASCOM drivers know nothing of these systems so this driver attempts to make features available via ASCOM that otherwise would not be.

Additionally the driver has some features such as setting the guide rate and compensating for asymmetric time based movement of the scope particularly at sidereal and less rate. These are a deliberate fudge around deficiencies in the mount when all else has failed.

E.g. You have critically balanced the scope and equal time slews at sidereal E/W yield consistently different actual amounts of movement.

This driver is for use with patched Meade #497 HBX systems. Typically LX90 and ETX model telescopes. The patches have been developed and maintained by Richard Seymour and Andrew Johansen in particular.

To that end, <a href="https://groups.yahoo.com/neo/groups/lx90/info" target="browser">the LX90 Yahoo Group</a> is regularly frequented by both Richard (Dick) and Andrew.

## Prerequisites.

ASCOM Driver Development and prerequisites are found at <a href="http //ascom-standards.org/Developer/DriverImpl.htm" target="browser">ASCOM Driver Development</a>
Familiarise yourself with the prerequisites and installation of the framework. Install the ASCOM Driver Development prerequisites and the developer components and tools first.

Documentation specific to this driver is found in the provided Readme.htm and comments throughout the source code.

## Caution.

The code should generally be thought of as highly experimental. The only test systems available to the developer (me) are a circa 2007 12" LX90 LNT, and a similarly aged ETX 125.



