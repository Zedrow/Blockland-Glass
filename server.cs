//================================
// Blockland Glass
//================================

$BLG::Version = "2.b2";

//================================
// Debug
// --------
// Level 0 = None
// Level 1 = Basic
// Level 2 = Advanced
//================================

$BLG::DebugLevel = 2;

//================================
// Modules
//================================

new ScriptGroup(BLG_Modules);

exec("./common/settings.cs");

exec("./server/connection.cs");
exec("./server/info.cs");
exec("./server/gui.cs");

for(%i = 0; %i < BLG_Modules.getCount(); %i++) {
	BLG_Modules.getObject(%i).init();
}

//================================
// Mod Version Implementation
//================================

package BLG_Server {
	function GameConnection::onConnectRequest(%client, %ip, %lan, %net, %prefix, %suffix, %arg5, %rtb, %arg7, %arg8, %arg9, %arg10, %arg11, %arg12, %arg13, %arg14, %arg15)
	{
		for(%a = 0; %a < getLineCount(%arg[8]); %a++)
		{
			%line = getLine(%arg[8], %a);
			if(getField(%line, 0) $= "BLG")
			{
				%version = getField(%line, 1);
				if(mFloatLength(%version, 0) >= 2) {
					%client.hasBLG = true;
					%client.BLG = %version;
				}

				break;
			}
		}

		if(BLG_S_GUI.required && !%client.hasBLG) {
			%client.schedule(0, delete, "This server uses the Blockland Glass gui manager for mandatory GUIs.<br><br><a:blockland.zivle.com/download.php>Please download Blockland Glass.</a>");
			//return;
		}
		
		// Properly package and return the result of the parent.
		return parent::onConnectRequest(%client, %ip, %lan, %net, %prefix, %suffix, %arg5, %rtb, %arg7, %arg8, %arg9, %arg10, %arg11, %arg12, %arg13, %arg14, %arg15);
	}
};
activatePackage(BLG_Server);