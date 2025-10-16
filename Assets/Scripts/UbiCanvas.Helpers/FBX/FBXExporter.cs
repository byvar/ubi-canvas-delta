// ===============================================================================================
//	The MIT License (MIT) for UnityFBXExporter
//
//  UnityFBXExporter was created for Building Crafter (http://u3d.as/ovC) a tool to rapidly 
//	create high quality buildings right in Unity with no need to use 3D modeling programs.
//
//  Copyright (c) 2016 | 8Bit Goose Games, Inc.
//		
//	Permission is hereby granted, free of charge, to any person obtaining a copy 
//	of this software and associated documentation files (the "Software"), to deal 
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//		
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//		
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// ===============================================================================================


using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UbiArt;
using UbiArt.Engine3D;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;

namespace UbiCanvas.Helpers.FBX {
	public class FBXExporter {
		public class FBXHeader {
			public int TextureCount { get; set; }
			public int MaterialCount { get; set; }
			public int GeometryCount { get; set; }
			public int DeformerCount { get; set; }
			public int ModelCount { get; set; }
			public int NodeAttributeCount { get; set; }
		}
		public static string VersionInformation => "ubi-canvas FBX Export";

		public static long GetRandomFBXId() => System.BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);

		public static void ExportFBX(Mesh3DComponent component, Mesh3DComponent_Template template, string outputPath) {

			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			string outputFile = System.IO.Path.Combine(outputPath, $"mesh.fbx");
			var header = new FBXHeader();

			StringBuilder sb2 = new StringBuilder();
			WriteGameObject(sb2, component, template, outputPath, header);

			StringBuilder sb = new StringBuilder();
			WriteHeader(sb, header);
			sb.AppendLine(sb2.ToString());
			string buildMesh = sb.ToString();

			if (System.IO.File.Exists(outputFile))
				System.IO.File.Delete(outputFile);

			System.IO.File.WriteAllText(outputFile, buildMesh);
		}
		
