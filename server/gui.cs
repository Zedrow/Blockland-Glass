//================================
// Blockland Glass - Server - GUI
//================================

if(!isObject(BLG_S_GUI)) {
	%so = new ScriptObject(BLG_S_GUI) {
		guis = 0;
		required = false;
	};
	BLG_Modules.add(%so);
}

function BLG_S_GUI::unload(%this) {

}

function BLG_S_GUI::init(%this) {
	
}

function BLG_S_GUI::addGui(%this, %name, %url, %mand) {
	if(%mand) {
		%this.required = true;
	}
	
	%this.gui[%this.guis] = %name TAB %url TAB %mand;
	%this.guis++;
}