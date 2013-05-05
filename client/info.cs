//================================
// Blockland Glass - Client - Info
//================================
$remapDivision[$remapCount] = "Blockland Glass";
$remapName[$remapCount] = "Open Server Settings";
$remapCmd[$remapCount] = "BLGSettings";
$remapCount++;

function BLGSettings() {
    commandtoserver('BLGSettings');
}

//================================
// BLG_C_INF
//================================

if(!isObject(BLG_C_INF)) {
	%so = new ScriptObject(BLG_C_INF) {
		tab = 1;
	};
	BLG_Modules.add(%so);
}

function BLG_C_INF::init(%this) {
	BLG_Connection.setHandle("server", "BLG_C_INF.onLine");
	BLG_Connection.setHandle("rating", "BLG_C_INF.onLine");
}

function BLG_C_INF::onLine(%this, %line) {
	if(getField(%line, 0) $= "server") {
		if(getField(%line, 1) $= "list") {
			%this.servers = "";
			for(%i = 2; %i < getFieldCount(%line); %i++) {
				%server = getField(%line, %i);
				%addrs  = strReplace(%server, "-", "\t");
				echo("BLG_C_INF.server[" @ getField(%addrs, 0) @ "]");
				%this.server[getField(%addrs, 0)] = getField(%addrs, 1);
			}
			%this.servers = trim(%servers);
		} else if(getField(%line, 1) $= "following") {
			BLG_C_SER.onLine(%line);
		}
	} else {
		if(getField(%line, 1) $= "list") {
			BLG_C_INF.setRating(getField(%line, 2));
			for(%i = 3; %i < getFieldCount(%line); %i += 3) {
				%this.comment[%this.comments++] = getField(%line, %i) TAB getField(%line, %i+1) TAB getField(%line, %i+2);
			}
		}
	}
}

function BLG_C_INF::getInfo(%this, %addr, %gui) {
	if(%addr $= "") {
		return; //Why the fuck does this work?
	}

	for(%i = 0; %i < 20; %i++) {
		%this.graphData[1, %i] = "";
		%this.graphData[2, %i] = "";
	}

	%tcp = new TCPObject(BLG_C_INF_TCP) {
		gui = %gui;
	};
	echo("Connecting to " @ %addr);
	//display loading layover
	%tcp.connect(%addr);
	BLG_C_INF.setCover("Connecting...");
}

//================================
// Information Collection
//================================

function BLG_C_INF_TCP::onConnected(%this) {
	echo("connected");
	BLG_C_GUI.guis = 0;
	BLG_C_GUI.mand = false;
	BLG_ServerInfo_PlayerList.clear();
	%this.send("GET DATA BLG/" @ $BLG::Version @ "\r\n");
	%this.send("BLID: " @ getNumKeyId() @ "\r\n");
	%this.send("\r\n");
}

function BLG_C_INF_TCP::onLine(%this, %line) {
	echo(%line);
	if(getWord(%line, 0) $= "STATUS:") {
		%this.greenlit = true;
	}

	if(getWord(%line, 0) $= "VERSION:") {
		%this.version = getWord(%line, 1);
		return;
	}

	if(getWord(%line, 0) $= "BLID:") {
		%this.blid = getWord(%line, 1)+0;
		BLG_Connection.placeCall("rating", "get\t" @ %this.blid);
	}

	if(%this.greenlit) {
		switch$(getField(%line, 0)) {
			case "gui":
				BLG_C_GUI.gui[BLG_C_GUI.guis] = getField(%line, 1) TAB getField(%line, 2) TAB getField(%line, 3);
				if(getField(%line, 3)) {
					BLG_C_GUI.mand = true;
				}
				BLG_C_GUI.guis++;

			case "desc":
				echo(%line);
				BLG_C_INF.description = getField(%line, 1);
				BLG_ServerInfo_Description.setText(BLG_C_INF.description);

			case "players":
				for(%i = 1; %i < getFieldCount(%line)-1; %i+=3) {
					BLG_ServerInfo_PlayerList.addRow(BLG_ServerInfo_PlayerList.rowCount(), getField(%line, %i) TAB getField(%line, %i+1) TAB getField(%line, %i+2));
				}

			case "tickdata":
				echo(%line);
				for(%i = 1; %i < getFieldCount(%line)-1; %i++) {
					BLG_C_INF.graphData[1, 20-%i] = getWord(getField(%line, %i), 0);
					BLG_C_INF.graphData[2, 20-%i] = getWord(getField(%line, %i), 1);
				}

			case "general":
				BLG_ServerInfo_GeneralStats.setText("<font:Arial Bold:14>Brick Count: <font:Arial:14>" @ getField(%line, 1) @
					"<br><font:Arial Bold:14>Uptime: <font:Arial:14>" @ mFloor(getField(%line, 2)/60000) @ " minutes" @
					"<br><font:Arial Bold:14>Total Joins: <font:Arial:14>" @ getField(%line, 3) @
					"<br><font:Arial Bold:14>Unique Joins: <font:Arial:14>" @ getField(%line, 4));
		}
	}
}

