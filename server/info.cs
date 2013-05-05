//================================
// Blockland Glass - Server - Info
//================================

if(!isObject(BLG_S_INF)) {
	%so = new ScriptObject(BLG_S_INF) {
		tickData = 0;
	};

	BLG_Modules.add(%so);
}

function BLG_S_INF::unload(%this) {
	%this.tcp.delete();
}

//================================
// Information Server
//================================

function BLG_S_INF::init(%this) {
	%this.tick();
	%tcp = new TCPObject(BLG_S_INF_TCP);
	%this.tcp = %tcp;

	if((%port = BLG_Settings.value("s_info", "port")) $= "0") {
		%tcp.listen($Server::Port); //The port is automatically opened but unused
		%this.port = $Server::Port;
	} else {
		%tcp.listen(%port);
		%this.port = %port;
	}
}

function BLG_S_INF_TCP::onConnectRequest(%this, %addr, %id) {
	echo("Connect Request");
	%client = new TCPObject(BLG_S_INF_Client, %id) {
		addr = %addr;
		id = %id;
	};
	%client.onConnected();
}

function BLG_S_INF_Client::onLine(%this, %line) {
	%arg0 = getField(%line, 0);
	%arg1 = getField(%line, 1);
	%arg2 = getField(%line, 2);
	%arg3 = getField(%line, 3);

	if(%this.protocol) {
		//Protocol
		switch$(getWord(%line, 0)) {
			case "GET":
				if(getWord(%line, 1) $= "DATA") {
					%this.call = "data";
					%this.version = getWord(strReplace(getWord(%line, 2), "/", " "), 1);
				}

			case "BLID:":
				%this.blid = getWord(%line, 1);
		}
		if(%line $= "") {
			if(%this.version !$= "" && %this.blid !$= "") {
				%this.protocol = false;

				if(%this.call $= "data") {
					echo("Sending data load...");
					%this.sendDataLoad();
					%this.disconnect();
					%this.schedule(0, delete);
				}
			}
		}
	} else {
		if(%this.streaming) {
			//Stream Processing
		} else {

		}
	}

}

function BLG_S_INF_Client::onConnected(%this) {
	%this.streaming = false;
	%this.protocol = true;
}

function BLG_S_INF_Client::sendDataLoad(%this) {
	%this.send("STATUS: BLG OK\r\n");
	%this.send("VERSION: " @ $BLG::Version @ "\r\n");
	%this.send("BLID: " @ getNumKeyId() @ "\r\n");
	%this.send("\r\n");
	%this.send("desc\tLorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n");

	%this.send("general\t" @ getBrickCount() TAB getSimTime() TAB BLG_S_INF.general["joins"] TAB BLG_S_INF.general["unique"] @ "\r\n");

	%td = "tickdata";
	for(%i = BLG_S_INF.tickData; %i > 0; %i--) {
		%td = %td TAB BLG_S_INF.tickData[%i-1];
	}
	%this.send(%td @ "\r\n");

	for(%i = 0; %i < BLG_S_GUI.guis; %i++) {
		%gui =  BLG_S_GUI.gui[%i];
		%this.send("gui\t" @ %gui @ "\r\n");
	}

	%pd = "players";
	for(%i = 0; %i < ClientGroup.getCount(); %i++) {
		%c = ClientGroup.getObject(%i);
		%title = "-";
		if(%c.isAdmin) {
			%title = "A";
		}
		if(%c.isSuperAdmin) {
			%title = "S";
		}
		if(%c.isHost || %c.bl_id == getNumKeyId()) {
			%title = "H";
		}
		%pd = %pd TAB %title TAB %c.name TAB %c.bl_id;
	}
	%this.send(%pd @ "\r\n");
}

//================================
// Data Collection
//================================

function BLG_S_INF::tick(%this) {
	echo("BLG Stats tick");
	cancel(%this.tick);

	%this.tickData[%this.tickData] = getBrickCount() SPC ClientGroup.getCount();
	%this.tickData++;

	%this.tick = %this.schedule(300000, tick);
}

//================================
// Packaging
//================================

package BLG_S_INF {
	function GameConnection::autoAdminCheck(%client) {
		BLG_S_INF.general["joins"]++;
		if(!BLG_S_INF.joined[%client.bl_id]) {
			BLG_S_INF.joined[%client.bl_id] = true;
			BLG_S_INF.general["unique"]++;
		}
		parent::autoAdminCheck(%client);
	}
};
activatePackage(BLG_S_INF);