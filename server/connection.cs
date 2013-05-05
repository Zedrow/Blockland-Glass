if(!isObject(BLG_Connection)) {
	new ScriptObject(BLG_Connection) {
		host = "direct.zivle.com";
		port = 9898;
	};
	BLG_Modules.add(BLG_Connection);
}

function BLG_Connection::init(%this) {
	%this.tcp = %tcp = new TCPObject(BLG_CON_TCP);

	%tcp.connect(%this.host @ ":" @ %this.port);
}

function BLG_Connection::placeCall(%this, %protocol, %data) {
	if(%this.threads++ > 255) {
		%this.threads = 0;
	}
	%this.thread[%this.threads] = %protocol TAB %data;
	%this.tcp.send(%protocol TAB %this.threads TAB %data @ "\r\n");
	return %this.threads;
}

function BLG_Connection::setHandle(%this, %protocol, %func) {
	%this.handle[%protocol] = %func;
}

function BLG_Connection::setErrorHandle(%this, %protocol, %func) {
	%this.errorHandle[%protocol] = %func;
}

function BLG_Connection::reconnect(%this) {
	echo("BLG: Attempting reconnect");
	BLG_CON_TCP.delete();
	%this.tcp = %tcp = new TCPObject(BLG_CON_TCP);
	%tcp.connect(%this.host @ ":" @ %this.port);
}

function BLG_CON_TCP::onConnected(%this) {
	echo("Connected to BLG server");
	BLG_Connection.placeCall("auth", "init\tserver\t" @ $BLG::Version TAB $Pref::Player::NetName TAB $Server::Port);
}

function BLG_CON_TCP::onLine(%this, %line) {
	echo(%line);
	%protocol = getField(%line, 0);
	if(%protocol $= "ERROR") {
		%thread = getField(%line, 1);
		%proto = getField(BLG_Connection.thread[%thread], 0);
		if(BLG_Connection.errorHandle[%proto] !$= "") {
			eval(BLG_Connection.errorHandle[%proto] @ "(" @ %thread @ ",\"" @ BLG_Connection.thread[%thread] @ "\");");
		}
		return;
	}

	if(BLG_Connection.handle[%protocol] !$= "") {
		eval(BLG_Connection.handle[%protocol] @ "(\"" @ %line @ "\");");
	}
}

function BLG_CON_TCP::onDisconnect(%this) {
	if(BLG_Connection.peacefulDisconnect) {
		BLG_Connection.peacefulDisconnect = false;
		return;
	}

	BLG_Connection.schedule(getRandom(10, 100)*100, reconnect);
}

function BLG_CON_TCP::onConnectFailed(%this) {
	BLG_Connection.schedule(getRandom(10, 100)*100, reconnect);
}

function BLG_CON_TCP::onDNSFailed(%this) {
	BLG_Connection.schedule(getRandom(10, 100)*100, reconnect);

}

if(!isObject(BLG_Auth)) {
	new ScriptObject(BLG_Auth);
	BLG_Connection.setHandle("auth", "BLG_Auth.onLine");
}

function BLG_Auth::onLine(%this, %line) {
	if(getField(%line, 1) $= "success") {
		echo("Place Call: " @ BLG_S_INF.port);
		BLG_Connection.placeCall("server", "data\t" @ BLG_S_INF.port);
	} else if(getField(%line, 1) $= "version") {
		echo("\n\n********************************");
		echo("*    THERE IS A BLG UPDATE!    *");
		echo("*    DOWNLOAD FOR THE FORUM    *");
		echo("********************************\n\n");
	}
}