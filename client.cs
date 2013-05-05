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

exec("./Support_ModVersion.cs");

exec("./client/connection.cs");
exec("./client/info.cs");
exec("./client/gui.cs");
exec("./client/profiles.cs");
exec("./client/server.cs");

exec("./client/BLG_Guis.gui");
exec("./client/ProgressBarGui.gui");
exec("./client/BLG_ServerInfo.gui");
exec("./client/BLG_ServerSettings.gui");
exec("./client/BLG_ServerList.gui");

for(%i = 0; %i < BLG_Modules.getCount(); %i++) {
	BLG_Modules.getObject(%i).init();
}

activatePackage(BLG_C_GUI);
activatePackage(BLG_Connection);

exec("./client/joinServer.gui");
JoinServerGui.clear();
JoinServerGui.add(BLGJS_window);
BLGJS_window.setName("JS_window");

//================================
// Mod Version Implementation
//================================

$cArg[8, mFloor($cArgs[8])] = "BLG" TAB $BLG::Version;
$cArgs[8]++;