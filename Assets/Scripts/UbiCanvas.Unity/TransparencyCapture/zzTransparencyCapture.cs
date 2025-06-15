﻿using UnityEngine;
using System.IO;

public class zzTransparencyCapture
{
    public static Texture2D  capture(Rect pRect, bool isTransparent)
    {
        Camera lCamera = Camera.main;
        //OutlineEffect outline = lCamera.GetComponent<OutlineEffect>();
        //if (outline != null) outline.enabled = false;
		RenderTexture renderTexture = new RenderTexture((int)pRect.width, (int)pRect.height, 32);
        Texture2D lOut;
        if (isTransparent) {
            var lPreClearFlags = lCamera.clearFlags;
            var lPreBackgroundColor = lCamera.backgroundColor;
            // Texture.ReadPixels reads from whatever texture is active. Ours needs to
            // be active. But let's remember the old one so we can restore it later.
            RenderTexture oldRenderTexture = RenderTexture.active;
            {
                lCamera.clearFlags = CameraClearFlags.Color;
                lCamera.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;

                //make two captures with black and white background
                lCamera.backgroundColor = Color.black;
                lCamera.Render();
                var lBlackBackgroundCapture = captureView(pRect);

                lCamera.backgroundColor = Color.white;
                lCamera.Render();
                var lWhiteBackgroundCapture = captureView(pRect);

                for (int x = 0; x < lWhiteBackgroundCapture.width; ++x) {
                    for (int y = 0; y < lWhiteBackgroundCapture.height; ++y) {
                        Color lColorWhenBlack = lBlackBackgroundCapture.GetPixel(x, y);
                        Color lColorWhenWhite = lWhiteBackgroundCapture.GetPixel(x, y);
                        if (lColorWhenBlack != Color.clear) {
                            //set real color
                            lWhiteBackgroundCapture.SetPixel(x, y,
                                getColor(lColorWhenBlack, lColorWhenWhite));
                        }
                    }
                }
                lWhiteBackgroundCapture.Apply();
                lOut = lWhiteBackgroundCapture;
                Object.DestroyImmediate(lBlackBackgroundCapture);
            }
            // Restore previous settings.
            Camera.main.targetTexture = null;
            RenderTexture.active = oldRenderTexture;
            lCamera.backgroundColor = lPreBackgroundColor;
            lCamera.clearFlags = lPreClearFlags;
        } else {
            var lPreClearFlags = lCamera.clearFlags;
            var lPreBackgroundColor = lCamera.backgroundColor;

            lCamera.clearFlags = CameraClearFlags.Color;
            lCamera.backgroundColor = new Color(lPreBackgroundColor.r, lPreBackgroundColor.g, lPreBackgroundColor.b, 1f);

            RenderTexture oldRenderTexture = RenderTexture.active;
            lCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            // Render
            lCamera.Render();
            lOut = captureView(pRect);
            // Restore previous settings.
            Camera.main.targetTexture = null;
            RenderTexture.active = oldRenderTexture;
            lCamera.backgroundColor = lPreBackgroundColor;
            lCamera.clearFlags = lPreClearFlags;
        }
        //if (outline != null) outline.enabled = true;
        return lOut;
    }

    /// <summary>
    /// Capture a screenshot(not include GUI)
    /// </summary>
    /// <returns></returns>
    public static Texture2D captureScreenshot(int width, int height, bool isTransparent)
    {
        return capture(new Rect(0f, 0f, width, height), isTransparent);
    }

    /// <summary>
    /// Capture a screenshot(not include GUI) at path filename as a PNG file
    /// eg. zzTransparencyCapture.captureScreenshot("Screenshot.png")
    /// </summary>
    /// <param name="pFileName"></param>
    /// <returns></returns>
    public static void captureScreenshot(string pFileName, bool isTransparent)
    {
        var lScreenshot = captureScreenshot(Screen.width, Screen.height, isTransparent);
        try
        {
            using (var lFile = new FileStream(pFileName, FileMode.Create))
            {
                BinaryWriter lWriter = new BinaryWriter(lFile);
                lWriter.Write(lScreenshot.EncodeToPNG());
            }
        }
        finally
        {
            Object.DestroyImmediate(lScreenshot);
        }
    }

    //pColorWhenBlack!=Color.clear
    static Color getColor(Color pColorWhenBlack,Color pColorWhenWhite)
    {
        float lAlpha = getAlpha(pColorWhenBlack.r, pColorWhenWhite.r);
        return new Color(
            Mathf.Clamp01(lAlpha > 0 ? (pColorWhenBlack.r / lAlpha) : pColorWhenBlack.r),
            Mathf.Clamp01(lAlpha > 0 ? (pColorWhenBlack.g / lAlpha) : pColorWhenBlack.g),
            Mathf.Clamp01(lAlpha > 0 ? (pColorWhenBlack.b / lAlpha) : pColorWhenBlack.b),
            lAlpha);
    }


    //           Color*Alpha      Color   Color+(1-Color)*(1-Alpha)=1+Color*Alpha-Alpha
    //0----------ColorWhenZero----Color---ColorWhenOne------------1
    static float getAlpha(float pColorWhenZero, float pColorWhenOne)
    {
        //pColorWhenOne-pColorWhenZero=1-Alpha
        return Mathf.Clamp01(1 + pColorWhenZero - pColorWhenOne);
    }

    static Texture2D captureView(Rect pRect)
    {
        Texture2D lOut = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.ARGB32, false);
        lOut.ReadPixels(pRect, 0, 0, false);
        return lOut;
    }

}