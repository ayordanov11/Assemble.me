include "utils.ms"	
include "XamlExport.ms"

fn Export  outFile = (		
	   print "fn Export"
		
			
				
			format "<ResourceDictionary \n " to:outFile
			format "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" to:outFile 
			format "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"> \n" to:outFile
			
			for m = 1 to geometry.count do
			(
				ExportMesh geometry[m] outFile
			)	

			for m = 1 to SceneMaterials.count do
			(
				ExportMaterial sceneMaterials[m] outFile
			)	
			
			for c = 1 to cameras.count do
			(
				 if (classof cameras[c]) != Targetobject then 
					ExportCamera cameras[c] outFile
			)
			
			ExportScene outFile
			
			format "</ResourceDictionary>" to:outFile
			close outFile		
			
	
	)
	
rollout MainUi "Export To Xaml" width:295 height:200
(
	button btnExport "Export" pos:[8,96] width:74 height:24
	button btnCancel "Cancel" pos:[88,96] width:74 height:24

	on btnExport pressed do
	( 
		outName = GetSaveFileName types:"Xaml(*.xaml)|All|*.*|"				
		if outName != undefined then (		
			outFile = createfile outName		
			Export outFile
			edit outName
			closeRolloutFloater theNewFloater
		)					
	)
	
	on btnCancel pressed do
	(
		closeRolloutFloater theNewFloater
	)
	
)
	theNewFloater = newRolloutFloater "Max2Xaml" 300 220
	addRollout MainUi theNewFloater