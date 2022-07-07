using SharpConfig;
using System.IO;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    public string serverAddr;
    public int serverPort;
    public string nanoleafAddr;
    public int nanoleafPort;
    public string nanoleafAuthToken;

    private Configuration cfg = new Configuration();

    private readonly string cfgFilePath = "config.cfg";

    public static ConfigurationManager Instance { get; private set; }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        FileInfo fi = new FileInfo(string.Format("{0}/{1}", Application.persistentDataPath, cfgFilePath));
        if (!fi.Exists)
        {
            Debug.Log("[ConfigurationManager] Creating new config file " + fi.FullName);
            CreateNewCfg();
            WriteCfg(fi.FullName);
        }
        else
        {
            Debug.Log("[ConfigurationManager] Reading from config file " + fi.FullName);
            cfg = Configuration.LoadFromFile(fi.FullName);
            LoadFromCfg();
        }
    }

    private void CreateNewCfg()
    {
        cfg["Network"]["ServerAddr"].StringValue = serverAddr;
        cfg["Network"]["ServerPort"].IntValue = serverPort;
        cfg["Network"]["NanoleafAddr"].StringValue = nanoleafAddr;
        cfg["Network"]["NanoleafPort"].IntValue = nanoleafPort;
        cfg["Network"]["NanoleafAuthToken"].StringValue = nanoleafAuthToken;
    }

    private void WriteCfg(string filePath)
    {
        Debug.Log("[ConfigurationManager] Writing CFG file");
        cfg.SaveToFile(filePath);
    }

    private void LoadFromCfg()
    {
        serverAddr = cfg["Network"]["ServerAddr"].StringValue;
        serverPort = cfg["Network"]["ServerPort"].IntValue;
        nanoleafAddr = cfg["Network"]["NanoleafAddr"].StringValue;
        nanoleafPort = cfg["Network"]["NanoleafPort"].IntValue;
        nanoleafAuthToken = cfg["Network"]["NanoleafAuthToken"].StringValue;
    }
}
