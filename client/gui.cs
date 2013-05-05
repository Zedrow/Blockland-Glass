//================================
// Blockland Glass - Server - GUI
//================================

if(!isObject(BLG_C_GUI)) {
	%so = new ScriptObject(BLG_C_GUI) {
		guis = 0;
		mand = false;
	};
	BLG_Modules.add(%so);
}

function BLG_C_GUI::download(%this) {
	Canvas.pushDialog(ProgressBarGui);
	ProgressBarGui_Progress.setValue(0);
	ProgressBarGui_Status.setValue("Downloading GUI 1 of " @ %this.guis);

	%this.download_next(0);
}

function BLG_C_GUI::download_next(%this, %i) {
	ProgressBarGui_Progress.setValue(%i/%this.guis);
	ProgressBarGui_Status.setValue("Downloading GUI " @ %i+1 @ " of " @ %this.guis);


	%url = getField(%this.gui[%i], 1);
	%host = getSubStr(%url, 0, strPos(%url, "/"));
	%path = getSubStr(%url, strPos(%url, "/"), strLen(%url));

	echo("Downloading GUI " @ %i+1 @ " of " @ %this.guis @ " (" @ %url @ ")");

	%tcp = new TCPObject(BLG_C_GUI_Downloader) {
		gui = %this.gui[%i];

		i = %i;

		url = %url;
		host = %host;
		path = %path;
	};
	%tcp.connect(%host @ ":80");
}

function BLG_C_GUI_Downloader::onConnected(%this) {
	%tString = "GET " @ %this.path @ " HTTP/1.1\n";
	%tString = %tString @ "Host: " @ %this.host @ "\n";
	%tString = %tString @ "\n";

	echo(%tString);

	%this.send(%tString);
}

function BLG_C_GUI_Downloader::onLine(%this, %line) {
	echo(%line);
	if(strPos(%line, "Content-Length:") >= 0) {
		%this.size = getWord(%line, 1);
		echo(%this.size);
	}
	
	if(strPos(%line, "404") >= 0) {
		messageBoxOk("Downloader Error", "One of the GUIs returned 404. Join aborted");
		Canvas.popDialog(ProgressBarGui);
	}

	if(%line $= "Content-Type: application/zip") {
		%this.isFile = true;
	}

	if(%line $= "") {
		if(%this.isFile) {
			if(%this.size) {
				%this.setBinarySize(%this.size);
			} else {
				%this.setBinary(true);
			}
		} else {
			messageBoxOk("Downloader Error", "One of the GUIs is not a downloadable file. Join aborted");
			Canvas.popDialog(ProgressBarGui);
		}
	}
}

function BLG_C_GUI_Downloader::onBinChunk(%this, %chunk) {
	if(%this.size) {
		if(%chunk >= %this.size) {
			%this.finished();
		}
	} else {
		cancel(%this.chunkSchedule);
		%this.chunkSchedule = %this.schedule(1000, finished);
	}
}

function BLG_C_GUI_Downloader::finished(%this) {
	Canvas.popDialog(ProgressBarGui);
	if(isWriteableFilename("config/GUIs/" @ getField(%this.gui, 0) @ ".zip"))
	{
		%this.saveBufferToFile("config/GUIs/" @ getField(%this.gui, 0) @ ".zip");
		discoverFile("config/GUIs/*");
		exec("config/GUIs/" @ urlEnc(getField(%this.gui, 0)) @ "/client.cs");
	}
	%this.disconnect();
	%this.schedule(0, delete);

	if(%this.i+1< BLG_C_GUI.guis) {
		BLG_C_GUI.download_next(%i++);
	} else {
		JoinServerGui.join(1);
	}
}

package BLG_C_GUI {
	function JoinServerGui::join(%this, %blg) {
		if(!%blg) {
			%addr = getField(JS_serverList.getValue(), 9);
			if(BLG_C_INF.server[%addr]) {
				echo("Checking Server for BLG - Giving 5 seconds");
				cancel(BLG_C_GUI.checkSchedule);
				BLG_C_GUI.checkSchedule = %this.schedule(5000, join, 1);
				BLG_C_INF.getInfo(BLG_C_INF.server[%addr], true);
			} else {
				parent::join(%this);
			}
		} else {
			parent::join(%this);
		}
	}

	function BLG_C_INF_TCP::onDisconnect(%this) {
		cancel(BLG_C_GUI.checkSchedule);
		parent::onDisconnect(%this);
		if(%this.gui) {
			if(BLG_C_GUI.guis) {
				if(BLG_C_GUI.mand) {
					BLG_C_GUI_Exit.setText("Exit");
					BLG_C_GUI_Exit.command = "canvas.popDialog(BLG_Guis);";
					BLG_C_GUI_Exit.mColor = "255 200 200 255";
				} else {
					BLG_C_GUI_Exit.setText("Join Anyways");
					BLG_C_GUI_Exit.command = "JoinServerGui.join(1);";
					BLG_C_GUI_Exit.mColor = "255 255 255 255";
				}
				canvas.pushDialog("BLG_Guis");
				BLG_C_GUI_List.clear();
				for(%i = 0; %i < BLG_C_GUI.guis; %i++) {
					BLG_C_GUI.gui[%i] = %gui = BLG_C_GUI.gui[%i];
					BLG_C_GUI_List.addRow(%i, getField(%gui, 0) TAB getField(%gui, 1) TAB (getField(%gui, 2) ? "Yes" : "No"));
				}
				BLG_C_GUI.guis = BLG_C_GUI.guis;
			} else {
				JoinServerGui.join(1);
			}
		}
	}
};