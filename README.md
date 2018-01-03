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

## ASCOM Telescope Interface.

The ASCOM.LX90.Telescope driver exposes the <a href="http://www.ascom-standards.org/Help/Platform/html/T_ASCOM_DeviceInterface_ITelescopeV3.htm" target="browser">ASCOM ITelescopeV3 interface.</a>

## Software Architecture.

The driver is implemented as a GoF State Pattern. State transition diagrams are provided in the docs/ directory.

In summary, there is a single thread managing a thread safe queue of tasks. The tasks are anything that talks to the mount. Tasks are queued and the queue is serviced. The GoF State pattern
ensures that only tasks that are legal for the current state are performed. All other tasks are no-ops.

The telescope axes are managed by independent states. When slewing to a location (Alt/Az or RA/Dec) the axes are set to a single "DualAxisSlewingState" subclass. For individual axis movements
such as pulse guiding or moving the axis by an ASCOM.Telescope MoveAxis call, each axis has its own independent state.

Correct combinations of states are enforced across axes. 

E.g. 

 1. PrimaryAxis in TrackingState and SecondaryAxis in Tracking State.
 2. Either axis in TrackingState and the other axis in PulseGuiding...State.
 3. Either axis in TrackingState and the other axis in Move...SlewingState.
 4. Both axes in pulse guiding state.
 5. Both axes in Move...SlewingState.
 6. DualAxisSlewingState (SlewingToAltAzState or SlewingToCoordinatesState).

