using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AssetGenerator
{
    [MenuItem("Tools/Generate Assets")]
    public static void GenerateAllAssets()
    {
        Debug.Log("Generating audio and texture assets...");

        if (!Directory.Exists("Assets/Audio")) Directory.CreateDirectory("Assets/Audio");
        if (!Directory.Exists("Assets/Textures")) Directory.CreateDirectory("Assets/Textures");
        if (!Directory.Exists("Assets/Prefabs")) Directory.CreateDirectory("Assets/Prefabs");
        if (!Directory.Exists("Assets/Settings")) Directory.CreateDirectory("Assets/Settings");

        // 1. Generate Textures
        Texture2D checker = CreateCheckerTexture(256, 256, Color.gray, Color.black);
        File.WriteAllBytes("Assets/Textures/Checker.png", checker.EncodeToPNG());
        
        Texture2D noise = CreateNoiseTexture(256, 256);
        File.WriteAllBytes("Assets/Textures/Noise.png", noise.EncodeToPNG());

        AssetDatabase.Refresh(); // Refresh to allow loading textures

        // 2. Generate Audio (.wav)
        GenerateWav("Assets/Audio/Ambience.wav", 44100, 3.0f, (t) => Mathf.Sin(t * 100f * Mathf.PI * 2f) * 0.1f);
        GenerateWav("Assets/Audio/Click.wav", 44100, 0.1f, (t) => (t * 400f % 1f > 0.5f ? 0.5f : -0.5f) * Mathf.Lerp(1, 0, t / 0.1f));
        GenerateWav("Assets/Audio/Success.wav", 44100, 0.5f, (t) => Mathf.Sin(t * 800f * Mathf.PI * 2f) * Mathf.Lerp(1, 0, t / 0.5f));
        GenerateWav("Assets/Audio/Error.wav", 44100, 0.5f, (t) => Mathf.Sin(t * 150f * Mathf.PI * 2f) * Mathf.Lerp(1, 0, t / 0.5f));
        GenerateWav("Assets/Audio/Door.wav", 44100, 1.5f, (t) => (UnityEngine.Random.value * 2f - 1f) * 0.2f * Mathf.Lerp(1, 0, t / 1.5f));
        GenerateWav("Assets/Audio/Victory.wav", 44100, 2.0f, (t) => Mathf.Sin(t * 440f * (1f + t) * Mathf.PI * 2f) * Mathf.Lerp(1, 0, t / 2.0f));

        AssetDatabase.Refresh();

        Debug.Log("Asset generation complete.");
    }

    private static Texture2D CreateCheckerTexture(int width, int height, Color c1, Color c2)
    {
        Texture2D tex = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isC1 = ((x / 32) % 2 == 0) ^ ((y / 32) % 2 == 0);
                tex.SetPixel(x, y, isC1 ? c1 : c2);
            }
        }
        tex.Apply();
        return tex;
    }

    private static Texture2D CreateNoiseTexture(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float v = UnityEngine.Random.value;
                tex.SetPixel(x, y, new Color(v, v, v));
            }
        }
        tex.Apply();
        return tex;
    }

    private static void GenerateWav(string path, int sampleRate, float duration, Func<float, float> synthFunction)
    {
        int samples = (int)(sampleRate * duration);
        short[] audioData = new short[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float val = synthFunction(t);
            val = Mathf.Clamp(val, -1f, 1f);
            audioData[i] = (short)(val * short.MaxValue);
        }

        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + samples * 2);
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1); // PCM
            writer.Write((short)1); // 1 Channel
            writer.Write(sampleRate);
            writer.Write(sampleRate * 2); // Byte Rate
            writer.Write((short)2); // Block Align
            writer.Write((short)16); // Bits per sample
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(samples * 2);

            foreach (short sample in audioData)
            {
                writer.Write(sample);
            }
        }
    }
}
