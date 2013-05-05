//================================
// Blockland Glass - Settings
//================================

if(!isObject(BLG_Settings)) {
	new ScriptGroup(BLG_Settings) {

	};
	BLG_Modules.add(BLG_Settings);
}

function BLG_Settings::unload(%this) {

}

function BLG_Settings::init(%this) {
	if(!isFile("config/BLG/config.dat")) {
		%this.setValue("s_info", "port", "0");

		%this.setValue("s_stats", "player", "1");
		%this.setValue("s_stats", "brick", "1");
		%this.setValue("s_stats", "joins", "1");
		%this.setValue("s_stats", "graph", "1");
	} else {
		%this.loadData();
	}
}

function BLG_Settings::setValue(%this, %category, %name, %data) {
	if(!isObject(%this.value[%category, %name])) {
		%so = %this.value[%category, %name] = new ScriptObject() {
			class = "BLG_Setting";
			category = %category;
			name = %name;

			data = %data;
		};
		%this.add(%so);
	} else {
		%this.value[%category, %name].data = %data;
	}
}

function BLG_Settings::value(%this, %category, %name) {
	return %this.value[%category, %name].data;
}

function BLG_Settings::loadData(%this) {
	%fo = new FileObject();
	%fo.openForRead("config/BLG/config.dat");
	while(!%fo.isEOF()) {
		%line = %fo.readLine();
		BLG_Settings.setValue(getField(%line, 0), getField(%line, 1), getField(%line, 2));
	}
	%fo.close();
	%fo.delete();
}

function BLG_Settings::saveData(%this) {
	%fo = new FileObject();
	%fo.openforwrite("config/BLG/config.dat");
	for(%i = 0; %i < %this.getCount(); %i++) {
		%sett = %this.getObject(%i);
		%fo.writeLine(%sett.category TAB %sett.name TAB %sett.data);
	}
	%fo.close();
	%fo.delete();
}

function serverCmdBLGSettings(%client, %func, %arg1, %arg2) {
	if(%client.isSuperAdmin) {
		if(%func $= "") {
			%s = BLG_Settings;
			commandToClient(%client, 'BLGSettings', %s.value("s_info", "port"), %s.value("s_stats", "player") TAB %s.value("s_stats", "brick") TAB %s.value("s_stats", "joins") TAB %s.value("s_stats", "graph"));
		} else if(%func $= "update") {
			%s = BLG_Settings;
			%s.setValue("s_info", "port", %arg1);

			%s.setValue("s_stats", "player", getField(%arg2, 0));
			%s.setValue("s_stats", "brick", getField(%arg2, 1));
			%s.setValue("s_stats", "joins", getField(%arg2, 2));
			%s.setValue("s_stats", "graph", getField(%arg2, 3));
		}
		
		BLG_Settings.saveData();
	}
}

package BLG_Settings {
	function onExit() {
		BLG_Settings.saveData();
		parent::onExit();
	}
};
activatePackage(BLG_Settings);