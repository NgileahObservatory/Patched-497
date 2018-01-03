# Patched-497

## Introduction.
Patches are available for Meade #497 handboxes. These add some new capabilities to the Meade LX90 and Etx model telescopes. Stock ASCOM drivers know nothing of these systems so this driver attempts to make features available via ASCOM that otherwise would not be.

This driver is for use with patched Meade #497 HBX systems. Typically LX90 and ETX model telescopes. The patches have been developed and maintained by Richard Seymour and Andrew Johansen in particular.

To that end, <a href="https://groups.yahoo.com/neo/groups/lx90/info" target="browser">the LX90 Yahoo Group</a> is regularly frequented by both Richard (Dick) and Andrew.

## The reason for another ASCOM driver.

Let's go back through some of the issues we were attempting to address with my Meade LX90 LNT circa 2007.
 1. Horrendous periodic error.
 2. With such large periodic error, large movements were necessary to correct it and this required the use of ± 1 × sidereal rate slews to correct instead of the finer ± 0.5 × sidereal rate pulse guiding.
 3. This came with the additional feature of mismatched E and W slews for any given slew time. This was partially caused, at least I believe so, by an E-ward drift overall. This has been bourne out by the final PEC achieved showing lopsided sine wave.
 4. Pulse Guiding appeared to not work with the stock Meade Classic/#497 ASCOM driver.
 
All of this made the results using PHD/2 Guiding and PEM Pro (v2 now using v3) less than ideal. PHD/2 would inevitably overshoot or undershoot due to the E/W asymmetry and PEM Pro relies on pulse guiding to accurately program mounts that it cannot directly write PEC data to. Worse than that, the attempted movement commands by PEM Pro would back up the FIFO on the #497 handbox such that the PEM Pro PEC step (once every 3.72s) would fall out of step with the Meade #497 which was struggling to clear its FIFO.

This all led to the decision to write an ASCOM driver with these deficiencies in mind and to address asymmetric movement at sidereal rate.

Having made this decision and having applied various SW patches to my scope's #497, it seemed sensible to take advantage of any patches I have applied that might benefit other owners of scopes like mine with the patched ROM

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
 7. Any single axis pulse guiding with any slew is disallowed.
 8. Any dual axis slew with any pulse guiding is disallowed.
 9. Any single axis Move...SlewingState with any pulse guide or dual axis slew is disallowed.
 10. Any slew can be interrupted by AbortSlew. So changing from one thing to another generally requires the issue of AbortSlew then the change to the desired movement.

## A note about :RA# and :RE# commands.

These Meade commands are very fickle as near as I can tell. All manner of tides, the position of the planets, a disturbance in the Earth's magnetic field seem to be able to prevent these
from working correctly. I have been unable to find the correct combination to make these commands work consistently ALL the time.
