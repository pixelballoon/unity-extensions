using UnityEngine;
using System.Collections;

/**
 
 TextSize for Unity3D by thienhaflash (thienhaflash@gmail.com)
 
 Version	: 0.1
 Update		: 18.Jun.2012
 Features	:
	Return perfect size for any TextMesh
 	Cache the size of each character to speed up the size
	Evaluate and cache only when there are requirements
 
 Sample 	:
		
		//declare it locally, so we can have access anywhere from the script
		TextSize ts;
		
		//put this on the Start function
	 	ts = new TextSize(gameObject.GetComponent<TextMesh>());
		
		//anywhere, after you change the text :
		print(ts.width);
		
		//or get the length of an abitrary text (that is not assign to TextMesh)
		print(ts.GetTextWidth("any abitrary text goes here"));

 You are free to use the code or modify it any way you want (even remove this block of comments) but if it's good
 please give it back to the community.
 
 */

public class TextSize
{

	private readonly Hashtable _dict; //map character -> width

	private readonly TextMesh _textMesh;
	private readonly Renderer _renderer;

	public TextSize(TextMesh textMesh)
	{
		_textMesh = textMesh;
		_renderer = textMesh.renderer;
		_dict = new Hashtable();
		GetSpace();
	}

	private void GetSpace()
	{
		//the space can not be got alone
		string oldText = _textMesh.text;

		_textMesh.text = "a";
		float aw = _renderer.bounds.size.x;
		_textMesh.text = "a a";
		float cw = _renderer.bounds.size.x - 2 * aw;

		_dict.Add(' ', cw);
		_dict.Add('a', aw);

		_textMesh.text = oldText;
	}

	public float GetTextWidth(string s)
	{
		char[] charList = s.ToCharArray();
		float w = 0;
		char c;
		string oldText = _textMesh.text;

		for (int i = 0; i < charList.Length; i++)
		{
			c = charList[i];

			if (_dict.ContainsKey(c))
			{
				w += (float)_dict[c];
			}
			else
			{
				_textMesh.text = "" + c;
				float cw = _renderer.bounds.size.x;
				_dict.Add(c, cw);
				w += cw;
			}
		}

		_textMesh.text = oldText;
		return w;
	}

	public void FitToWidth(float wantedWidth)
	{
		string oldText = _textMesh.text;
		_textMesh.text = "";

		oldText = oldText.Replace("\\n", "\n");

		string[] lines = oldText.Split('\n');

		foreach (string line in lines)
		{
			_textMesh.text += WrapLine(line, wantedWidth);
			_textMesh.text += "\n";
		}
	}

	private string WrapLine(string s, float w)
	{
		// need to check if smaller than maximum character length, really...
		if (w == 0 || s.Length <= 0) return s;

		char c;
		char[] charList = s.ToCharArray();

		float charWidth = 0;
		float wordWidth = 0;
		float currentWidth = 0;

		string word = "";
		string newText = "";
		string oldText = _textMesh.text;

		for (int i = 0; i < charList.Length; i++)
		{
			c = charList[i];

			if (_dict.ContainsKey(c))
			{
				charWidth = (float)_dict[c];
			}
			else
			{
				_textMesh.text = "" + c;
				charWidth = _renderer.bounds.size.x;
				_dict.Add(c, charWidth);
				//here check if max char length
			}

			if (c == ' ' || i == charList.Length - 1)
			{
				if (c != ' ')
				{
					word += c.ToString();
					wordWidth += charWidth;
				}

				if (currentWidth + wordWidth < w)
				{
					currentWidth += wordWidth;
					newText += word;
				}
				else
				{
					currentWidth = wordWidth;
					newText += word.Replace(" ", "\n");
				}

				word = "";
				wordWidth = 0;
			}

			word += c.ToString();
			wordWidth += charWidth;
		}

		_textMesh.text = oldText;
		return newText;
	}

	public float Width { get { return GetTextWidth(_textMesh.text); } }
	public float Height { get { return _renderer.bounds.size.y; } }

}