function BLG_C_INF_TCP::onDNSFailed(%this) {
	BLG_C_INF.setCover("DNS Failed");
}

function BLG_C_INF_TCP::onConnectFailed(%this) {
	BLG_C_INF.setCover("Connection Failed");
	BLG_C_INF.displayGraphData();
}

function BLG_C_INF_TCP::onDisconnect(%this) {
	//hide loading layover
	BLG_C_INF.setCover();
	//display data:
	BLG_C_INF.displayGraphData();
}

//================================
// GUI Functions
//================================

function BLG_C_INF::displayInfo(%this) {
	BLG_C_INF.setCover("Loading...");
	%addr = getField(JS_serverList.getValue(), 9);
	echo(%addr);
	if(BLG_C_INF.server[%addr]) {
		BLG_C_INF.getInfo(BLG_C_INF.server[%addr]);
	} else {
		BLG_C_INF.setCover("Data unavailable");
	}
	canvas.pushDialog(BLG_ServerInfo);
}

function BLG_C_INF::setGraphValue(%this, %table, %data, %value) {
	%y = (%value * 130)+5;
	%bar = ("BLG_ServerInfo_Graph_" @ %table @ "_" @ %data).getId();
	%bar.extent = 5 SPC mCeil(%y);
	%bar.position = getWord(%bar.position, 0) SPC 6+(135-mCeil(%y));
	%bar.table = %table;
	%bar.data = %data;
}

function BLG_C_INF::graphTest(%this) {
	for(%i = 0; %i < 20; %i++) {
		%this.graphData[1, %i] = getRandom(getRandom(0, 1000));
		%this.graphData[2, %i] = getRandom(getRandom(0, 32));
	}
}

function BLG_C_INF::displayGraphData(%this) {
	%max1 = 100;
	%max2 = 5;
	for(%i = 0; %i < 20; %i++) {
		%dat1 = %this.graphData[1, %i];
		if(%dat1 > %max1) {
			%max1 = %dat1;
		}


		%dat2 = %this.graphData[2, %i];
		if(%dat2 > %max2) {
			%max2 = %dat2;
		}
	}

	for(%i = 0; %i < 20; %i++) {
		%dat1 = %this.graphData[1, %i];
		%dat2 = %this.graphData[2, %i];

		%this.setGraphValue(1, %i+1, %dat1/%max1);
		%this.setGraphValue(2, %i+1, %dat2/%max2);
	}

	BLG_ServerInfo_GraphMax.setText(%max1 @ " - " @ %max2);
}

function BLG_C_INF::tab(%this, %tab) {
	("BLG_ServerInfo_Swatch" @ %this.tab).setVisible(false);
	("BLG_ServerInfo_Button" @ %this.tab).setBitmap("base/client/ui/tab1");
	%this.tab = %tab;
	("BLG_ServerInfo_Swatch" @ %tab).setVisible(true);
	("BLG_ServerInfo_Button" @ %tab).setBitmap("base/client/ui/tab1use");
}

function BLG_C_INF::setCover(%this, %text) {
	if(%text $= "") {
		BLG_ServerInfo_Cover1.setVisible(false);
		BLG_ServerInfo_Cover2.setVisible(false);
	} else {
		BLG_ServerInfo_Cover1.setVisible(true);
		BLG_ServerInfo_Cover2.setVisible(true);

		BLG_ServerInfo_CoverText1.setText(%text);
		BLG_ServerInfo_CoverText2.setText(%text);
	}
}

function BLG_C_INF::setRating(%this, %rate) {
	BLG_ServerInfo_Star1.setBitmap("Add-Ons/System_BlocklandGlass/client/image/star0");
	BLG_ServerInfo_Star2.setBitmap("Add-Ons/System_BlocklandGlass/client/image/star0");
	BLG_ServerInfo_Star3.setBitmap("Add-Ons/System_BlocklandGlass/client/image/star0");
	BLG_ServerInfo_Star4.setBitmap("Add-Ons/System_BlocklandGlass/client/image/star0");
	BLG_ServerInfo_Star5.setBitmap("Add-Ons/System_BlocklandGlass/client/image/star0");

	for(%i = 0; %i < mFloor(%rate); %i++) {
		("BLG_ServerInfo_Star" @ %i+1).setBitmap("Add-Ons/System_BlocklandGlass/client/image/star");
	}

	%dec = %rate - mFloor(%rate);
	if(%dec != 0) {
		if(%dec <= (1/3)) {
			("BLG_ServerInfo_Star" @ mCeil(%rate)).setBitmap("Add-Ons/System_BlocklandGlass/client/image/star1");
		} else if(%dec <= (2/3)) {
			("BLG_ServerInfo_Star" @ mCeil(%rate)).setBitmap("Add-Ons/System_BlocklandGlass/client/image/star2");
		} else {
			("BLG_ServerInfo_Star" @ mCeil(%rate)).setBitmap("Add-Ons/System_BlocklandGlass/client/image/star3");
		}
	}
}

