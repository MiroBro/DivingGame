using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    public Tilemap tileMap;
    public Tile defaultTile;

    public float cutOffForTile;



    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    void Start()
    {
        map = new int[width, height];
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;

        CalcNoise();
    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        float z = 0.0F;

        while (z < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + z / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)z * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            z++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pix[(int)y * noiseTex.width + (int)x].r > cutOffForTile)
                {
                    map[x, y] = 1;
                }
                else
                {
                    //Set edges to filled
                    //if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    //{
                    //    map[x, y] = 1;
                    //}
                    //else
                    {
                        map[x, y] = 0;
                    }
                }

                if (map[x, y] == 1)
                    tileMap.SetTile(new Vector3Int(x, y, 0), defaultTile);
                else
                    tileMap.SetTile(new Vector3Int(x, y, 0), null);

            }
        }
    }

    void Update()
    {
        //CalcNoise();
    }


    // Start is called before the first frame update
    //void Start()
    //{
   //     GenerateMap();
    //}

    /*
    private void GenerateMap()
    {
        map = new int[width, height];
        var noise = Mathf.PerlinNoise(UnityEngine.Random.Range(0, 100f), UnityEngine.Random.Range(0, 100f));
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                //if (noise)
            }
        //RandomFillMap();
    }

    private void RandomFillMap()
    {
        if (useRandomSeed) 
        {
            seed = Time.time.ToString();       
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                //Set edges to filled
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else 
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }

                if (map[x,y] == 1)
                    tileMap.SetTile(new Vector3Int(x, y, 0), defaultTile);
                else 
                    tileMap.SetTile(new Vector3Int(x, y, 0), null);

            }
        }
    }
    */
    /*
public void SmoothMap()
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            int neightbourWallTiles = GetSurroundingWallCount(x, y);

            if (neightbourWallTiles > 4)
            {
                map[x, y] = 1;
            }
            else 
            {
                map[x, y] = 0;
            }


        }
    }
}


private int GetSurroundingWallCount(int x, int y)
{
    int wallCount = 0;
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < length; y++)
        {

        } 
    }
}
*/
}
