using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

class PremultiplyAlpha : AssetPostprocessor
{
	protected void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter textureImporter = assetImporter as TextureImporter;

		if (textureImporter.textureType == TextureImporterType.Sprite)
		{
			Color[] pixels = texture.GetPixels();

			List<Color> premultiplied = new List<Color>();
			foreach (Color pixel in pixels)
			{
				Color modified = pixel;
				modified.r *= pixel.a;
				modified.g *= pixel.a;
				modified.b *= pixel.a;
				premultiplied.Add(modified);
			}

			texture.SetPixels(premultiplied.ToArray());
		}
	}
}
