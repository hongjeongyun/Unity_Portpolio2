using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FellowOakDicom;
using System.IO;
using FellowOakDicom.Imaging.Render;

public class TestScript : MonoBehaviour
{
    [SerializeField] RawImage pixelData;
    
    void Start()
    {        
        var imageFilepath = @"c:\imagetest\test2.jpg"; //image file test
        
        if (!File.Exists(imageFilepath))
            Debug.Log("imageFiletath error");
        
        var tex = new Texture2D(2, 2);
        var bytes = File.ReadAllBytes(imageFilepath);        
        
        Debug.Log($"ReadAllBytes : {bytes.Length}");

        if (!tex.LoadImage(bytes))
            Debug.Log("fail LoadImage");
        
        Debug.Log($"tex.width : {tex.width}, tex.height : {tex.height}");                        
        tex.Apply();

        pixelData.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
        pixelData.material.mainTexture = tex;

        //Create test
        var gen = new DicomUIDGenerator();
        var file = new DicomFile();
        file.Dataset.AddOrUpdate(DicomTag.SOPClassUID, gen.Generate(DicomUID.SecondaryCaptureImageStorage));
        file.Dataset.AddOrUpdate(DicomTag.SOPInstanceUID, gen.Generate(DicomUID.SecondaryCaptureImageStorage));
        file.Dataset.AddOrUpdate(DicomTag.PatientName, "KIM");
        file.Dataset.AddOrUpdate(DicomTag.PatientID, "123456");
        file.Dataset.AddOrUpdate(DicomTag.Rows, (ushort)tex.height); //ushort ok
        file.Dataset.AddOrUpdate(DicomTag.Columns, (ushort)tex.width);
        file.Dataset.AddOrUpdate(DicomTag.ImageType, "ORIGINAL\\PRIMARY");
        file.Dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "RGB");
        file.Dataset.AddOrUpdate(DicomTag.BitsStored, (ushort)8);
        file.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
        file.Dataset.AddOrUpdate(DicomTag.HighBit, (ushort)7);
        file.Dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)0);
        file.Dataset.AddOrUpdate(DicomTag.PlanarConfiguration, (ushort)0);   
        file.Dataset.AddOrUpdate(DicomTag.PixelData, bytes);  
        
        //Debug.Log(file.Dataset.GetString(DicomTag.PatientName));
        //Debug.Log($"PixelData : {(file.Dataset.GetValues<byte>(DicomTag.PixelData)).Length}");
    }    
}
