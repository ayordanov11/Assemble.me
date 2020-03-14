	fn ExportMesh msh outFile = (
		print "fn ExportMesh"
		tmesh = snapshotAsMesh msh

		num_verts = tmesh.numverts 
		num_faces = tmesh.numfaces

		--format "%,%\n" num_verts num_faces to:out_file
		format "<MeshGeometry3D  " to:outFile
		print "Exporting:" + msh.name
		format "x:Key = \"g%\"\n" (CleanString msh.name) to:outFile
		format "Positions=\n\t \"" to:outFile                                                  
		for v = 1 to num_verts do
		(
			vert = getVert tmesh v
			format "%,%,% " vert.x vert.y vert.z to:outFile
		)
		format "\" \n" to:outFile

		format "TriangleIndices=\n\t\"" to:outFile
		for f = 1 to num_faces do
		(
			face = getFace tmesh f
			format "%,%,% " ((face.x-1) as integer) ((face.y-1) as integer) ((face.z-1) as integer) to:outFile
		)

		format "\" \n" to:outFile

		format "Normals=\n\t\"" to:outFile
		for f = 1 to num_faces do
		(
			normal = getFaceNormal tmesh f
			format "%,%,% " normal.x normal.y normal.z to:outFile
		)
		
		format "\" \n" to:outFile

		if (getNumTVerts tmesh) > 0 then (
			format "TextureCoordinates=\n\t\"" to:outFile
			for f = 1 to num_verts do
			(
				UVW = getTVert tmesh f
				format "% %" UVW.x UVW.y to:outFile
				if f < num_verts then format ", " to:outFile
			)
			format "\" \n" to:outFile
		)
		
		format "/>\n\n" to:outFile
		
		delete tmesh
	)


	
	fn ExportMaterial mat outFile = (
		print "fn ExportMaterial"
		format "<MaterialGroup " to:outFile		
		format "x:Key = \"m%\" >\n" (CleanString mat.name) to:outFile
		
		format "<DiffuseMaterial>\n<DiffuseMaterial.Brush>\n" to:outFile
		if mat.diffusemap != undefined then
			format "<ImageBrush ViewportUnits=\"Absolute\" TileMode=\"Tile\"  ImageSource=\"%\"/>\n" (mat.diffusemap.bitmap.filename) to:outFile
		else
			format "<SolidColorBrush Color=\"%\"/>\n" (colorToHex mat.diffuse) to:outFile			
		format "</DiffuseMaterial.Brush>\n</DiffuseMaterial>\n " to:outFile			

		if mat.selfIllumMap != undefined then (
			format "<EmissiveMaterial>\n<EmissiveMaterial.Brush>\n"to:outFile
			format "<ImageBrush ViewportUnits=\"Absolute\" TileMode=\"Tile\"  ImageSource=\"%\"/>\n" (mat.selfIllumMap.bitmap.filename) to:outFile
			format "</EmissiveMaterial.Brush>\n</EmissiveMaterial>\n"to:outFile
		)

		format "</MaterialGroup>\n\n" to:outFile
	)
	
	fn ExportCamera cam outFile=(
		print "fn ExportCamera"
		if cam.orthoProjection == true then (
			format "<OrthographicCamera  " to:outFile
		)else(
			format "<PerspectiveCamera  " to:outFile	
			format "FieldOfView=\"%\" " cam.fov to:outFile			
		)
		Lookvector = (cam.target.pos - cam.pos) / (distance cam.pos cam.target.pos)
		format "LookDirection=\"%,%,%\" " Lookvector.x Lookvector.y Lookvector.z to:outFile
		Format " FarPlaneDistance=\"%\" " cam.farClip to:outFile
		Format " NearPlaneDistance=\"0.1\" " cam.nearClip to:outFile
		format " x:Key=\"c%\" "  (CleanString cam.name) to:outFile
		format " Position = \"% % %\" " cam.pos.x cam.pos.y cam.pos.z to:outFile
		format " UpDirection=\"0,0,1\" " to:outFile
		format " />\n" to:outFile
	)
	
	fn ExportLight lght outFile = (
		 print "fn ExportLight"
		if (classof lght) == TargetDirectionallight then (
		   DirectionVector = (lght.target.pos-lght.pos)/(distance lght.pos lght.target.pos)
		   format "<DirectionalLight Color=\"%\" Direction=\"%,%,%\"/>\n"  (ColorToHex lght.color) DirectionVector.x DirectionVector.y DirectionVector.z to:outFile
		)
		if(classof lght) == Omnilight then (
			
		)
		if(classof lght) ==  targetSpot then (
			
		)
		if(classof lght) == Directionallight then (
			format "<DirectionalLight Color=\"%\" Direction=\"0,0,-1\">\n"  (ColorToHex lght.color) to:outFile
			format "\t<DirectionalLight.Transform>\n" to:outFile
            format "\t\t<RotateTransform3D Rotation=\"%,%,%\" />\n" (lght.rotation as eulerangles).x (lght.rotation as eulerangles).y (lght.rotation as eulerangles).z to:outFile
			format "\t</DirectionalLight.Transform>\n" to:outFile
			format "</DirectionalLight >\n" to:outFile
		)
		
		if(classof lght) == freeSpot then (
-- 			<SpotLight Position="1,1,1" Direction="0,0,-1" Color="AliceBlue" >
--             <SpotLight.Transform>
--                 <RotateTransform3D Rotation="0,0,0" />
--             </SpotLight.Transform>
--         </SpotLight>
		)
	    
	)
	fn ExportModel model outFile =(
		 print "fn ExportModel"
		format "<GeometryModel3D " to:outFile
		--format "x:Name=\"Model%\" \n" (CleanString model.name) to:outFile
		format "\tGeometry=\"{StaticResource g%}\"  " (CleanString model.name) to:outFile
		format "\t Material=\"{StaticResource m%}\" /> \n" (CleanString model.material.name) to:outFile
	)
	
	fn ExportScene outFile = (
		print "fn ExportScene"		
		format "\n<Model3DGroup x:Key= \"Scene%\" >\n " (CleanString maxFileName) to:OutFile
		format "<AmbientLight Color=\"%\" />\n" (ColorToHex ambientcolor)  to:OutFile

		for l = 1 to lights.count do
		(
			ExportLight lights[l] outFile
		)	
		for m = 1 to geometry.count do
		(
			ExportModel geometry[m] outFile
		)	
		format "</Model3DGroup>\n " to:outFile
	)





	