		#region Header
		private static void WriteHeader(StringBuilder sb, FBXHeader header) {
			// ========= Create header ========
			// Intro
			sb.AppendLine("; FBX 7.3.0 project file");
			sb.AppendLine("; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.");
			sb.AppendLine("; All rights reserved.");
			sb.AppendLine("; ----------------------------------------------------");
			sb.AppendLine();

			// The header
			sb.AppendLine("FBXHeaderExtension:  {");
			sb.AppendLine("\tFBXHeaderVersion: 1003");
			sb.AppendLine("\tFBXVersion: 7300");

			// Creationg Date Stamp
			System.DateTime currentDate = System.DateTime.Now;
			sb.AppendLine("\tCreationTimeStamp:  {");
			sb.AppendLine("\t\tVersion: 1000");
			sb.AppendLine("\t\tYear: " + currentDate.Year);
			sb.AppendLine("\t\tMonth: " + currentDate.Month);
			sb.AppendLine("\t\tDay: " + currentDate.Day);
			sb.AppendLine("\t\tHour: " + currentDate.Hour);
			sb.AppendLine("\t\tMinute: " + currentDate.Minute);
			sb.AppendLine("\t\tSecond: " + currentDate.Second);
			sb.AppendLine("\t\tMillisecond: " + currentDate.Millisecond);
			sb.AppendLine("\t}");

			// Info on the Creator
			sb.AppendLine("\tCreator: \"" + VersionInformation + "\"");
			sb.AppendLine("\tSceneInfo: \"SceneInfo::GlobalInfo\", \"UserData\" {");
			sb.AppendLine("\t\tType: \"UserData\"");
			sb.AppendLine("\t\tVersion: 100");
			sb.AppendLine("\t\tMetaData:  {");
			sb.AppendLine("\t\t\tVersion: 100");
			sb.AppendLine("\t\t\tTitle: \"\"");
			sb.AppendLine("\t\t\tSubject: \"\"");
			sb.AppendLine("\t\t\tAuthor: \"\"");
			sb.AppendLine("\t\t\tKeywords: \"\"");
			sb.AppendLine("\t\t\tRevision: \"\"");
			sb.AppendLine("\t\t\tComment: \"\"");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t\tProperties70:  {");

			// Information on how this item was originally generated
			/*string documentInfoPaths = Application.dataPath + newPath + ".fbx";
			sb.AppendLine("\t\t\tP: \"DocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");
			sb.AppendLine("\t\t\tP: \"SrcDocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");*/
			sb.AppendLine("\t\t\tP: \"Original\", \"Compound\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|FileName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved\", \"Compound\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// The Global information
			sb.AppendLine("GlobalSettings:  {");
			sb.AppendLine("\tVersion: 1000");
			sb.AppendLine("\tProperties70:  {");

			// Change this for each axis system
			sb.AppendLine("\t\tP: \"CoordAxis\", \"int\", \"Integer\", \"\",0");
			sb.AppendLine("\t\tP: \"CoordAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"FrontAxis\", \"int\", \"Integer\", \"\",2");
			sb.AppendLine("\t\tP: \"FrontAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"UpAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"OriginalUpAxis\", \"int\", \"Integer\", \"\",-1");
			sb.AppendLine("\t\tP: \"OriginalUpAxisSign\", \"int\", \"Integer\", \"\",1");

			sb.AppendLine("\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",100"); // NOTE: This sets the resize scale upon import
			sb.AppendLine("\t\tP: \"OriginalUnitScaleFactor\", \"double\", \"Number\", \"\",100");
			sb.AppendLine("\t\tP: \"AmbientColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\tP: \"DefaultCamera\", \"KString\", \"\", \"\", \"Producer Perspective\"");
			sb.AppendLine("\t\tP: \"TimeMode\", \"enum\", \"\", \"\",11");
			sb.AppendLine("\t\tP: \"TimeSpanStart\", \"KTime\", \"Time\", \"\",0");
			sb.AppendLine("\t\tP: \"TimeSpanStop\", \"KTime\", \"Time\", \"\",479181389250");
			sb.AppendLine("\t\tP: \"CustomFrameRate\", \"double\", \"Number\", \"\",-1");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// The Object definations
			sb.AppendLine("; Object definitions");
			sb.AppendLine(";------------------------------------------------------------------");
			sb.AppendLine("");
			sb.AppendLine("Definitions:  {");
			sb.AppendLine("\tVersion: 100");
			sb.AppendLine("\tCount: 4");

			sb.AppendLine("\tObjectType: \"GlobalSettings\" {");
			sb.AppendLine("\t\tCount: 1");
			sb.AppendLine("\t}");


			sb.AppendLine("\tObjectType: \"Model\" {");
			sb.AppendLine($"\t\tCount: {header.ModelCount}");
			sb.AppendLine("\t\tPropertyTemplate: \"FbxNode\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"QuaternionInterpolate\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationSpaceForLimitOnly\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"AxisLen\", \"double\", \"Number\", \"\",10");
			sb.AppendLine("\t\t\t\tP: \"PreRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"PostRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"GeometricTranslation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"GeometricRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"GeometricScaling\", \"Vector3D\", \"Vector\", \"\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"LookAtProperty\", \"object\", \"\", \"\"");
			sb.AppendLine("\t\t\t\tP: \"UpVectorProperty\", \"object\", \"\", \"\"");
			sb.AppendLine("\t\t\t\tP: \"Show\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"NegativePercentShapeSupport\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",-1");
			sb.AppendLine("\t\t\t\tP: \"Freeze\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"LODBox\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"Visibility\", \"Visibility\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"Visibility Inheritance\", \"Visibility Inheritance\", \"\", \"\",1");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// The geometry
			sb.AppendLine("\tObjectType: \"Geometry\" {");
			sb.AppendLine($"\t\tCount: {header.GeometryCount}");
			sb.AppendLine("\t\tPropertyTemplate: \"FbxMesh\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
			sb.AppendLine("\t\t\t\tP: \"BBoxMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"BBoxMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Primary Visibility\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Casts Shadows\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Receive Shadows\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// Deformers
			sb.AppendLine("\tObjectType: \"Deformer\" {");
			sb.AppendLine($"\t\tCount: {header.DeformerCount}");
			sb.AppendLine("\t}");

			// NodeAttributes
			sb.AppendLine("\tObjectType: \"NodeAttribute\" {");
			sb.AppendLine($"\t\tCount: {header.NodeAttributeCount}");
			sb.AppendLine("\t}");

			// The materials
			sb.AppendLine("\tObjectType: \"Material\" {");
			sb.AppendLine($"\t\tCount: {header.MaterialCount}");
			sb.AppendLine("\t\tPropertyTemplate: \"FbxSurfacePhong\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"ShadingModel\", \"KString\", \"\", \"\", \"Phong\"");
			sb.AppendLine("\t\t\t\tP: \"MultiLayer\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"EmissiveColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
			sb.AppendLine("\t\t\t\tP: \"AmbientFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",0.8,0.8,0.8");
			sb.AppendLine("\t\t\t\tP: \"DiffuseFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"Bump\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"NormalMap\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TransparencyFactor\", \"Number\", \"\", \"A\",0");
			sb.AppendLine("\t\t\t\tP: \"DisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"DisplacementFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"VectorDisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"VectorDisplacementFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"SpecularColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
			sb.AppendLine("\t\t\t\tP: \"SpecularFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"ShininessExponent\", \"Number\", \"\", \"A\",20");
			sb.AppendLine("\t\t\t\tP: \"ReflectionColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ReflectionFactor\", \"Number\", \"\", \"A\",0");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// Explanation of how textures work
			sb.AppendLine("\tObjectType: \"Texture\" {");
			sb.AppendLine($"\t\tCount: {header.TextureCount}");
			sb.AppendLine("\t\tPropertyTemplate: \"FbxFileTexture\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"TextureTypeUse\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"Texture alpha\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"CurrentMappingType\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"WrapModeU\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"WrapModeV\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"UVSwap\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PremultiplyAlpha\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Translation\", \"Vector\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Rotation\", \"Vector\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Scaling\", \"Vector\", \"\", \"A\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"TextureRotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TextureScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"default\"");
			sb.AppendLine("\t\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"UseMipMap\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			sb.AppendLine("}");
			sb.AppendLine("");
		}
		#endregion


		private static void WriteGameObject(StringBuilder sb, Mesh3DComponent component, Mesh3DComponent_Template template, string outputPath, FBXHeader header) {
			StringBuilder objectProps = new StringBuilder();
			objectProps.AppendLine("; Object properties");
			objectProps.AppendLine(";------------------------------------------------------------------");
			objectProps.AppendLine("");
			objectProps.AppendLine("Objects:  {");

			StringBuilder objectConnections = new StringBuilder();
			objectConnections.AppendLine("; Object connections");
			objectConnections.AppendLine(";------------------------------------------------------------------");
			objectConnections.AppendLine("");
			objectConnections.AppendLine("Connections:  {");
			objectConnections.AppendLine("\t");

			// Dictionary to collect all materials used by this FBX
			var writtenObjects = new HashSet<long>();

			// First finds all unique materials and compiles them (and writes to the object connections) for funzies
			string materialsObjectSerialized = "";
			string materialConnectionsSerialized = "";

			// Run recursive FBX Mesh grab over the entire gameobject
			List<UbiArt.Path> meshPaths = new List<UbiArt.Path>();
			if(!UbiArt.Path.IsNull(component.mesh3D))
				meshPaths.Add(component.mesh3D);
			else if(!UbiArt.Path.IsNull(template.mesh3D))
				meshPaths.Add(template.mesh3D);
			else {
				CListO<UbiArt.Path> usedMeshList = component.mesh3DList;
				if (usedMeshList == null || !usedMeshList.Any()) usedMeshList = template.mesh3Dlist;
				if (usedMeshList != null) {
					foreach (var p in usedMeshList) {
						meshPaths.Add(p);
					}
				}
			}
			var usedMaterialList = (component.materialList == null || !component.materialList.Any()) ? template.materialList : component.materialList;

			foreach (var m in meshPaths) {
				WriteGeometryForGameObject(m, objectProps, objectConnections, writtenObjects, header, usedMaterialList);
			}
			AllMaterialsToString(usedMaterialList, outputPath, out materialsObjectSerialized, out materialConnectionsSerialized, header);


			// write the materials to the objectProps here. Should not do it in the above as it recursive.

			objectProps.Append(materialsObjectSerialized);
			objectConnections.Append(materialConnectionsSerialized);

			// Close up both builders;
			objectProps.AppendLine("}");
			objectConnections.AppendLine("}");

			sb.Append(objectProps.ToString());
			sb.Append(objectConnections.ToString());
		}
		
		#region Geometry
		/// <summary>
		/// Gets all the meshes and outputs to a string (even grabbing the child of each gameObject)
		/// </summary>
		/// <returns>The mesh to string.</returns>
		/// <param name="m3d">GameObject Parent.</param>
		/// <param name="objects">The StringBuidler to create objects for the FBX file.</param>
		/// <param name="connections">The StringBuidler to create connections for the FBX file.</param>
		/// <param name="parentObject">Parent object, if left null this is the top parent.</param>
		/// <param name="parentGao">Parent model id, 0 if top parent.</param>
		public static void WriteGeometryForGameObject(UbiArt.Path p,
										   StringBuilder objects,
										   StringBuilder connections,
										   HashSet<long> writtenObjects,
										   FBXHeader header,
										   CListO<GFXMaterialSerializable> materials) {

			var m3d = p.GetObject<Mesh3D>();
			if(m3d == null) return;

			long gameObjectID = p.stringID.stringID;
			if (writtenObjects.Contains(gameObjectID)) return;
			writtenObjects.Add(gameObjectID);

			long geometryID = FBXExporter.GetRandomFBXId();

			bool appendComma = false;
			StringBuilder tempObjectSb = new StringBuilder();
			StringBuilder tempConnectionsSb = new StringBuilder();

			string meshName = p.GetFilenameWithoutExtension();

			// A NULL parent means that the gameObject is at the top
			string modelType = "Null";
			bool isBone = false;

			modelType = "Mesh";

			var currentParent = 0;
			if (currentParent == 0) {
				tempConnectionsSb.AppendLine($"\t;Model::{meshName}, Model::RootNode");
				tempConnectionsSb.AppendLine($"\tC: \"OO\",{gameObjectID},{currentParent}");
				tempConnectionsSb.AppendLine();
			} else {
				tempConnectionsSb.AppendLine($"\t;Model::{meshName}, Model::USING PARENT");
				tempConnectionsSb.AppendLine($"\tC: \"OO\",{gameObjectID},{currentParent}");
				tempConnectionsSb.AppendLine();
			}

			header.ModelCount++;
			string modelName = $"Model::{meshName}";
			tempObjectSb.AppendLine($"\tModel: {gameObjectID}, \"{modelName}\", \"{modelType}\" {{");
			tempObjectSb.AppendLine("\t\tVersion: 232");
			tempObjectSb.AppendLine("\t\tProperties70:  {");
			tempObjectSb.AppendLine("\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",4");
			tempObjectSb.AppendLine("\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",1");
			tempObjectSb.AppendLine("\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
			tempObjectSb.AppendLine("\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			tempObjectSb.AppendLine("\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
			if (isBone) {
				tempObjectSb.AppendLine("\t\t\tP: \"lockInfluenceWeights\", \"Bool\", \"\", \"A+U\",0");
			}

			// ===== Local Transform =========
			var pos = UnityEngine.Vector3.zero ;
			var rot = UnityEngine.Vector3.zero;
			var scl = UnityEngine.Vector3.one;

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A+\",{0},{1},{2}", pos.x, pos.y, pos.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A+\",{0},{1},{2}", rot.x, rot.y, rot.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",{0},{1},{2}", scl.x, scl.y, scl.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendLine("\t\t\tP: \"currentUVSet\", \"KString\", \"\", \"U\", \"map1\"");
			tempObjectSb.AppendLine("\t\t}");
			tempObjectSb.AppendLine("\t\tShading: T");
			tempObjectSb.AppendLine("\t\tCulling: \"CullingOff\"");
			tempObjectSb.AppendLine("\t}");

			// Adds in geometry if it exists, if it it does not exist, this is a empty gameObject file and skips over this
			if (m3d != null && !isBone) {

				IEnumerable<T> SelectTriple<T>(Mesh3D.TripleIndex t, System.Func<int, T> func) {
					yield return func((int)t.Index0);
					yield return func((int)t.Index1);
					yield return func((int)t.Index2);
				}
				Vec3d ConvertVertex(Vec3d v) => v; //new Vec3d(v.x, v.y, -v.z);
				Vec3d GetVertex(int index) => ConvertVertex(m3d.Vertices[index]);
				//Vec2d GetUV(int index) => m3d.UVs[index]; //.GetUnityVector();
				Vec3d GetNormal(int index) => ConvertVertex(m3d.Normals[index]);

				// =================================
				//         General Geometry Info
				// =================================
				// Generate the geometry information for the mesh created
				header.GeometryCount++;
				string mainGeometryName = $"Geometry::{p.GetFilenameWithoutExtension():X8}";
				tempObjectSb.AppendLine($"\tGeometry: {geometryID}, \"{mainGeometryName}\", \"Mesh\" {{");

				// ===== WRITE THE VERTICES =====
				WriteVector3dArray("Vertices", m3d.Vertices.Select((v, i) => GetVertex(i)).ToArray(), tempObjectSb);

				// ======= WRITE THE TRIANGLES ========
				int triangleCount = m3d.Elements.Sum(e => e.Triangles.Count) * 3;
				tempObjectSb.AppendLine("\t\tPolygonVertexIndex: *" + triangleCount + " {");

				// Write triangle indices
				tempObjectSb.Append("\t\t\ta: ");
				appendComma = false;
				foreach (var e in m3d.Elements) {
					foreach (var t in e.Triangles) {
						if (appendComma) tempObjectSb.Append(",");
						appendComma = true;

						tempObjectSb.AppendFormat("{0},{1},{2}",
												t.Vertex.Index0,
												t.Vertex.Index1,
												-t.Vertex.Index2 - 1); // <= Tells the poly is ended

					}
				}

				tempObjectSb.AppendLine();

				tempObjectSb.AppendLine("\t\t} ");
				tempObjectSb.AppendLine("\t\tGeometryVersion: 124");

				bool hasNormals = true; // geo.Normals != null;
				if (hasNormals) {
					// ===== WRITE THE NORMALS ==========
					var normals = m3d.Elements.SelectMany(el => el.Triangles.SelectMany(t => SelectTriple(t.Normal, i => GetNormal(i)))).ToArray();
					tempObjectSb.AppendLine("\t\tLayerElementNormal: 0 {");
					tempObjectSb.AppendLine("\t\t\tVersion: 101");
					tempObjectSb.AppendLine("\t\t\tName: \"\"");
					tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
					tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");

					WriteVector3dArray("Normals", m3d.Normals.ToArray(), tempObjectSb, indentCount: 3);
					tempObjectSb.AppendLine("\t\t\tNormalsIndex: *" + triangleCount + " {");
					tempObjectSb.Append("\t\t\t\ta: ");

					appendComma = false;
					foreach (var e in m3d.Elements) {
						foreach (var t in e.Triangles) {
							if (appendComma) tempObjectSb.Append(",");
							appendComma = true;

							tempObjectSb.AppendFormat("{0},{1},{2}", t.Normal.Index0, t.Normal.Index1, t.Normal.Index2);
						}
					}

					tempObjectSb.AppendLine();

					tempObjectSb.AppendLine("\t\t\t}");
					tempObjectSb.AppendLine("\t\t}");
				}

				// ================ UV CREATION =========================

				// -- UV 1 Creation
				var uvs = m3d.UVs;

				tempObjectSb.AppendLine("\t\tLayerElementUV: 0 {"); // the Zero here is for the first UV map
				tempObjectSb.AppendLine("\t\t\tVersion: 101");
				tempObjectSb.AppendLine("\t\t\tName: \"map1\"");
				tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
				tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
				WriteVector2dArray("UV", uvs.ToArray(), tempObjectSb, indentCount: 3);

				// UV tile index coords
				tempObjectSb.AppendLine("\t\t\tUVIndex: *" + triangleCount + " {");
				tempObjectSb.Append("\t\t\t\ta: ");

				appendComma = false;
				foreach (var e in m3d.Elements) {
					foreach (var t in e.Triangles) {
						if (appendComma) tempObjectSb.Append(",");
						appendComma = true;

						tempObjectSb.AppendFormat("{0},{1},{2}", t.UV1.Index0, t.UV1.Index1, t.UV1.Index2);

					}
				}

				tempObjectSb.AppendLine();

				tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t}");

				// ============ MATERIALS =============

				tempObjectSb.AppendLine("\t\tLayerElementMaterial: 0 {");
				tempObjectSb.AppendLine("\t\t\tVersion: 101");
				tempObjectSb.AppendLine("\t\t\tName: \"\"");
				tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygon\"");
				tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");

				int totalFaceCount = 0;

				// So by polygon means that we need 1/3rd of how many indices we wrote.
				int numberOfSubmeshes = (int)m3d.Elements.Count;

				StringBuilder submeshesSb = new StringBuilder();

				appendComma = false;
				for (int i = 0; i < m3d.Elements.Count; i++) {
					var e = m3d.Elements[i];
					foreach (var t in e.Triangles) {
						if (appendComma) submeshesSb.Append(",");
						appendComma = true;
						submeshesSb.Append(e.MaterialIndex);
						//submeshesSb.AppendFormat("{0}", e.MaterialID);
						totalFaceCount += 1;
					}
				}

				tempObjectSb.AppendLine("\t\t\tMaterials: *" + totalFaceCount + " {");
				tempObjectSb.Append("\t\t\t\ta: ");
				tempObjectSb.AppendLine(submeshesSb.ToString());
				tempObjectSb.AppendLine("\t\t\t} ");
				tempObjectSb.AppendLine("\t\t}");

				// ============= INFORMS WHAT TYPE OF LATER ELEMENTS ARE IN THIS GEOMETRY =================
				tempObjectSb.AppendLine("\t\tLayer: 0 {");
				tempObjectSb.AppendLine("\t\t\tVersion: 100");
				if (hasNormals) {
					tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
					tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementNormal\"");
					tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
					tempObjectSb.AppendLine("\t\t\t}");
				}
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementMaterial\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementTexture\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				// TODO: Here we would add UV layer 1 for ambient occlusion UV file
				//			tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				//			tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
				//			tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 1");
				//			tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t}");
				tempObjectSb.AppendLine("\t}");

				// Add the connection for the model to the geometry so it is attached the right mesh
				tempConnectionsSb.AppendLine("\t;Geometry::, Model::" + meshName);
				tempConnectionsSb.AppendLine("\tC: \"OO\"," + geometryID + "," + gameObjectID);
				tempConnectionsSb.AppendLine();

				// Add the connection of all the materials in order of submesh
				uint id = 0;
				foreach(var matKV in materials) {
					var mat = matKV;

					// We rename the material if it is being copied
					string materialName = $"{matKV.textureSet.diffuse.GetFilenameWithoutExtension()}";

					uint materialID = 0x10000000 + id;

					tempConnectionsSb.AppendLine($"\t;Material::{materialName}, Model::{meshName}");
					tempConnectionsSb.AppendLine($"\tC: \"OO\",{materialID},{gameObjectID}");
					tempConnectionsSb.AppendLine();
					id ++;
				}

			}


			// Write connections
			objects.Append(tempObjectSb.ToString());
			connections.Append(tempConnectionsSb.ToString());
		}
		#endregion
		
		private static void WriteVector3dArray(string name, Vec3d[] vectors, StringBuilder tempObjectSb, int indentCount = 2) {
			string indent = new string('\t', indentCount);
			tempObjectSb.AppendLine($"{indent}{name}: *{vectors.Length * 3} {{");
			tempObjectSb.Append($"{indent}\ta: ");
			for (int i = 0; i < vectors.Length; i++) {
				if (i > 0) tempObjectSb.Append(",");

				tempObjectSb.AppendFormat("{0},{1},{2}", vectors[i].x, vectors[i].y, vectors[i].z);
			}
			tempObjectSb.AppendLine();
			tempObjectSb.AppendLine($"{indent}}}");
		}
		private static void WriteVector2dArray(string name, Vec2d[] vectors, StringBuilder tempObjectSb, int indentCount = 2) {
			string indent = new string('\t', indentCount);
			tempObjectSb.AppendLine($"{indent}{name}: *{vectors.Length * 2} {{");
			tempObjectSb.Append($"{indent}\ta: ");
			for (int i = 0; i < vectors.Length; i++) {
				if (i > 0) tempObjectSb.Append(",");

				tempObjectSb.AppendFormat("{0},{1}", vectors[i].x, vectors[i].y);
			}
			tempObjectSb.AppendLine();
			tempObjectSb.AppendLine($"{indent}}}");
		}

		public static void AllMaterialsToString(CListO<GFXMaterialSerializable> materials, string outputPath, out string matObjects, out string connections, FBXHeader header) {
			StringBuilder tempObjectSb = new StringBuilder();
			StringBuilder tempConnectionsSb = new StringBuilder();

			uint id = 0;

			foreach (var matKV in materials) {
				var mat = matKV;

				// We rename the material if it is being copied
				string materialName = $"{matKV.textureSet.diffuse.GetFilenameWithoutExtension()}";

				uint referenceId = 0x10000000 + id;

				header.MaterialCount++;
				// Header
				tempObjectSb.AppendLine();
				tempObjectSb.AppendLine("\tMaterial: " + referenceId + ", \"Material::" + materialName + "\", \"\" {");
				tempObjectSb.AppendLine("\t\tVersion: 102");
				tempObjectSb.AppendLine("\t\tShadingModel: \"phong\"");
				tempObjectSb.AppendLine("\t\tMultiLayer: 0");
				tempObjectSb.AppendLine("\t\tProperties70:  {");



				// Colors
				void writeColor(string name, UbiArt.Color color) {

					tempObjectSb.AppendLine($"\t\t\tP: \"{name}\", \"Vector3D\", \"Vector\", \"\",{color.r},{color.g},{color.b}");
					tempObjectSb.AppendLine($"\t\t\tP: \"{name}Color\", \"Color\", \"\", \"A\",{color.r},{color.g},{color.b}");
					// Todo: check this factor
					tempObjectSb.AppendLine($"\t\t\tP: \"{name}Factor\", \"Number\", \"\", \"A\",{color.a}");
				}

				writeColor("Ambient", UbiArt.Color.White);
				writeColor("Diffuse", UbiArt.Color.White);
				writeColor("Specular", UbiArt.Color.White);

				// This is already 0 in the model, but Blender needs this to be 0 in the material too, otherwise the material becomes Metallic
				tempObjectSb.AppendLine("\t\t\tP: \"ReflectionFactor\", \"Number\", \"\", \"A\",0");


				// TODO: Add these to the file based on their relation to the PBR files
				//				tempObjectSb.AppendLine("\t\t\tP: \"ShininessExponent\", \"Number\", \"\", \"A\",6.31179285049438");
				//				tempObjectSb.AppendLine("\t\t\tP: \"Shininess\", \"double\", \"Number\", \"\",6.31179285049438");
				//				tempObjectSb.AppendLine("\t\t\tP: \"Reflectivity\", \"double\", \"Number\", \"\",0");

				tempObjectSb.AppendLine("\t\t}");
				tempObjectSb.AppendLine("\t}");

				// Textures

				// TODO: FBX import currently only supports Diffuse Color and Normal Map
				// Because it is undocumented, there is no way to easily find out what other textures
				// can be attached to an FBX file so it is imported into the PBR shaders at the same time.
				// Also NOTE, Unity 5.1.2 will import FBX files with legacy shaders. This is fix done
				// in at least 5.3.4.

				// Serialize first texture only
				var texture = mat.textureSet.tex_diffuse;
				if (texture != null) {
					SerializeOneTexture(mat.UbiArtContext, texture, materialName + "_tex", "DiffuseColor", tempObjectSb, tempConnectionsSb, true, outputPath, referenceId, materialName, header);
				}
				id++;
			}

			matObjects = tempObjectSb.ToString();
			connections = tempConnectionsSb.ToString();
		}


		private static bool SerializeOneTexture(Context context,
												TextureCooked texture,
												string filename,
												string textureType,
												StringBuilder objectsSb,
												StringBuilder connectionsSb,
												bool extractTextures,
												string outputPath,
												uint materialId,
												string materialName,
												FBXHeader header) {
			if (texture == null) return false;

			header.TextureCount++;
			string textureFilePathFullName = $"Textures/{filename}.png";
			var fullPath = System.IO.Path.Combine(outputPath, textureFilePathFullName);

			if (extractTextures && !File.Exists(fullPath)) {
				var tex = texture.GetUnityTexture(context)?.TextureForExport;
				if (tex == null) {
				} else {
					var colors = tex.GetPixels();
					var newTex = new Texture2D(tex.width, tex.height);
					newTex.SetPixels(colors);
					newTex.Apply();
					tex = newTex;
					Util.ByteArrayToFile(fullPath, tex.EncodeToPNG());
				}
			}

			long textureReference = texture.GetHashCode();
			var textureName = $"{filename:X8}";

			objectsSb.AppendLine($"\tTexture: {textureReference}, \"Texture::{materialName}\", \"\" {{");
			objectsSb.AppendLine("\t\tType: \"TextureVideoClip\"");
			objectsSb.AppendLine("\t\tVersion: 202");
			objectsSb.AppendLine($"\t\tTextureName: \"Texture::{materialName}\"");
			objectsSb.AppendLine("\t\tProperties70:  {");
			objectsSb.AppendLine("\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",0");
			objectsSb.AppendLine("\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"map1\"");
			objectsSb.AppendLine("\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",1");
			objectsSb.AppendLine("\t\t}");
			objectsSb.AppendLine($"\t\tMedia: \"Video::{materialName}\"");

			// Sets the absolute path for the copied texture
			objectsSb.AppendLine($"\t\tFileName: \"/{textureFilePathFullName}\"");

			// Sets the relative path for the copied texture
			objectsSb.AppendLine($"\t\tRelativeFilename: \"/{textureFilePathFullName}\"");

			objectsSb.AppendLine("\t\tModelUVTranslation: 0,0"); // TODO: Figure out how to get the UV translation into here
			objectsSb.AppendLine("\t\tModelUVScaling: 1,1"); // TODO: Figure out how to get the UV scaling into here
			objectsSb.AppendLine("\t\tTexture_Alpha_Source: \"None\""); // TODO: Add alpha source here if the file is a cutout.
			objectsSb.AppendLine("\t\tCropping: 0,0,0,0");
			objectsSb.AppendLine("\t}");

			connectionsSb.AppendLine($"\t;Texture::{textureName}, Material::{materialName}");
			connectionsSb.AppendLine($"\tC: \"OP\",{textureReference},{materialId}, \"{textureType}\"");

			connectionsSb.AppendLine();

			return true;
		}
	}
}
