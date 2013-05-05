RTBCC_NotificationManager.push("Server Online", "Jincux is now hosting", "server", "", 5000);



//================================
// Blockland Glass - Server
//================================

if(!isObject(BLG_C_SER)) {
	%so = new ScriptObject(BLG_C_SER) {
		categories = 0;
	};
	BLG_Modules.add(%so);
}

function BLG_C_SER::init(%this) {
	RTB_Overlay.add(BLG_ServerList.getObject(0));

	%this.addCategory("Following", "world");
	%this.addCategory("Your Servers", "user");

	BLG_C_SER.addServer("Brian Smith's I WILL RAPE YOU", "Following", "bullshit.ip.address", true, false, false);
	BLG_C_SER.addServer("Pecon7's Boss Battles", "Following", "68.168.212.106:30400", false, true, false);
	BLG_C_SER.addServer("Jincux's Development Server", "Your Servers", "142.196.39.2:28000", true, true, true);

	BLG_C_SER.draw();
}

function BLG_C_SER::onLine(%this, %line) {
	switch$(getWord(%line, 2)) {
		case "set":
			%this.serverData = getField(%line, 3);
			%this.pullServerList();

		case "add":
			BLG_C_SER.addServer(getField(%line, 3));
			%f = strReplace(getField(%line, 3), "-", "\t");
			%this.oldServerData = %this.serverData;
			%this.serverData = strReplace(%this.serverData, getField(%line, 3), "");
			%this.serverData = %this.serverData SPC getField(%line, 3);
			%this.pullServerList();

		case "remove":
			%this.serverData = strReplace(%this.serverData, getField(%line, 3), "");
			%server = %this.serverlist_serverByIp[getField(%line, 3)];
			RTBCC_NotificationManager.push("Server Offline", getField(%server, 4), "server_database", "", 5000);
			BLG_C_SER_TCP.onDisconnect();
	}
}

function BLG_C_SER::addCategory(%this, %title, %icon) {
	%this.category[%this.categories] = %title TAB %icon;
	%this.categories++;
	%this.servers[%title] = 0;
}

function BLG_C_SER::addServer(%this, %text, %cat, %ip, %locked, %dedicated, %canedit) {
	%icons = 0;
	if(%canedit) {
		%icon[%icons++] = "cog";
	}
	if(%dedicated) {
		%icon[%icons++] = "server";
	}
	if(%locked) {
		%icon[%icons++] = "lock";
	}

	%this.server[%cat, %this.servers[%cat]] = %text TAB %ip TAB %icon1 TAB %icon2 TAB %icon3;
	%this.servers[%cat]++;
}
function BLG_C_SER::draw(%this) {
	BLG_ServerList_Content.clear();
	%y = 5;
	for(%i = 0; %i < %this.categories; %i++) {
		%gui = new GuiBitmapCtrl() {
			profile = "BLG_TextProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "5" SPC %y;
			extent = "200 22";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			bitmap = "./image/serverList/bar1";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			keepCached = "0";
			mColor = "255 255 255 255";
			mMultiply = "0";

			new GuiTextCtrl() {
				profile = "BLG_TextProfile2";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "27 2";
				extent = "200 18";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				text = getField(%this.category[%i], 0);
				maxLength = "255";
				blgScrollingText = true;
			};
			new GuiBitmapCtrl() {
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "5 3";
				extent = "16 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = "Add-Ons/System_BlocklandGlass/client/image/icons/" @ getField(%this.category[%i], 1);
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = "255 255 255 255";
				mMultiply = "0";
			};
		};
		BLG_ServerList_Content.add(%gui);
		%y += 27;
		%this.obj[%obj++] = %gui;

		for(%j = 0; %j < %this.servers[getField(%this.category[%i], 0)]; %j++) {
			%server = %this.server[getField(%this.category[%i], 0), %j];
			%gui = new GuiBitmapCtrl() {
				blgdata = %server;
				profile = "BLG_TextProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "20" SPC %y;
				extent = "180 22";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = "./image/serverList/bar2";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = "255 255 255 255";
				mMultiply = "0";

				new GuiTextCtrl() {
					profile = "BLG_TextProfile2";
					horizSizing = "right";
					vertSizing = "center";
					position = "5 1";
					extent = "139 18";
					minExtent = "8 2";
					enabled = "1";
					visible = "1";
					clipToParent = "1";
					text = getField(%server, 0);
					maxLength = "255";
					blgScrollingText = true;
				};
			};

			%end = 180;
			if(getField(%server, 2) !$= "") {
				%icon = new GuiBitmapCtrl() {
					profile = "GuiDefaultProfile";
					horizSizing = "right";
					vertSizing = "center";
					position = "160 3";
					extent = "16 16";
					minExtent = "8 2";
					enabled = "1";
					visible = "1";
					clipToParent = "1";
					bitmap = "./image/icons/" @ getField(%server, 2);
					wrap = "0";
					lockAspectRatio = "0";
					alignLeft = "0";
					alignTop = "0";
					overflowImage = "0";
					keepCached = "0";
					mColor = "255 255 255 255";
					mMultiply = "0";
				};
				%gui.add(%icon);
				%end = 160;
			}
			if(getField(%server, 3) !$= "") {
				%icon = new GuiBitmapCtrl() {
					profile = "GuiDefaultProfile";
					horizSizing = "right";
					vertSizing = "center";
					position = "142 3";
					extent = "16 16";
					minExtent = "8 2";
					enabled = "1";
					visible = "1";
					clipToParent = "1";
					bitmap = "./image/icons/" @ getField(%server, 3);
					wrap = "0";
					lockAspectRatio = "0";
					alignLeft = "0";
					alignTop = "0";
					overflowImage = "0";
					keepCached = "0";
					mColor = "255 255 255 255";
					mMultiply = "0";
				};
				%gui.add(%icon);
				%end = 142;
			}
			if(getField(%server, 4) !$= "") {
				%icon = new GuiBitmapCtrl() {
					profile = "GuiDefaultProfile";
					horizSizing = "right";
					vertSizing = "center";
					position = "124 3";
					extent = "16 16";
					minExtent = "8 2";
					enabled = "1";
					visible = "1";
					clipToParent = "1";
					bitmap = "./image/icons/" @ getField(%server, 4);
					wrap = "0";
					lockAspectRatio = "0";
					alignLeft = "0";
					alignTop = "0";
					overflowImage = "0";
					keepCached = "0";
					mColor = "255 255 255 255";
					mMultiply = "0";
				};
				%gui.add(%icon);
				%end = 124;
			}
			BLG_ServerList_Content.add(%gui);
			%gui.getObject(0).extent = %end-10 SPC "18";
			%y += 27;
			%this.obj[%obj++] = %gui;
		}
	}
	BLG_ServerList_Content.extent = "220" SPC %y;
	BLG_ServerList_Mouse.extent = "220" SPC %y;
}


