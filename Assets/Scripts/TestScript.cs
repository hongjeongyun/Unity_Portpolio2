using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FellowOakDicom;
using System.IO;
using TMPro;

enum DicomTagEnums
{
    SOPClassUID = 0,
    SOPInstanceUID,
    PatientName,
    PatientID,
    Rows,
    Columns,
    ImageType,
    PhotometricInterpretation,
    BitsStored,
    BitsAllocated,
    HighBit,
    PixelRepresentation,
    PlanarConfiguration,
    PixelData,
}

public class TestScript : MonoBehaviour
{
    [SerializeField] RawImage PixelData;
    [SerializeField] RectTransform ContentTransform;
    [SerializeField] GameObject ItemPrefab;
    public int itemCount = 13;
    public List<GameObject> items = new(13);

    private readonly List<DicomTag> dicomTags = new() 
    {
        DicomTag.SOPClassUID, 
        DicomTag.SOPInstanceUID,
        DicomTag.PatientName,
        DicomTag.PatientID,
        DicomTag.Rows,
        DicomTag.Columns,
        DicomTag.ImageType,
        DicomTag.PhotometricInterpretation,
        DicomTag.BitsStored,
        DicomTag.BitsAllocated,
        DicomTag.HighBit,
        DicomTag.PixelRepresentation,
        DicomTag.PlanarConfiguration,
        DicomTag.PixelData,
    };
    
    void Start()
    {
        var imageFilepath = @"c:\imagetest\test2.jpg"; //image file test
        var pixelTexture = LoadFromPath(imageFilepath, out byte[] pixelBytes);
        var dicomFile = TestCreate(pixelTexture.width, pixelTexture.height, pixelBytes);
                
        TestCreateScrollList();
        TestSetToUI(dicomFile);

        PixelData.rectTransform.sizeDelta = new Vector2(pixelTexture.width, pixelTexture.height);
        PixelData.material.mainTexture = pixelTexture;

        //Debug.Log(dicomFile.Dataset.GetString(DicomTag.PatientName));
        //Debug.Log($"PixelData : {(dicomFile.Dataset.GetValues<byte>(DicomTag.PixelData)).Length}");
    }

    void TestCreateScrollList() 
    {
        int y = -25;
        for (var i = 0; i < itemCount; i++)
        {
            var item = Instantiate(ItemPrefab, ContentTransform);
            item.GetComponent<RectTransform>().localPosition = new Vector3(256, y, 0);
            y -= 50;
            items.Add(item);
        }
    }
    Texture2D LoadFromPath(string imageFilepath, out byte[] bytes)
    {
        if (!File.Exists(imageFilepath))
            Debug.Log("imageFiletath error");

        var tex = new Texture2D(2, 2);
        bytes = File.ReadAllBytes(imageFilepath);

        Debug.Log($"ReadAllBytes : {bytes.Length}");

        if (!tex.LoadImage(bytes))
            Debug.Log("fail LoadImage");
        Debug.Log($"tex.width : {tex.width}, tex.height : {tex.height}");
        tex.Apply();
        return tex;
    }
    DicomFile TestCreate(int w, int h, byte[] pixelBytes)
    {
        //var gen = new DicomUIDGenerator();
        var file = new DicomFile();
        file.Dataset.AddOrUpdate(DicomTag.SOPClassUID, "11");//gen.Generate(DicomUID.SecondaryCaptureImageStorage));
        file.Dataset.AddOrUpdate(DicomTag.SOPInstanceUID, "22");//gen.Generate(DicomUID.SecondaryCaptureImageStorage));
        file.Dataset.AddOrUpdate(DicomTag.PatientName, "KIM");
        file.Dataset.AddOrUpdate(DicomTag.PatientID, "123456");
        file.Dataset.AddOrUpdate(DicomTag.Rows, (ushort)h); //ushort ok
        file.Dataset.AddOrUpdate(DicomTag.Columns, (ushort)w);
        file.Dataset.AddOrUpdate(DicomTag.ImageType, "ORIGINAL\\PRIMARY");
        file.Dataset.AddOrUpdate(DicomTag.PhotometricInterpretation, "RGB");
        file.Dataset.AddOrUpdate(DicomTag.BitsStored, (ushort)9);//8
        file.Dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);//8
        file.Dataset.AddOrUpdate(DicomTag.HighBit, (ushort)7);//7
        file.Dataset.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)1);//0
        file.Dataset.AddOrUpdate(DicomTag.PlanarConfiguration, (ushort)2);//0
        file.Dataset.AddOrUpdate(DicomTag.PixelData, pixelBytes);
        return file;
    }
    void TestSetToUI(DicomFile file)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (dicomTags[i] != DicomTag.PixelData)
            {
                file.Dataset.TryGetString(dicomTags[i], out string sv);
                items[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{(DicomTagEnums)i} : {sv}";
            }   
        }                        
    }
}
