if(!isObject(GuiTextRightProfile)) new GuiControlProfile(GuiTextRightProfile : GuiTextProfile) {
	justify = "right";
};

if(!isObject(BLG_HudText)) new GuiControlProfile(BLG_HudText)
{
	fontColor = "255 255 255 255";
	fontSize = 20;
	fontType = "Impact";
	justify = "Left";
};

if(!isObject(BLG_HudTextRight)) new GuiControlProfile(BLG_HudTextRight)
{
   fontColor = "255 255 255 255";
   fontSize = 20;
   fontType = "Impact";
   justify = "Right";
};

if(!isObject(BLG_HudTextCenter)) new GuiControlProfile(BLG_HudTextCenter)
{
   fontColor = "255 255 255 255";
   fontSize = 20;
   fontType = "Impact";
   justify = "Center";
};

if(!isObject(BLG_ScrollProfile)) new GuiControlProfile(BLG_ScrollProfile)
{
   fillColor = "230 230 230 255";
   borderColor = "150 150 150 255";
   border = "1";
   opaque = "1";

   fontColor = "70 70 70 255";
   fontColors[0] = "70 70 70 255";
   fontColors[1] = "255 70 70 255"; //R
   fontColors[2] = "70 255 70 255"; //G
   fontColors[3] = "70 70 255 255"; //B
   fontColors[4] = "255 255 255 255";

   hasBitmapArray = true;
   bitmap = "Add-Ons/System_BlocklandGlass/client/image/scroll.png";
};

if(!isObject(BLG_TextEditProfile)) new GuiControlProfile(BLG_TextEditProfile : GuiTextEditProfile)
{
   fillColor = "230 230 230 255";
   borderColor = "150 150 150 255";
   fontColor = "70 70 70 255";
};

if(!isObject(BLG_TextProfile)) new GuiControlProfile(BLG_TextProfile : GuiTextProfile)
{
   fontColor = "70 70 70 255";
   fontColors[0] = "70 70 70 255";
   fontColors[1] = "255 70 70 255"; //R
   fontColors[2] = "70 255 70 255"; //G
   fontColors[3] = "70 70 255 255"; //B
   fontColors[4] = "255 255 255 255";
};

if(!isObject(BLG_TextProfile2)) new GuiControlProfile(BLG_TextProfile2 : BLG_TextProfile)
{
   autoSizeWidth = false;
};

if(!isObject(BLG_TextCenterProfile)) new GuiControlProfile(BLG_TextCenterProfile : BLG_TextProfile)
{
   justify = "Center";
};

if(!isObject(BLG_ProgressProfile)) new GuiControlProfile(BLG_ProgressProfile : GuiProgressProfile)
{
   borderColor = "150 150 150 255";
   borderThickness = "0.5";
   fillColor = "100 100 100 128";   
};

if(!isObject(BLG_BlockButtonProfile)) new GuiControlProfile(BLG_BlockButtonProfile : BlockButtonProfile)
{
   fontColor = "40 40 40 255";
   fontColors[0] = "40 40 40";
   fontColors[1] = "255 40 40";
   fontColors[2] = "40 255 40";
   fontColors[3] = "40 40 255";
   
};

if(!isObject(BLG_VersionTextProfile)) new GuiControlProfile(BLG_VersionTextProfile : MM_LeftProfile) {
   fontOutlineColor = "24 175 24 255";
   justify = "center";
};