function BLG_C_SER::init(%this) {
	RTB_Overlay.add(BLG_ServerList.getObject(0));

	%this.addCategory("Following", "world");
	%this.addCategory("Your Servers", "user");

	BLG_C_SER.addServer("Brian Smith's I WILL RAPE YOU", "Following", "bullshit.ip.address", true, false, false);
	BLG_C_SER.addServer("Pecon7's Boss Battles", "Following", "68.168.212.106:30400", false, true, false);
	BLG_C_SER.addServer("Jincux's Development Server", "Your Servers", "142.196.39.2:28000", true, true, true);

	BLG_C_SER.draw();
}

//================================
// Server List
//================================

function BLG_C_SER::pullServerList(%this) {
	%this.slTCP = new TCPObject(BLG_C_SER_TCP);
	%this.slTCP.connect("master2.blockland.us:80");
}

function BLG_C_SER_TCP::onConnected(%this) {
	BLG_C_INF.serverlist_servers = 0;
	%this.send("GET / HTTP/1.1\nHost: master2.blockland.us\n\n");
}

function BLG_C_SER_TCP::onLine(%this, %line) {
	if(%line $= "END") {
		%this.buffer = false;
	}

	if(%this.buffer) {
		BLG_C_INF.serverlist_server[BLG_C_INF.serverlist_servers++] = %line; 
		BLG_C_INF.serverlist_serverByIp[getField(%line, 0) @ ":" @ getField(%line, 1)] = %line;
	}

	if(%line $= "START") {
		%this.buffer = true;
	}
}

function BLG_C_SER_TCP::onDisconnect(%this) {
	for(%i = 0; %i < BLG_C_SER.categories; %i++) {
		BLG_C_SER.servers[getField(BLG_C_SER.category[%i], 0)] = 0;
	}

	for(%i = 0; %i < getWordCount(BLG_C_SER.serverData); %i++) {
		%field = strReplace(getWord(BLG_C_SER.serverData, %i), "-", "\t");
		%host = getField(%field, 0);
		%addr = getField(%field, 1);
		echo(%addr TAB BLG_C_INF.serverlist_serverByIp[%addr]);
		if((%data = BLG_C_INF.serverlist_serverByIp[%addr]) !$= "") {
			if(strPos(%field, BLG_C_INF.oldServerData) == -1) {
				RTBCC_NotificationManager.push("Server Online", getField(%server, 4), "server", "", 5000);
			} 

			if(%host == getNumKeyId()) {
				%canEdit = true;
				%cat = "Your Servers";
			} else {
				%canEdit = false;
				%cat = "Following";
			}
			echo("add");
			BLG_C_SER.addServer(getField(%data, 4), %cat, %addr, getField(%data, 2), getField(%data, 3), %canEdit);
		}
	}
	BLG_C_SER.draw();
}

//================================
// GUI Effects
//================================

function BLG_ServerList_Mouse::onMouseMove(%this, %mod, %pos, %click) {
	%pos = vectorSub(%pos, %this.getCanvasPosition());
	%obj = mFloor((getWord(%pos,1)-5)/26)+1;
	
	if(%this.hover) {
		%this.hover.setBitmap("Add-Ons/System_BlocklandGlass/client/image/serverList/bar2");
	}

	if(BLG_C_SER.obj[%obj].bitmap $= "Add-Ons/System_BlocklandGlass/client/image/serverList/bar2") {
		%this.hover = BLG_C_SER.obj[%obj];
		BLG_C_SER.obj[%obj].setBitmap("Add-Ons/System_BlocklandGlass/client/image/serverList/bar2_hover");
	}
}

function BLG_ServerList_Mouse::onMouseUp(%this, %mod, %pos, %click) {
	%pos = vectorSub(%pos, %this.getCanvasPosition());
	%obj = mFloor((getWord(%pos,1)-5)/26)+1;

	%ob = BLG_C_SER.obj[%obj];
	if(%ob.blgdata !$= "") {
		canvas.pushDialog(BLG_ServerInfo);
		BLG_C_INF.getInfo(BLG_C_INF.server[getField(%ob.blgdata,1)], false);
	}
}