//================================
// Packaging
//================================

package BLG_C_INF {
	function ServerInfoSO_Add(%a,%b,%c,%d,%e,%f,%g,%h,%i,%j) {
		Parent::ServerInfoSO_Add(%a,%b,%c,%d,%e,%f,%g,%h,%i,%j);
		
		%search = strReplace(strReplace(%a,":","X"),".","_");

		if(BLG_C_INF.server[%a] !$= "") {
			%soIndex = $ServerSOFromIP[%search];
			%so = $ServerSO[%soIndex];
			if(isObject(%so)) {
				%so.hasBLG = 1;
				%so.display();
			}
		}
	}
	
	function ServerSO::serialize(%this) {
		%serialized = Parent::serialize(%this);
		
		if(%this.hasBLG)
			%hasRTB = "Yes";
		else
			%hasRTB = "No";
		
		if(getFieldCount(%serialized) >= 12) {
			for(%i = 0; %i < 11; %i++) {
				%s = %s @ getField(%serialized, %i) @ "\t";
			}
		}
		return %s @ %hasRTB;
	}

	function BLG_ServerInfo_Mouse::onMouseMove(%this, %mod, %pos, %click) {
		if(%this.getName() $= "BLG_ServerInfo_Mouse") {
			%offset = (getWord(BLG_ServerInfo.getObject(0).position, 0) + getWord(BLG_ServerInfo_Swatch2.position, 0) + getWord(BLG_ServerInfo_Graph.position, 0))
				SPC (getWord(BLG_ServerInfo.getObject(0).position, 1) + getWord(BLG_ServerInfo_Swatch2.position, 1) + getWord(BLG_ServerInfo_Graph.position, 1));
			%x = getWord(%pos, 0)-getWord(%offset, 0);
			%y = getWord(%pos, 1)-getWord(%offset, 1);
			
			for(%i = 0; %i < BLG_ServerInfo_Graph.getCount(); %i++) {
				%obj = BLG_ServerInfo_Graph.getObject(%i);

				if(%obj == %this) continue;

				%posX = getWord(%obj.position, 0);
				%posY = getWord(%obj.position, 1);
				%extX = getWord(%obj.extent, 0);
				%extY = getWord(%obj.extent, 1);

				if(%x >= %posX && %x < %posX+%extX) {
					if(%y >= %posY && %y < %posY+%extY) {
						if(%obj.table == 1) {
							%title = " bricks";
						} else {
							%title = " players";
						}
						BLG_ServerInfo_GraphText.setText((BLG_C_INF.graphData[%obj.table, %obj.data-1] += 0) @ %title);

						if(%this.hoverObj != %obj) {
							if(isObject(%this.hoverObj)) {
								%this.hoverObj.color = %this.hoverObj.originalColor;
							}

							%this.hoverObj = %obj;
							%obj.originalColor = %obj.color;
							if(%obj.table == 1) {
								%obj.color = "150 150 255 255";
							} else {
								%obj.color = "255 150 150 255";
							}
						}
						return;
					}
				}
			}
			if(isObject(%this.hoverObj)) {
				%this.hoverObj.color = %this.hoverObj.originalColor;
				%this.hoverObj = "";
			}
			BLG_ServerInfo_GraphText.setText("");
		}//arent::onMouseMove(%this, %mod, %pos, %click);
	}

	function BLG_ServerInfo_RatingMouse::onMouseMove(%this, %mod, %pos, %click) {
		if(%this.getName() $= "BLG_ServerInfo_RatingMouse") {
			%offset = (getWord(BLG_ServerInfo.getObject(0).position, 0) + getWord(BLG_ServerInfo_Swatch1.position, 0) + getWord(%this.getGroup().position, 0))
				SPC (getWord(BLG_ServerInfo.getObject(0).position, 1) + getWord(BLG_ServerInfo_Swatch2.position, 1) + getWord(%this.getGroup().position, 1)) + 210;
			%x = getWord(%pos, 0)-getWord(%offset, 0);
			%y = getWord(%pos, 1)-getWord(%offset, 1);

			echo(%x/5);
		}
	}
};
activatePackage(BLG_C_INF);