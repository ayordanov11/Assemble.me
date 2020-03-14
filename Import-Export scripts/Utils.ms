-- Convert a Max color to a standard color string like #AARRGGBB
fn ColorToHex Clr = (
	str="#"
	C = (bit.intAsHex clr.a)
	if C.count == 1 then str += "0"+C
		else str += C
	C = (bit.intAsHex clr.r)
	if C.count == 1 then str += "0"+C
		else str += C
	C = (bit.intAsHex clr.g)
	if C.count == 1 then str += "0"+C
		else str += C
	C = (bit.intAsHex clr.b)
	if C.count == 1 then str += "0"+C
		else str += C
	return (toupper str)
)

-- Remove unwanted chars that can cause problems in Xaml
fn CleanString str = (
	print ("cleaning:"+str)
	return (substituteString (substituteString str "." "_") " " "_")
)
	
--Rounds down a value to a power of 2
fn FloorPowerOf2  val = (
	MaxVal = 4096	
	while Maxval>0 do(
	    if val > MaxVal then return MaxVal	
		MaxVal/=2
	)	
)
