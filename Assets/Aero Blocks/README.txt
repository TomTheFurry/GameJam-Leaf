Welcome to Aero Blocks!

To get started, make sure you have either edit or join mode selected in the toolbar at the bottom of the scene view.
You can then begin creating aero blocks or hubs.

Aero Block
These are the main blocks in the tool, they have a 2D aerodynamic model attached to them.
This can produce some undesireable effects if you have them in your world with nothing else attached to them.
If you want to have them detatched and free you can fix two blocks together with one rotated (0,90,0).
See the demo scene for an example of this.

Hub
These are used to quickly create propellers or turbines etc.
To use the hub:
	first design your propeller blade using an aero block
	then drag the aero block component into the hub script
	input your desired number of blades and click the 'Update' button
To edit the blades you must make all changes to the first aero block child, the original. It won't have clone or (1).
If your hub explodes when you press play make sure the masses of your baldes and the hub part are similar.

Joining Aero Blocks
The join buttons that appear in join mode will join any aero blocks you have selected together.
The selection order does matter.
For example, if you select block A and then block B and click the join button 'R to L'
The tool will join block A's right edge to block B's left edge

Models
When you join aero blocks together, the tool will group them in a model.
The model's centre can then be set to the far left block's left edge by clicking the 'Make model zero' button.
This is to make positioning wings easier.

If you have any questions or problems please email me at marshcza@gmail.com.
I am aiming to have tutorial videos up on youtube in the near future.